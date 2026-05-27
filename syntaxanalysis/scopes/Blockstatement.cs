namespace syntaxer.scopes;

using syntaxer;
public abstract class BlockStatementSyntax:SyntaxNode
{
    public BlockStatementSyntax(Syntaxtoken openBrace, IReadOnlyList<LiteralExpressionsyntax> statements, Syntaxtoken closeBrace)
    {
        OpenBrace = openBrace;
        Statements = statements;
        CloseBrace = closeBrace;
    }

    public Syntaxtoken OpenBrace { get; }
    public IReadOnlyList<LiteralExpressionsyntax> Statements { get; }
    public Syntaxtoken CloseBrace { get; }

    public override  SyntaxKind Kind => SyntaxKind.opencurly;


    public  SyntaxKind syntaxKind =>SyntaxKind.closedcurly;

    public override IEnumerable<SyntaxNode> getchildren()
    {
        throw new NotImplementedException();
    }
}
