using Microsoft.VisualBasic;
using syntaxer;

public class curlysyntax : LiteralExpressionsyntax
{
    public override GuidanceSignalKind Kind => GuidanceSignalKind.opencurly;

    public override IEnumerable<GuidanceSignalNode> getchildren()
    {
        throw new NotImplementedException();
    }
}