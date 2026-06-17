using Microsoft.VisualBasic;
using syntaxer;

public class curlysyntax : LiteralExpressionsyntax
{
    public override SyntaxKind Kind => SyntaxKind.opencurly;

    public override IEnumerable<SyntaxNode> getchildren()
    {
        throw new NotImplementedException();
    }
}