namespace syntaxer.scopes;

using bindings;
using System.Text;

using syntaxer;


public class ScopeParser
{

    //private object Iftoken;
    public ScopeParser ?Parent;
    private readonly string _sourcetext = null;
    private readonly List<string> _diagnostics = new();
    private readonly List<ScopeParser> _subblocks = new();
    // Set true when an if-statement's condition evaluates to false; the next
    // brace block is then skipped instead of being parsed and bound.
    private bool _skipNextBlock = false;
    private readonly Dictionary<VariableSymbol, object> _variables;
    internal Dictionary<VariableSymbol, object> Variables => _variables;
    public Dictionary<string, VariableSymbol> VariableSymbols=new();

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
    }

    // Wraps an existing symbol table as a scope (used when binding without parsing a full source block).
    public ScopeParser(Dictionary<string, VariableSymbol> variableSymbols)
    {
        VariableSymbols = variableSymbols;
        _variables = new Dictionary<VariableSymbol, object>();
    }
     public ScopeParser(string sourcetext, ScopeParser parent)
    {
        _sourcetext = sourcetext;
        Parent=parent;
        _variables = parent._variables;
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
            var statement = new StringBuilder();
            foreach (char c in parseable)
            {
                if (c == '{')
                {
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
                        if (_skipNextBlock)
                        {
                            System.Console.WriteLine("if condition was false, skipping block");
                            System.Console.WriteLine();
                            _skipNextBlock = false;
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
        Parser parser = new(statementText);
        var expression = parser.parse();
        _diagnostics.AddRange(parser.Diagnostics);
        expressions.Add(expression);

        PrettyPrint(expression);

        // An if-statement binds and evaluates its condition here; when the
        // condition is false the following block is flagged to be skipped.
        if (expression is Ifsyntax ifStatement)
        {
            BindIfStatement(ifStatement);
            System.Console.WriteLine();
            return;
        }

        Binder binder = new(this);
        var boundExpression = binder.Bind(expression);
        _diagnostics.AddRange(binder.Diagnostics);

        if (!binder.Diagnostics.Any() && !parser.Diagnostics.Any())
        {
            try
            {
                new Evaluator(boundExpression, Variables).Evaluate();
            }
            catch (Exception exception)
            {
                _diagnostics.Add(exception.Message);
            }
        }

        System.Console.WriteLine();
    }

    private void BindIfStatement(Ifsyntax ifStatement)
    {
        Binder binder = new(this);
        // The binder evaluates the boolean condition; a false result flags the
        // following block to be skipped rather than recursed into.
        var enterBlock = binder.ShouldEnterIfBlock(ifStatement, Variables);
        _diagnostics.AddRange(binder.Diagnostics);

        if (!enterBlock)
            _skipNextBlock = true;
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
        PrettyPrint(node, "", true);
    }

    private static void PrettyPrint(GuidanceSignalNode node, string indent, bool isLast)
    {
        var marker = indent == "" ? "" : (isLast ? "+--" : "+--");
        Console.WriteLine($"{indent}{marker}{GetNodeLabel(node)}");

        indent += indent == "" ? "    " : (isLast ? "    " : "|   ");

        var children = node.getchildren().ToArray();
        for (var i = 0; i < children.Length; i++)
            PrettyPrint(children[i], indent, i == children.Length - 1);
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
