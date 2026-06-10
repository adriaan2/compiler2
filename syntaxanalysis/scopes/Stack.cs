namespace syntaxer.scopes;

using bindings;
using System.Text;

using syntaxer;


public class Stack
{

    //private object Iftoken;
    public Stack ?Parent;
    private readonly string _sourcetext = null;
    private readonly List<string> _diagnostics = new();
    private readonly List<Stack> _subblocks = new();
    Dictionary<VariableSymbol,object> Variables=new();
    public Dictionary<string, VariableSymbol> VariableSymbols=new();

    // Looks up a variable by name in this scope, then walks up the parent chain.
    public bool TryLookupVariable(string name, out VariableSymbol variable)
    {
        if (VariableSymbols.TryGetValue(name, out variable))
            return true;

        if (Parent is not null)
            return Parent.TryLookupVariable(name, out variable);

        variable = null;
        return false;
    }
    

    public Stack(string sourcetext)
    {
        _sourcetext = sourcetext;
    }

    // Wraps an existing symbol table as a scope (used when binding without parsing a full source block).
    public Stack(Dictionary<string, VariableSymbol> variableSymbols)
    {
        VariableSymbols = variableSymbols;
    }
     public Stack(string sourcetext,Stack parent)
    {
        _sourcetext = sourcetext;
        Parent=parent;
    }



    public IEnumerable<string> Diagnostics => _diagnostics;
    public IReadOnlyList<Stack> Subblocks => _subblocks;

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
                        ParseBlock(blockText.ToString(), expressions);
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
        Binder binder = new(this);
        binder.Bind(expression);
        _diagnostics.AddRange(binder.Diagnostics);
        System.Console.WriteLine();
    }

    private void ParseBlock(string blockText, List<LiteralExpressionsyntax> expressions)
    {
        Stack subparser = new(blockText, this);
        _subblocks.Add(subparser);

        System.Console.WriteLine("{");
        System.Console.WriteLine();
        var nested = subparser.Parse();
        System.Console.WriteLine("}");
        System.Console.WriteLine();

        _diagnostics.AddRange(subparser.Diagnostics);
        foreach (var item in nested)
        {
            expressions.Add(item);
        }
    }

    

    private sealed record ParseUnit(string Text, int Line);

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
