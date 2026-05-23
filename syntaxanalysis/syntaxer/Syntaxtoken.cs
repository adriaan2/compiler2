namespace syntaxer;

public class Syntaxtoken : SyntaxNode
{
    public Syntaxtoken(SyntaxKind kind, int position, string? text, object? value)
    {
        Kind = kind;
        POsition = position;
        Text = text;
        Value = value;
    }
    public override SyntaxKind Kind { get; }
    public int POsition { get; }
    public string? Text { get; }
    public object? Value { get; }

    public override IEnumerable<SyntaxNode> getchildren()
    {
        return Enumerable.Empty<SyntaxNode>();
    }
}
