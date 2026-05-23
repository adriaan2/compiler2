
namespace bindings{
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
        ifexpression
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
        LogicalAnd,
        LogicalOr
    }
    
    }
    
