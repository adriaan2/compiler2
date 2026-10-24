namespace syntaxer;

public static class Precedence
{
        public static int GetBinaryOperatorPrecedence(GuidanceSignalKind kind)
        {
                if (kind == GuidanceSignalKind.pipePipeToken)
                        return 1;
                if (kind == GuidanceSignalKind.ampersandAmpersandToken)
                        return 2;
                if (kind == GuidanceSignalKind.equalsEqualsToken || kind == GuidanceSignalKind.lessToken || kind == GuidanceSignalKind.greaterToken)
                        return 3;
                if (kind == GuidanceSignalKind.timestoken || kind == GuidanceSignalKind.slashtoken)
                        return 5;
                if (kind == GuidanceSignalKind.plusToken || kind == GuidanceSignalKind.minusToken)
                        return 4;
                return 0;
        }

        public static int GetUnaryOperatorPrecedence(GuidanceSignalKind kind)
        {
                if (kind == GuidanceSignalKind.plusToken || kind == GuidanceSignalKind.minusToken || kind == GuidanceSignalKind.bangToken)
                        return 6;
                return 0;
        }
}
