using System.Threading.Channels;

namespace syntaxer;


sealed class Parenthessese:LiteralExpressionsyntax
{
    public Parenthessese(Syntaxtoken openparenthesis, LiteralExpressionsyntax expression, Syntaxtoken closeparenthesis )
    {
        Openparenthesis=openparenthesis;
        Closeparenthesis=closeparenthesis;
        Expression=expression;
    }

    public Syntaxtoken Openparenthesis { get;  }
    public Syntaxtoken Closeparenthesis { get; }
    public LiteralExpressionsyntax Expression { get; }

    public override GuidanceSignalKind Kind => GuidanceSignalKind.bracketexpression;

    public override IEnumerable<GuidanceSignalNode> getchildren()
    {
        yield return Openparenthesis;
        yield return Expression;
        yield return Closeparenthesis;
    }
}

