  namespace syntaxer.scopes;

public sealed class forLogic : Stack
{
        public forLogic(string sourcetext)
                : base(sourcetext)
        {
                
        }

        public forLogic(string sourcetext, Stack parent)
                : base(sourcetext, parent)
        {
                
        }
}
