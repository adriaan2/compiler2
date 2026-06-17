namespace syntaxer;

sealed class numberSyntax : LiteralExpressionsyntax
{
    public numberSyntax(Syntaxtoken token)
    {
        Token = token;
    }

    public override SyntaxKind Kind => SyntaxKind.numberexpression;
    public Syntaxtoken Token { get; }

    public override IEnumerable<SyntaxNode> getchildren()
    {
        yield return Token;
    }
}
