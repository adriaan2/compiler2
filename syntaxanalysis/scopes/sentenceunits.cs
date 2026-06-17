namespace syntaxer.scopes;

public sealed class sentenceunits:LiteralExpressionsyntax
{
    public sentenceunits(IReadOnlyList<LiteralExpressionsyntax> statements)
    {
        Statements = statements;
    }

    public IReadOnlyList<LiteralExpressionsyntax> Statements { get; }

    public override SyntaxKind Kind => throw new NotImplementedException();

    public override IEnumerable<SyntaxNode> getchildren()
    {
        throw new NotImplementedException();
    }

}
