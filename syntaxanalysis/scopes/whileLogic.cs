namespace syntaxer.scopes;

public sealed class whileLogic : ScopeParser
{
        public whileLogic(string sourcetext)
                : base(sourcetext)
        {
        }

        public whileLogic(string sourcetext, ScopeParser parent)
                : base(sourcetext, parent)
        {
        }
}
