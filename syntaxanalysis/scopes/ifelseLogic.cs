namespace syntaxer.scopes;

public sealed class ifelseLogic : Stack
{
        public ifelseLogic(string logic,string sourcetext)
                : base(sourcetext)
        {
                
        }
      

        public ifelseLogic(string logic, string sourcetext, Stack parent)
                : base(sourcetext, parent)
        {
                
        }
}
