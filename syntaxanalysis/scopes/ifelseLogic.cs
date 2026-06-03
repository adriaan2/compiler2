namespace syntaxer.scopes;

public sealed class ifelseLogic : compulatinglogic
{
        public ifelseLogic(string logic,string sourcetext)
                : base(sourcetext)
        {
                
        }
      

        public ifelseLogic(string logic, string sourcetext, compulatinglogic parent)
                : base(sourcetext, parent)
        {
                
        }
}
