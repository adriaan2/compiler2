
namespace CodeAnalysis.Text
{
        public readonly struct TextSpan
        {
                public TextSpan(int start, int length)
                {
                        Start = start;
                        Length = length;
                }

                public int Start { get; }
                public int Length { get; }
                public int End => Start + Length;
        }
}

namespace codeanalysis
{
        public readonly struct TextSpan
        {
                public TextSpan(int start, int length)
                {
                        Start = start;
                        Length = length;
                }

                public int Start { get; }
                public int Length { get; }
                public int End => Start + Length;
        }
}

namespace codeanalysis.Text
{
        public readonly struct TextSpan
        {
                public TextSpan(int start, int length)
                {
                        Start = start;
                        Length = length;
                }

                public int Start { get; }
                public int Length { get; }
                public int End => Start + Length;
        }
}
