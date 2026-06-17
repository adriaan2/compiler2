using System;

namespace bindings
{
    // The bound form of an if-statement. Holds the bound boolean condition;
    // the block that follows is parsed (entered) only when the condition is true.
    internal sealed class BoundIfexpression : Boundexpression
    {
        public BoundIfexpression(Boundexpression condition)
        {
            Condition = condition;
        }

        public Boundexpression Condition { get; }

        public override Type Type => typeof(void);

        internal override Boundnodekind Boundnodekind => Boundnodekind.ifexpression;
    }
}
