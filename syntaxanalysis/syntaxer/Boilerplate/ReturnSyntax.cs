namespace syntaxer;

public sealed class ReturnSyntax : LiteralExpressionsyntax
{
        public ReturnSyntax(Syntaxtoken keyword, LiteralExpressionsyntax? expression)
        {
                Keyword = keyword;
                Expression = expression;
        }

        public Syntaxtoken Keyword { get; }
        public LiteralExpressionsyntax? Expression { get; }
        public override GuidanceSignalKind Kind => GuidanceSignalKind.returnexpression;

        public override IEnumerable<GuidanceSignalNode> getchildren()
        {
                yield return Keyword;
                if (Expression is not null)
                        yield return Expression;
        }
}
