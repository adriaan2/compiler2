namespace syntaxer;

sealed class NameExpressionSyntax : LiteralExpressionsyntax
{
    public NameExpressionSyntax(Syntaxtoken identifierToken)
    {
        IdentifierToken = identifierToken;
    }

    public Syntaxtoken IdentifierToken { get; }
    public override GuidanceSignalKind Kind => GuidanceSignalKind.nameexpression;

    public override IEnumerable<GuidanceSignalNode> getchildren()
    {
        yield return IdentifierToken;
    }
}
