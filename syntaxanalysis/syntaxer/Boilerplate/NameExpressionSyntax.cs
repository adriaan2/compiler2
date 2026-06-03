namespace syntaxer;

sealed class NameExpressionSyntax : LiteralExpressionsyntax
{
    public NameExpressionSyntax(Syntaxtoken identifierToken)
    {
        IdentifierToken = identifierToken;
    }

    public Syntaxtoken IdentifierToken { get; }
    public override SyntaxKind Kind => SyntaxKind.nameexpression;

    public override IEnumerable<SyntaxNode> getchildren()
    {
        yield return IdentifierToken;
    }
}
