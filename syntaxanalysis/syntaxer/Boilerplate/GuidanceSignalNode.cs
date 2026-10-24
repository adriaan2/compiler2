namespace syntaxer;

public abstract class GuidanceSignalNode
{
    public abstract GuidanceSignalKind Kind { get; }
    public abstract IEnumerable<GuidanceSignalNode> getchildren();
}
