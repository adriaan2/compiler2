
namespace bindings
{
        internal enum Boundnodekind
        {
                Expressionstatement,
                Blockstatement,
                Compilationunit,

                charexpression,
                Arrayexpression,
                Arrayindexexpression,
                Unaryexpression,
                Numberexpression,
                Binaryexpression,
                Booleanexpression,
                Variableexpression,
                Assignmentexpression,
                Variabledeclarationexpression,
                Semicolonexpression,
                ifexpression,
                FunctionCallExpression
        }
        internal enum Boundunaryoperatorkind
        {
                Identity,
                Negation,
                LogicalNegation
        }
        internal enum BoundBinaryoperatorkind
        {
                addition,
                subtraction,
                times,
                division,
                Equals,
                LessThan,
                GreaterThan,
                LogicalAnd,
                LogicalOr
        }

}

