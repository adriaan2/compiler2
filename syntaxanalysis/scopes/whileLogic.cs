namespace syntaxer.scopes;

public sealed class whileLogic : Stack
{
        public whileLogic(string sourcetext)
                : base(sourcetext)
        {
        }

        public whileLogic(string sourcetext, Stack parent)
                : base(sourcetext, parent)
        {
        }
}
