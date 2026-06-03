namespace syntaxer.scopes;

public sealed class whileLogic : compulatinglogic
{
        public whileLogic(string sourcetext)
                : base(sourcetext)
        {
        }

        public whileLogic(string sourcetext, compulatinglogic parent)
                : base(sourcetext, parent)
        {
        }
}
