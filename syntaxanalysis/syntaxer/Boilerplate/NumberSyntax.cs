namespace syntaxer;

sealed class numberSyntax : LiteralExpressionsyntax
{
    public numberSyntax(Syntaxtoken token)
    {
        Token = token;
    }

    public override GuidanceSignalKind Kind => GuidanceSignalKind.numberexpression;
    public Syntaxtoken Token { get; }

    public override IEnumerable<GuidanceSignalNode> getchildren()
    {
        yield return Token;
    }
}
