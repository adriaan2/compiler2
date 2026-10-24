namespace syntaxer;

public sealed class FunctionCallSyntax : LiteralExpressionsyntax
{
        public FunctionCallSyntax(Syntaxtoken identifier, IReadOnlyList<LiteralExpressionsyntax> arguments)
        {
                Identifier = identifier;
                Arguments = arguments;
        }

        public Syntaxtoken Identifier { get; }
        public IReadOnlyList<LiteralExpressionsyntax> Arguments { get; }
        public override GuidanceSignalKind Kind => GuidanceSignalKind.functioncallExpression;

        public override IEnumerable<GuidanceSignalNode> getchildren()
        {
                yield return Identifier;
                foreach (var argument in Arguments)
                        yield return argument;
        }
}
