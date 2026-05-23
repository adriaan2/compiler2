namespace syntaxer.scopes;

public sealed class sentenceunits
{
    public sentenceunits(IReadOnlyList<StatementSyntax> statements)
    {
        Statements = statements;
    }

    public IReadOnlyList<StatementSyntax> Statements { get; }
}
