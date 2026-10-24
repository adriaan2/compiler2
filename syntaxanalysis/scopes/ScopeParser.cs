namespace syntaxer.scopes;

using bindings;
using System.Text;
using System.Text.RegularExpressions;

using syntaxer;


public class ScopeParser
{

        //private object Iftoken;
        public ScopeParser? Parent;
        private readonly string _sourcetext = null;
        private readonly List<string> _diagnostics = new();
        private readonly List<ScopeParser> _subblocks = new();
        // Set true when an if-statement's condition evaluates to false; the next
        // brace block is then skipped instead of being parsed and bound.
        private bool _skipNextBlock = false;
        private LiteralExpressionsyntax? _pendingLoopCondition;
        private FunctionDefinition? _pendingFunction;
        private readonly FunctionExecutionContext? _functionExecution;
        private readonly Dictionary<VariableSymbol, object> _variables;
        public Functions FunctionMap { get; }
        internal Dictionary<VariableSymbol, object> Variables => _variables;
        public Dictionary<string, VariableSymbol> VariableSymbols = new();

        // Looks up a variable by name in this scope, then walks up the parent chain.
        public bool TryLookupVariable(string name, out VariableSymbol variable)
        {
                if (VariableSymbols.TryGetValue(name, out var localVariable))
                {
                        variable = localVariable;
                        return true;
                }

                if (Parent is not null)
                        return Parent.TryLookupVariable(name, out variable);

                variable = null!;
                return false;
        }


        public ScopeParser(string sourcetext)
        {
                _sourcetext = sourcetext;
                _variables = new Dictionary<VariableSymbol, object>();
                FunctionMap = new Functions();
        }

        // Wraps an existing symbol table as a scope (used when binding without parsing a full source block).
        public ScopeParser(Dictionary<string, VariableSymbol> variableSymbols)
        {
                VariableSymbols = variableSymbols;
                _variables = new Dictionary<VariableSymbol, object>();
                FunctionMap = new Functions();
        }
        public ScopeParser(string sourcetext, ScopeParser parent)
        {
                _sourcetext = sourcetext;
                Parent = parent;
                _variables = parent._variables;
                FunctionMap = parent.FunctionMap;
                _functionExecution = parent._functionExecution;
        }

        internal ScopeParser(string sourcetext, ScopeParser parent, FunctionExecutionContext functionExecution)
            : this(sourcetext, parent)
        {
                _functionExecution = functionExecution;
        }

        internal void AddParameter(FunctionParameter parameter, object value)
        {
                var symbol = new VariableSymbol(parameter.Name, parameter.Type, parameter.ElementType);
                VariableSymbols.Add(parameter.Name, symbol);
                Variables[symbol] = value;
        }



        public IEnumerable<string> Diagnostics => _diagnostics;
        public IReadOnlyList<ScopeParser> Subblocks => _subblocks;

        public IReadOnlyList<LiteralExpressionsyntax> Parse()
        {
                var expressions = new List<LiteralExpressionsyntax>();
                var blockText = new StringBuilder();
                int depth = 0;

                string[] ParseUnits = _sourcetext.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                foreach (string parseable in ParseUnits)
                {
                        if (_functionExecution?.HasReturned == true)
                                break;
                        var statement = new StringBuilder();
                        foreach (char c in parseable)
                        {
                                if (c == '{')
                                {
                                        if (depth == 0 && statement.Length > 0 && TryParseFunctionHeader(statement.ToString(), out var inlineFunction))
                                        {
                                                _pendingFunction = inlineFunction;
                                                statement.Clear();
                                        }
                                        depth++;
                                        // The outermost brace only delimits the block; inner braces
                                        // stay in the text so the child scope can recurse on them.
                                        if (depth == 1)
                                                continue;
                                }
                                else if (c == '}')
                                {
                                        if (depth == 0)
                                        {
                                                _diagnostics.Add("unexpected '}' without a matching '{'");
                                                continue;
                                        }

                                        depth--;
                                        if (depth == 0)
                                        {
                                                // A false if-condition skips the whole block: the text is
                                                // discarded instead of being recursed into and bound.
                                                if (_pendingFunction is not null)
                                                {
                                                        var function = _pendingFunction with { Body = blockText.ToString() };
                                                        _pendingFunction = null;
                                                        FunctionMap.Add(function, this);
                                                }
                                                else if (_skipNextBlock)
                                                {
                                                        System.Console.WriteLine("if condition was false, skipping block");
                                                        System.Console.WriteLine();
                                                        _skipNextBlock = false;
                                                }
                                                else if (_pendingLoopCondition is not null)
                                                {
                                                        var condition = _pendingLoopCondition;
                                                        _pendingLoopCondition = null;
                                                        while (_functionExecution?.HasReturned != true && ShouldEnterConditionalBlock(condition))
                                                                ParseBlock(blockText.ToString(), expressions);
                                                }
                                                else
                                                {
                                                        ParseBlock(blockText.ToString(), expressions);
                                                }
                                                blockText.Clear();
                                                continue;
                                        }
                                }

                                if (depth > 0)
                                        blockText.Append(c);
                                else
                                        statement.Append(c);
                        }

                        if (depth > 0)
                                blockText.Append('\n');

                        // Statements separated by ';' on one line are parsed one at a time,
                        // because the lexer treats ';' as end of input.
                        foreach (var piece in statement.ToString().Split(';'))
                        {
                                if (_functionExecution?.HasReturned == true)
                                        break;
                                var text = piece.Trim();
                                if (text.Length > 0)
                                        BindAndPrint(text + ";", expressions);
                        }
                }

                if (depth != 0)
                        _diagnostics.Add("openbrackets not closed");

                return expressions;
        }

        private void BindAndPrint(string statementText, List<LiteralExpressionsyntax> expressions)
        {
                if (TryParseFunctionHeader(statementText, out var function))
                {
                        _pendingFunction = function;
                        return;
                }

                Parser parser = new(statementText);
                var expression = parser.parse();
                _diagnostics.AddRange(parser.Diagnostics);
                expressions.Add(expression);

                PrettyPrint(expression);

                // An if-statement binds and evaluates its condition here; when the
                // condition is false the following block is flagged to be skipped.
                if (expression is Ifsyntax ifStatement)
                {
                        BindConditionalStatement(ifStatement.Condition);
                        System.Console.WriteLine();
                        return;
                }
                if (expression is LoopSyntax loop)
                {
                        _pendingLoopCondition = loop.Condition;
                        System.Console.WriteLine();
                        return;
                }
                if (expression is ReturnSyntax returnSyntax)
                {
                        BindReturnStatement(returnSyntax, parser.Diagnostics);
                        return;
                }

                Binder binder = new(this);
                var boundExpression = binder.Bind(expression);
                _diagnostics.AddRange(binder.Diagnostics);

                if (!binder.Diagnostics.Any() && !parser.Diagnostics.Any())
                {
                        new Evaluator(boundExpression, Variables).Evaluate();
                }

                System.Console.WriteLine();
        }

        private void BindReturnStatement(ReturnSyntax returnSyntax, IEnumerable<string> parserDiagnostics)
        {
                if (_functionExecution is null)
                {
                        _diagnostics.Add("return can only be used inside a function.");
                        return;
                }

                var definition = _functionExecution.Definition;
                if (definition.ReturnType == typeof(void))
                {
                        if (returnSyntax.Expression is not null)
                                _diagnostics.Add($"Void function '{definition.Name}' cannot return a value.");
                        else
                                _functionExecution.HasReturned = true;
                        return;
                }

                if (returnSyntax.Expression is null)
                {
                        _diagnostics.Add($"Function '{definition.Name}' must return a value.");
                        return;
                }

                var binder = new Binder(this);
                var bound = binder.Bind(returnSyntax.Expression);
                _diagnostics.AddRange(binder.Diagnostics);
                _diagnostics.AddRange(parserDiagnostics);
                var actualElementType = bound is BoundArrayexpression array ? array.ElementType :
                    bound is BoundVariableexpression variable ? variable.Variable.ElementType :
                    bound is BoundAssignmentexpression assignment ? assignment.Variable.ElementType : null;
                if (bound.Type != definition.ReturnType ||
                    (definition.ReturnType == typeof(ArrayValue) && actualElementType != definition.ReturnElementType))
                {
                        _diagnostics.Add($"Function '{definition.Name}' must return {GetFunctionTypeName(definition.ReturnType, definition.ReturnElementType)}.");
                        return;
                }

                if (binder.Diagnostics.Any() || parserDiagnostics.Any())
                        return;
                _functionExecution.Value = new Evaluator(bound, Variables).Evaluate();
                _functionExecution.HasReturned = true;
        }

        private static string GetFunctionTypeName(Type type, Type? elementType) =>
            type == typeof(ArrayValue) ? $"{elementType?.Name}[]" : type.Name;

        private static bool TryParseFunctionHeader(string text, out FunctionDefinition function)
        {
                function = null!;
                var match = Regex.Match(text.Trim().TrimEnd(';'), @"^(int|char|void)(\[\])?\s+([A-Za-z_]\w*)\s*\((.*)\)$");
                if (!match.Success)
                        return false;

                var returnType = ParseFunctionType(match.Groups[1].Value, match.Groups[2].Success);
                var name = match.Groups[3].Value;
                var parameters = new List<FunctionParameter>();
                var parameterText = match.Groups[4].Value.Trim();
                if (parameterText.Length > 0)
                {
                        foreach (var item in parameterText.Split(','))
                        {
                                var parameterMatch = Regex.Match(item.Trim(), @"^(int|char)(\[\])?\s+([A-Za-z_]\w*)$");
                                if (!parameterMatch.Success)
                                        throw new InvalidOperationException($"Invalid parameter declaration '{item.Trim()}' in function '{name}'.");
                                var (type, elementType) = ParseFunctionType(parameterMatch.Groups[1].Value, parameterMatch.Groups[2].Success);
                                parameters.Add(new FunctionParameter(parameterMatch.Groups[3].Value, type, elementType));
                        }
                }

                function = new FunctionDefinition(name, returnType.Type, returnType.ElementType, parameters, string.Empty);
                return true;
        }

        private static (Type Type, Type? ElementType) ParseFunctionType(string name, bool isArray) =>
            isArray ? (typeof(ArrayValue), name == "int" ? typeof(int) : typeof(char)) :
            name switch
            {
                    "int" => (typeof(int), null),
                    "char" => (typeof(char), null),
                    "void" => (typeof(void), null),
                    _ => throw new InvalidOperationException($"Unsupported function type '{name}'.")
            };

        private void BindConditionalStatement(LiteralExpressionsyntax condition)
        {
                Binder binder = new(this);
                var enterBlock = binder.ShouldEnterConditionalBlock(condition, Variables);
                _diagnostics.AddRange(binder.Diagnostics);

                if (!enterBlock)
                        _skipNextBlock = true;
        }

        private bool ShouldEnterConditionalBlock(LiteralExpressionsyntax condition)
        {
                var binder = new Binder(this);
                var enter = binder.ShouldEnterConditionalBlock(condition, Variables);
                _diagnostics.AddRange(binder.Diagnostics);
                return enter;
        }

        private void ParseBlock(string blockText, List<LiteralExpressionsyntax> expressions)
        {
                ScopeParser trigger = new(blockText, this);
                _subblocks.Add(trigger);

                System.Console.WriteLine("{");
                System.Console.WriteLine();
                trigger.Parse();
                System.Console.WriteLine("}");
                System.Console.WriteLine();

                _diagnostics.AddRange(trigger.Diagnostics);
                // Keep child statements in the child's result. Flattening them here
                // would make them appear to belong to this scope downstream.
        }



        private sealed record ParseUnit(string Text, int Line);

        public static void PrettyPrint(GuidanceSignalNode node)
        {
                PrettyPrint2(node, "", true);
        }

        private static void PrettyPrint2(GuidanceSignalNode node, string indent, bool isLast)
        {
                var marker = indent == "" ? "" : (isLast ? "+--" : "+--");
                Console.WriteLine($"{indent}{marker}{GetNodeLabel(node)}");

                indent += indent == "" ? "    " : (isLast ? "    " : "|   ");

                var children = node.getchildren().ToArray();
                for (var i = 0; i < children.Length; i++)
                        PrettyPrint2(children[i], indent, i == children.Length - 1);
        }

        private static string GetNodeLabel(GuidanceSignalNode node)
        {
                return node.Kind switch
                {
                        GuidanceSignalKind.binaryexpression => "BinaryExpression",
                        GuidanceSignalKind.unaryexpression => "UnaryExpression",
                        GuidanceSignalKind.numberexpression => "NumberExpression",
                        GuidanceSignalKind.booleanexpression => "BooleanExpression",
                        GuidanceSignalKind.arrayexpression => "ArrayExpression",
                        GuidanceSignalKind.arrayindexexpression => "ArrayIndexExpression",
                        GuidanceSignalKind.typeclause => "TypeClause",
                        GuidanceSignalKind.nameexpression => "NameExpression",
                        GuidanceSignalKind.assignmentexpression => "AssignmentExpression",
                        GuidanceSignalKind.variabledeclarationexpression => "VariableDeclarationExpression",
                        GuidanceSignalKind.numberToken => $"NumberToken {(node as Syntaxtoken)?.Value}",
                        GuidanceSignalKind.openbracket => "OpenBracketToken",
                        GuidanceSignalKind.closebracket => "CloseBracketToken",
                        GuidanceSignalKind.commaToken => "CommaToken",
                        GuidanceSignalKind.identifierToken => $"IdentifierToken {(node as Syntaxtoken)?.Text}",
                        GuidanceSignalKind.trueKeyword => "TrueKeyword",
                        GuidanceSignalKind.falseKeyword => "FalseKeyword",
                        GuidanceSignalKind.intKeyword => "IntKeyword",
                        GuidanceSignalKind.boolKeyword => "BoolKeyword",
                        GuidanceSignalKind.charkeyword => "CharKeyword",
                        GuidanceSignalKind.equalsToken => "EqualsToken",
                        GuidanceSignalKind.equalsEqualsToken => "EqualsEqualsToken",
                        GuidanceSignalKind.plusToken => "PlusToken",
                        GuidanceSignalKind.minusToken => "MinusToken",
                        GuidanceSignalKind.timestoken => "StarToken",
                        GuidanceSignalKind.slashtoken => "SlashToken",
                        GuidanceSignalKind.bangToken => "BangToken",
                        GuidanceSignalKind.ampersandAmpersandToken => "AmpersandAmpersandToken",
                        GuidanceSignalKind.pipePipeToken => "PipePipeToken",
                        _ => node.Kind.ToString()
                };
        }
}
