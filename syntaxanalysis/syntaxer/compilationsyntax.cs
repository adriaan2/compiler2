using System.Drawing;

namespace syntaxer;

public sealed class CompilationUnitSyntax : SyntaxNode
{
           // Dictionary<> variables = new Dictionary<VariableSymbol, object>();

    public CompilationUnitSyntax(List<LiteralExpressionsyntax> statements, Syntaxtoken eof)
    {
        Statements = statements;
        EndOfFileToken = eof;
    }

   


    public List<LiteralExpressionsyntax> Statements { get; }
    public Syntaxtoken EndOfFileToken { get; }

    public override SyntaxKind Kind => SyntaxKind.compilationunit;

    public List<SyntaxNode> Statements1 { get; }
    public Syntaxtoken Eof { get; }
    


    public override IEnumerable<SyntaxNode> getchildren()
    {
        foreach (var statement in Statements)
            yield return statement;

        yield return EndOfFileToken;
    }
}

