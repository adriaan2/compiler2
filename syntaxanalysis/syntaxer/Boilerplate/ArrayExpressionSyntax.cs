namespace syntaxer;

sealed class ArrayExpressionSyntax : LiteralExpressionsyntax
{
    public ArrayExpressionSyntax(Syntaxtoken openBracketToken, IReadOnlyList<LiteralExpressionsyntax> elements, Syntaxtoken closeBracketToken)
    {
        OpenBracketToken = openBracketToken;
        Elements = elements;
        CloseBracketToken = closeBracketToken;
    }

    public Syntaxtoken OpenBracketToken { get; }
    public IReadOnlyList<LiteralExpressionsyntax> Elements { get; }
    public Syntaxtoken CloseBracketToken { get; }

    public override GuidanceSignalKind Kind => GuidanceSignalKind.arrayexpression;

    public override IEnumerable<GuidanceSignalNode> getchildren()
    {
        yield return OpenBracketToken;

        foreach (var element in Elements)
            yield return element;

        yield return CloseBracketToken;
    }
}
