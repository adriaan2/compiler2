namespace syntaxer;

public sealed class LoopSyntax : LiteralExpressionsyntax
{
        public LoopSyntax(Syntaxtoken keyword, LiteralExpressionsyntax condition)
        {
                Keyword = keyword;
                Condition = condition;
        }

        public Syntaxtoken Keyword { get; }
        public LiteralExpressionsyntax Condition { get; }
        public bool IsFor => Keyword.Kind == GuidanceSignalKind.forToken;
        public override GuidanceSignalKind Kind => GuidanceSignalKind.loopexpression;

        public override IEnumerable<GuidanceSignalNode> getchildren()
        {
                yield return Keyword;
                yield return Condition;
        }
}
