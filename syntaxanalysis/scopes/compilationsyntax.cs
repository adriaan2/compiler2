namespace syntaxer.scopes;

using syntaxer;

public class CompilationUnitSyntax : SyntaxNode
{
    public CompilationUnitSyntax(IReadOnlyList<LiteralExpressionsyntax> statements, Syntaxtoken eof)
    {
        Statements = statements;
        EndOfFileToken = eof;
    }

    public IReadOnlyList<LiteralExpressionsyntax> Statements { get; }
    public Syntaxtoken EndOfFileToken { get; }

    public override SyntaxKind Kind => SyntaxKind.compilationunit;

    public override IEnumerable<SyntaxNode> getchildren()
    {
        foreach (var statement in Statements)
            yield return statement;

        yield return EndOfFileToken;
    }
}

