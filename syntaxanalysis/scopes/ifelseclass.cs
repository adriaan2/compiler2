namespace syntaxer.scopes;

using syntaxer;

public sealed class IfStatementSyntax : StatementSyntax
{
    public IfStatementSyntax(
        Syntaxtoken ifKeyword,
        Syntaxtoken openParen,
        LiteralExpressionsyntax condition,
        Syntaxtoken closeParen,
        StatementSyntax thenStatement,
        ElseClauseSyntax? elseClause)
    {
        IfKeyword = ifKeyword;
        OpenParen = openParen;
        Condition = condition;
        CloseParen = closeParen;
        ThenStatement = thenStatement;
        ElseClause = elseClause;
    }

    public Syntaxtoken IfKeyword { get; }
    public Syntaxtoken OpenParen { get; }
    public LiteralExpressionsyntax Condition { get; }
    public Syntaxtoken CloseParen { get; }
    public StatementSyntax ThenStatement { get; }
    public ElseClauseSyntax? ElseClause { get; }

    public override SyntaxKind Kind => SyntaxKind.ifexpression;

    public override IEnumerable<SyntaxNode> getchildren()
    {
        yield return IfKeyword;
        yield return OpenParen;
        yield return Condition;
        yield return CloseParen;
        yield return ThenStatement;

        if (ElseClause is not null)
            yield return ElseClause;
    }
}

public sealed class ElseClauseSyntax : SyntaxNode
{
    public ElseClauseSyntax(Syntaxtoken elseKeyword, StatementSyntax elseStatement)
    {
        ElseKeyword = elseKeyword;
        ElseStatement = elseStatement;
    }

    public Syntaxtoken ElseKeyword { get; }
    public StatementSyntax ElseStatement { get; }

    public override SyntaxKind Kind => SyntaxKind.elsekeyword;

    public override IEnumerable<SyntaxNode> getchildren()
    {
        yield return ElseKeyword;
        yield return ElseStatement;
    }
}
