namespace syntaxer;

sealed class ArrayIndexExpressionSyntax : LiteralExpressionsyntax
{
    public ArrayIndexExpressionSyntax(LiteralExpressionsyntax target, Syntaxtoken openBracketToken, LiteralExpressionsyntax index, Syntaxtoken closeBracketToken)
    {
        Target = target;
        OpenBracketToken = openBracketToken;
        Index = index;
        CloseBracketToken = closeBracketToken;
    }

    public LiteralExpressionsyntax Target { get; }
    public Syntaxtoken OpenBracketToken { get; }
    public LiteralExpressionsyntax Index { get; }
    public Syntaxtoken CloseBracketToken { get; }

    public override GuidanceSignalKind Kind => GuidanceSignalKind.arrayindexexpression;

    public override IEnumerable<GuidanceSignalNode> getchildren()
    {
        yield return Target;
        yield return OpenBracketToken;
        yield return Index;
        yield return CloseBracketToken;
    }
}
