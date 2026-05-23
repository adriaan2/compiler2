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

    public override SyntaxKind Kind => SyntaxKind.arrayindexexpression;

    public override IEnumerable<SyntaxNode> getchildren()
    {
        yield return Target;
        yield return OpenBracketToken;
        yield return Index;
        yield return CloseBracketToken;
    }
}
