namespace syntaxer.scopes;

using bindings;

using syntaxer;


public class compulatinglogic
{

    public Dictionary<int, int> variablesymbols=new();
    //private object Iftoken;
    public compulatinglogic ?Parent;
    private readonly string _sourcetext = null;
    private readonly List<string> _diagnostics = new();
    Dictionary<VariableSymbol,object> Variables=new();
    Dictionary<string, VariableSymbol> VariableSymbols=new();
    

    public compulatinglogic(string sourcetext)
    {
        _sourcetext = sourcetext;
    }
     public compulatinglogic(string sourcetext,compulatinglogic parent)
    {
        _sourcetext = sourcetext;
        Parent=parent;
    }



    public IEnumerable<string> Diagnostics => _diagnostics;

    public IReadOnlyList<LiteralExpressionsyntax> Parse()
    {
        _diagnostics.Clear();
        var expressions = new List<LiteralExpressionsyntax>();
        var lines = _sourcetext.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var parser = new Parser(line);
            LiteralExpressionsyntax syntax = parser.parse();
            if (syntax is not Ifsyntax)
            {


                expressions.Add(syntax);
            }
            else
            {
                //Binder binder = new();
            }
            foreach (var diagnostic in parser.Diagnostics)
                _diagnostics.Add($"Line {i + 1}: {diagnostic}");

            PrettyPrint(syntax);
        }

        return expressions;
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
            SyntaxKind.arrayexpression => "ArrayExpression",
            SyntaxKind.arrayindexexpression => "ArrayIndexExpression",
            SyntaxKind.typeclause => "TypeClause",
            SyntaxKind.nameexpression => "NameExpression",
            SyntaxKind.assignmentexpression => "AssignmentExpression",
            SyntaxKind.variabledeclarationexpression => "VariableDeclarationExpression",
            SyntaxKind.numberToken => $"NumberToken {(node as Syntaxtoken)?.Value}",
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
            _ => node.Kind.ToString()
        };
    }
}
