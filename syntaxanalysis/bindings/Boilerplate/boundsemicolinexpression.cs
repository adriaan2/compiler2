namespace bindings;

internal sealed class Boundsemicolinexpression : Boundexpression
{
        public override Type Type => typeof(void);

        internal override Boundnodekind Boundnodekind => Boundnodekind.Semicolonexpression;
}
