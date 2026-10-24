namespace syntaxer;

sealed class UnarySyntax : LiteralExpressionsyntax
{
        public UnarySyntax(Syntaxtoken operatorToken, LiteralExpressionsyntax operand)
        {
                OperatorToken = operatorToken;
                Operand = operand;
        }

        public Syntaxtoken OperatorToken { get; }
        public LiteralExpressionsyntax Operand { get; }
        public override GuidanceSignalKind Kind => GuidanceSignalKind.unaryexpression;

        public override IEnumerable<GuidanceSignalNode> getchildren()
        {
                yield return OperatorToken;
                yield return Operand;
        }
}

