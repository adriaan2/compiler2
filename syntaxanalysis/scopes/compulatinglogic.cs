namespace syntaxer.scopes;
using bindings;

using syntaxer;

public class compulatinglogic
{
        private Dictionary<VariableSymbol, object> variables=new();
        private readonly string _sourcetext;
        private Parser? _parser;

        public compulatinglogic(string sourcetext)
        {
                _sourcetext = sourcetext;
        }

        public IEnumerable<string> Diagnostics
                => _parser?.Diagnostics ?? Enumerable.Empty<string>();

        public CompilationUnitSyntax Parse()
        {
                _parser = new Parser(_sourcetext);

                var syntax = _parser.ParseCompilationUnit();
                PrettyPrint(syntax);

                return syntax;
        }

        public static void PrettyPrint(SyntaxNode node)
        {
                PrettyPrint(node, "", true);
        }

        private static void PrettyPrint(SyntaxNode node, string indent, bool isLast)
        {
                var marker = indent == "" ? "" : (isLast ? "+--" : "+--");
                Console.WriteLine($"{indent}{marker}{GetNodeLabel(node)}");

                indent += indent == "" ? "    " : (isLast ? "    " : "|   ");

                var children = node.getchildren().ToArray();
                for (var i = 0; i < children.Length; i++)
                        PrettyPrint(children[i], indent, i == children.Length - 1);
        }

        private static string GetNodeLabel(SyntaxNode node)
        {
                return node.Kind switch
                {
                        SyntaxKind.binaryexpression => "BinaryExpression",
                        SyntaxKind.unaryexpression => "UnaryExpression",
                        SyntaxKind.numberexpression => "NumberExpression",
                        SyntaxKind.booleanexpression => "BooleanExpression",
                        SyntaxKind.bracketexpression => "ParenthesizedExpression",
                        SyntaxKind.charvaltoken when node is Charsyntax => "CharExpression",
                        SyntaxKind.arrayexpression => "ArrayExpression",
                        SyntaxKind.arrayindexexpression => "ArrayIndexExpression",
                        SyntaxKind.typeclause => "TypeClause",
                        SyntaxKind.nameexpression => "NameExpression",
                        SyntaxKind.assignmentexpression => "AssignmentExpression",
                        SyntaxKind.variabledeclarationexpression => "VariableDeclarationExpression",
                        SyntaxKind.compilationunit => "CompilationUnit",
                        SyntaxKind.expressionstatement => "ExpressionStatement",
                        SyntaxKind.numberToken => $"NumberToken {(node as Syntaxtoken)?.Value}",
                        SyntaxKind.charvaltoken => $"CharToken {(node as Syntaxtoken)?.Value}",
                        SyntaxKind.badtoken => $"BadToken {(node as Syntaxtoken)?.Text}",
                        SyntaxKind.openbracket => "OpenBracketToken",
                        SyntaxKind.closebracket => "CloseBracketToken",
                        SyntaxKind.commaToken => "CommaToken",
                        SyntaxKind.identifierToken => $"IdentifierToken {(node as Syntaxtoken)?.Text}",
                        SyntaxKind.trueKeyword => "TrueKeyword",
                        SyntaxKind.falseKeyword => "FalseKeyword",
                        SyntaxKind.intKeyword => "IntKeyword",
                        SyntaxKind.boolKeyword => "BoolKeyword",
                        SyntaxKind.charkeyword => "CharKeyword",
                        SyntaxKind.equalsToken => "EqualsToken",
                        SyntaxKind.equalsEqualsToken => "EqualsEqualsToken",
                        SyntaxKind.plusToken => "PlusToken",
                        SyntaxKind.minusToken => "MinusToken",
                        SyntaxKind.timestoken => "StarToken",
                        SyntaxKind.slashtoken => "SlashToken",
                        SyntaxKind.bangToken => "BangToken",
                        SyntaxKind.ampersandAmpersandToken => "AmpersandAmpersandToken",
                        SyntaxKind.pipePipeToken => "PipePipeToken",
                        SyntaxKind.openparen => "OpenParenthesisToken",
                        SyntaxKind.closedparen => "CloseParenthesisToken",
                        SyntaxKind.semicolonToken => "SemicolonToken",
                        SyntaxKind.endoffiletoken => "EndOfFileToken",
                        _ => node.Kind.ToString()
                };
        }
}
