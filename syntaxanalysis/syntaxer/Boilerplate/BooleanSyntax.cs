namespace syntaxer;

sealed class BooleanSyntax : LiteralExpressionsyntax
{
        public BooleanSyntax(Syntaxtoken keywordToken)
        {
                KeywordToken = keywordToken;
        }

        public Syntaxtoken KeywordToken { get; }
        public override GuidanceSignalKind Kind => GuidanceSignalKind.booleanexpression;

        public override IEnumerable<GuidanceSignalNode> getchildren()
        {
                yield return KeywordToken;
        }
}
