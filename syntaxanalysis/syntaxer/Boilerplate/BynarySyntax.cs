namespace syntaxer;

sealed class BynarySyntax : LiteralExpressionsyntax
{
    public BynarySyntax(LiteralExpressionsyntax left, Syntaxtoken operatortoken, LiteralExpressionsyntax right)
    {
        Left = left;
        OperatorToken = operatortoken;
        Right = right;
    }

    public override SyntaxKind Kind => SyntaxKind.binaryexpression;

    public LiteralExpressionsyntax Left { get; }
    public Syntaxtoken OperatorToken { get; }
    public LiteralExpressionsyntax Right { get; }

    public override IEnumerable<SyntaxNode> getchildren()
    {
        yield return Left;
        yield return OperatorToken;
        yield return Right;
    }
}
