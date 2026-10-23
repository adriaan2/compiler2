
namespace syntaxer;

public sealed class closingsyntax : LiteralExpressionsyntax
{
    public closingsyntax(Syntaxtoken semicolonToken)
    {
        SemicolonToken = semicolonToken;
    }

    public Syntaxtoken SemicolonToken { get; }

    public override GuidanceSignalKind Kind => GuidanceSignalKind.semicolonToken;

    public override IEnumerable<GuidanceSignalNode> getchildren()
    {
        yield return SemicolonToken;
    }
}
