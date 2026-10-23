namespace syntaxer;

sealed class AssignmentExpressionSyntax : LiteralExpressionsyntax
{
    public AssignmentExpressionSyntax(Syntaxtoken identifierToken, Syntaxtoken equalsToken, LiteralExpressionsyntax expression)
    {
        IdentifierToken = identifierToken;
        EqualsToken = equalsToken;
        Expression = expression;
    }

    public Syntaxtoken IdentifierToken { get; }
    public Syntaxtoken EqualsToken { get; }
    public LiteralExpressionsyntax Expression { get; }

    public override GuidanceSignalKind Kind => GuidanceSignalKind.assignmentexpression;

    public override IEnumerable<GuidanceSignalNode> getchildren()
    {
        yield return IdentifierToken;
        yield return EqualsToken;
        yield return Expression;
    }
}
