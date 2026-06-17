namespace syntaxer;

sealed class BooleanSyntax : LiteralExpressionsyntax
{
    public BooleanSyntax(Syntaxtoken keywordToken)
    {
        KeywordToken = keywordToken;
    }

    public Syntaxtoken KeywordToken { get; }
    public override SyntaxKind Kind => SyntaxKind.booleanexpression;

    public override IEnumerable<SyntaxNode> getchildren()
    {
        yield return KeywordToken;
    }
}
