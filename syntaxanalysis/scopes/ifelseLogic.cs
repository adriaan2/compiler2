namespace syntaxer.scopes;

public sealed class ifelseLogic : ScopeParser
{
        public ifelseLogic(string logic, string sourcetext)
                : base(sourcetext)
        {

        }


        public ifelseLogic(string logic, string sourcetext, ScopeParser parent)
                : base(sourcetext, parent)
        {

        }
}
