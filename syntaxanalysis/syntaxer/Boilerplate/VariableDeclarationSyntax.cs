namespace syntaxer;

sealed class VariableDeclarationSyntax : LiteralExpressionsyntax
{
        public VariableDeclarationSyntax(TypeClauseSyntax typeClause, Syntaxtoken identifier, Syntaxtoken equalsToken, LiteralExpressionsyntax initializer)
        {
                TypeClause = typeClause;
                Identifier = identifier;
                EqualsToken = equalsToken;
                Initializer = initializer;
        }

        public TypeClauseSyntax TypeClause { get; }
        public Syntaxtoken Identifier { get; }
        public Syntaxtoken EqualsToken { get; }
        public LiteralExpressionsyntax Initializer { get; }

        public override GuidanceSignalKind Kind => GuidanceSignalKind.variabledeclarationexpression;

        public override IEnumerable<GuidanceSignalNode> getchildren()
        {
                yield return TypeClause;
                yield return Identifier;
                yield return EqualsToken;
                yield return Initializer;
        }
}
