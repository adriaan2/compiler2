
namespace syntaxer;

internal sealed class Charsyntax : LiteralExpressionsyntax
{
        public Charsyntax(Syntaxtoken openQuote, Syntaxtoken characterValue, Syntaxtoken closeQuote)
        {
                OpenQuote = openQuote;
                CharacterValue = characterValue;
                CloseQuote = closeQuote;
        }

        public Syntaxtoken OpenQuote { get; }
        public Syntaxtoken CharacterValue { get; }
        public Syntaxtoken CloseQuote { get; }

        public override GuidanceSignalKind Kind => GuidanceSignalKind.charvaltoken;

        public override IEnumerable<GuidanceSignalNode> getchildren()
        {
                yield return OpenQuote;
                yield return CharacterValue;
                yield return CloseQuote;
        }
}
