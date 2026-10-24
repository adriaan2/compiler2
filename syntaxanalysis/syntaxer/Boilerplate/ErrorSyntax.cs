namespace syntaxer;

sealed class ErrorSyntax : LiteralExpressionsyntax
{
        public ErrorSyntax(Syntaxtoken token)
        {
                Token = token;
        }

        public Syntaxtoken Token { get; }
        public override GuidanceSignalKind Kind => GuidanceSignalKind.errorexpression;

        public override IEnumerable<GuidanceSignalNode> getchildren()
        {
                yield return Token;
        }
}
