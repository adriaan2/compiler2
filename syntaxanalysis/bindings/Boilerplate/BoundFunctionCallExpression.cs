using syntaxer.scopes;

namespace bindings;

internal sealed class BoundFunctionCallExpression : Boundexpression
{
        public BoundFunctionCallExpression(FunctionDefinition function, Functions functions, Boundexpression[] arguments)
        {
                Function = function;
                Functions = functions;
                Arguments = arguments;
        }

        public FunctionDefinition Function { get; }
        public Functions Functions { get; }
        public Boundexpression[] Arguments { get; }
        public override Type Type => Function.ReturnType;
        internal override Boundnodekind Boundnodekind => Boundnodekind.FunctionCallExpression;
}
