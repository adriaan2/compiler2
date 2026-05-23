
namespace syntaxer;

public sealed class closingsyntax : LiteralExpressionsyntax
{
    public closingsyntax(Syntaxtoken semicolonToken)
    {
        SemicolonToken = semicolonToken;
    }

    public Syntaxtoken SemicolonToken { get; }

    public override SyntaxKind Kind => SyntaxKind.semicolonToken;

    public override IEnumerable<SyntaxNode> getchildren()
    {
        yield return SemicolonToken;
    }
}
