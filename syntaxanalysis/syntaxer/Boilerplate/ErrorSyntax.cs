namespace syntaxer;

sealed class ErrorSyntax : LiteralExpressionsyntax
{
    public ErrorSyntax(Syntaxtoken token)
    {
        Token = token;
    }

    public Syntaxtoken Token { get; }
    public override SyntaxKind Kind => SyntaxKind.errorexpression;

    public override IEnumerable<SyntaxNode> getchildren()
    {
        yield return Token;
    }
}
