using syntaxer;

namespace syntaxer;

// An if statement made of, in order: the `if` token, a boolean (condition)
// expression, and the closing `;` token.
public sealed class Ifsyntax : LiteralExpressionsyntax
{
    public Ifsyntax(Syntaxtoken ifToken, LiteralExpressionsyntax condition, Syntaxtoken semicolonToken)
    {
        IfToken = ifToken;
        Condition = condition;
        SemicolonToken = semicolonToken;
    }

    public Syntaxtoken IfToken { get; }
    public LiteralExpressionsyntax Condition { get; }
    public Syntaxtoken SemicolonToken { get; }

    public override SyntaxKind Kind => SyntaxKind.ifexpression;

    public override IEnumerable<SyntaxNode> getchildren()
    {
        yield return IfToken;
        yield return Condition;
        yield return SemicolonToken;
    }
}
