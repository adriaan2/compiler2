namespace syntaxer.scopes;

public sealed class sentenceunits:LiteralExpressionsyntax
{
    public sentenceunits(IReadOnlyList<LiteralExpressionsyntax> statements)
    {
        Statements = statements;
    }

    public IReadOnlyList<LiteralExpressionsyntax> Statements { get; }

    public override GuidanceSignalKind Kind => throw new NotImplementedException();

    public override IEnumerable<GuidanceSignalNode> getchildren()
    {
        throw new NotImplementedException();
    }

}
