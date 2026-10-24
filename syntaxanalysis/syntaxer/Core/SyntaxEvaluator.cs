
namespace syntaxer;

public class SyntaxEvaluator
{
    readonly LiteralExpressionsyntax _root;
    public SyntaxEvaluator(LiteralExpressionsyntax root)
    {
        _root=root;
    }
    public int Evaluate()
    {
        return evaluateexpressionroot(_root);
    }
     int evaluateexpressionroot(LiteralExpressionsyntax expression)
    {
        if (expression is numberSyntax n)
        {
#pragma warning disable CS8605 // Unboxing a possibly null value.
            return (int) n.Token.Value;
#pragma warning restore CS8605 // Unboxing a possibly null value.
        }

        if (expression is BynarySyntax b)
        {
            var left = evaluateexpressionroot(b.Left);
            var right = evaluateexpressionroot(b.Right);

            switch (b.OperatorToken.Kind)
            {
                case GuidanceSignalKind.plusToken:
                    return left + right;
                case GuidanceSignalKind.minusToken:
                    return left - right;
                case GuidanceSignalKind.timestoken:
                    return left * right;
                case GuidanceSignalKind.slashtoken:
                    return left / right;
                default:
                    throw new Exception($"Unexpected binary operator {b.OperatorToken.Kind}");
            }
        }

        if (expression is UnarySyntax u)
        {
            var operand = evaluateexpressionroot(u.Operand);

            switch (u.OperatorToken.Kind)
            {
                case GuidanceSignalKind.plusToken:
                    return operand;
                case GuidanceSignalKind.minusToken:
                    return -operand;
                default:
                    throw new Exception($"Unexpected unary operator {u.OperatorToken.Kind}");
            }
        }

        if (expression is Parenthessese p)
        {
            return evaluateexpressionroot(p.Expression);
        }

        if (expression is Charsyntax character)
        {
            return character.CharacterValue.Value is char value ? value : '\0';
        }

        throw new Exception($"Unexpected node {expression.Kind}");
    }

}

