  namespace syntaxer.scopes;

public sealed class forLogic : compulatinglogic
{
        public forLogic(string sourcetext)
                : base(sourcetext)
        {
                
        }

        public forLogic(string sourcetext, compulatinglogic parent)
                : base(sourcetext, parent)
        {
                
        }
}
