namespace syntaxer.scopes;

using syntaxer;

public class CompilationUnitSyntax : GuidanceSignalNode
{
    public CompilationUnitSyntax(IReadOnlyList<LiteralExpressionsyntax> statements, Syntaxtoken eof)
    {
        Statements = statements;
        EndOfFileToken = eof;
    }

    public IReadOnlyList<LiteralExpressionsyntax> Statements { get; }
    public Syntaxtoken EndOfFileToken { get; }

    public override GuidanceSignalKind Kind => GuidanceSignalKind.compilationunit;

    public override IEnumerable<GuidanceSignalNode> getchildren()
    {
        foreach (var statement in Statements)
            yield return statement;

        yield return EndOfFileToken;
    }
}

