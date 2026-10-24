namespace syntaxer;

public class Syntaxtoken : GuidanceSignalNode
{
        public Syntaxtoken(GuidanceSignalKind kind, int position, string? text, object? value)
        {
                Kind = kind;
                POsition = position;
                Text = text;
                Value = value;
        }
        public override GuidanceSignalKind Kind { get; }
        public int POsition { get; }
        public string? Text { get; }
        public object? Value { get; }

        public override IEnumerable<GuidanceSignalNode> getchildren()
        {
                return Enumerable.Empty<GuidanceSignalNode>();
        }
}
