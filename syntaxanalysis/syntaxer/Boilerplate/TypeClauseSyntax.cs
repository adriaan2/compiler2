namespace syntaxer;

sealed class TypeClauseSyntax : GuidanceSignalNode
{
        public TypeClauseSyntax(Syntaxtoken keyword, Syntaxtoken? openBracketToken = null, Syntaxtoken? closeBracketToken = null)
        {
                Keyword = keyword;
                OpenBracketToken = openBracketToken;
                CloseBracketToken = closeBracketToken;
        }

        public Syntaxtoken Keyword { get; }
        public Syntaxtoken? OpenBracketToken { get; }
        public Syntaxtoken? CloseBracketToken { get; }
        public bool IsArray => OpenBracketToken is not null && CloseBracketToken is not null;

        public override GuidanceSignalKind Kind => GuidanceSignalKind.typeclause;

        public override IEnumerable<GuidanceSignalNode> getchildren()
        {
                yield return Keyword;

                if (OpenBracketToken is not null)
                        yield return OpenBracketToken;

                if (CloseBracketToken is not null)
                        yield return CloseBracketToken;
        }
}
