using syntaxer;

sealed class Ifsyntax:LiteralExpressionsyntax
{
    public override SyntaxKind Kind=>SyntaxKind.ifToken;

    public Parenthessese Paranthese { get; }
    public BooleanSyntax BooleanSyntaxp { get; }

    public Ifsyntax(Parenthessese parenthessese ,BooleanSyntax booleanSyntax)
    {
        Paranthese=parenthessese;
        BooleanSyntaxp=booleanSyntax;
    }
    
    public override IEnumerable<SyntaxNode> getchildren()
    {
        yield return Paranthese;
        yield return BooleanSyntaxp;
        
    }
}