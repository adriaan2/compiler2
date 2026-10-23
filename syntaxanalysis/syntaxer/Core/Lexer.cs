


//using Microsoft.CodeAnalysis.Text;
namespace syntaxer;


public class Lexer
{
        private readonly string _text;
        
        private int _position;
        private readonly List<string> _diagnostics = new();
        public IEnumerable<string> Diagnostics => _diagnostics;

        public Lexer(string text)
        {
                _text = text;
        }

        private char Current
        {
                get
                {
                        if (_position == _text.Length)
                        {
                                return '\0';
                        }

                        return _text[_position];
                }
        }

        private char Lookahead
        {
                get
                {
                        var index = _position + 1;
                        if (index >= _text.Length)
                                return '\0';

                        return _text[index];
                }
        }

        private void Next()
        {
                _position++;
                //  System.Console.WriteLine(_position);
        }

        public Syntaxtoken Lex()

        {
                if (_position >= _text.Length)
                {
                       
                        return new Syntaxtoken(GuidanceSignalKind.endoffiletoken, _position, "\0", null);
                }

                if (char.IsLetter(Current))
                {
                        var start = _position;
                        while (char.IsLetter(Current))
                        {
                                Next();
                        }

                        var length = _position - start;
                        var text = _text.Substring(start, length);

                        var kind = text switch
                        {
                                "char" => GuidanceSignalKind.charkeyword,
                                "true" => GuidanceSignalKind.trueKeyword,
                                "false" => GuidanceSignalKind.falseKeyword,
                                "int" => GuidanceSignalKind.intKeyword,
                                "bool" => GuidanceSignalKind.boolKeyword,
                                "if" => GuidanceSignalKind.ifToken,
                                "else" => GuidanceSignalKind.elseToken,
                                "for" => GuidanceSignalKind.forToken,
                                "while" => GuidanceSignalKind.whileToken,
                                _ => GuidanceSignalKind.identifierToken
                        };

                        object? value = kind switch
                        {
                                GuidanceSignalKind.trueKeyword => true,
                                GuidanceSignalKind.falseKeyword => false,
                                _ => null
                        };
                        

                        return new Syntaxtoken(kind, start, text, value);
                }

                if (char.IsDigit(Current))
                {
                        //System.Console.WriteLine("digit");
                        var start = _position;
                        while (char.IsDigit(Current))
                        {
                                Next();
                        }

                        var length = _position - start;
                        var text = _text.Substring(start, length);
                        if (!int.TryParse(text, out var value))
                        {
                                _diagnostics.Add($"Invalid number literal '{text}'.");
                        }

                        return new Syntaxtoken(GuidanceSignalKind.numberToken, start, text, value);
                }

                if (char.IsWhiteSpace(Current))
                {
                        var start = _position;
                        while (char.IsWhiteSpace(Current))
                        {
                                Next();
                        }

                        var length = _position - start;
                        var text = _text.Substring(start, length);
                        return new Syntaxtoken(GuidanceSignalKind.whitespaceToken, start, text, null);
                }
                 if (Current == '{')
                {
                        return new Syntaxtoken(GuidanceSignalKind.opencurly, _position++, "{", null);
                }
                 if (Current == '}')
                        {
                                return new Syntaxtoken(GuidanceSignalKind.closedcurly, _position++, "}", null);
                        }

                if (Current == '+')
                {
                        return new Syntaxtoken(GuidanceSignalKind.plusToken, _position++, "+", null);
                }
                else if (Current == '[')
                {
                        return new Syntaxtoken(GuidanceSignalKind.openbracket, _position++, "[", null);
                }
                else if (Current == ']')
                {
                        return new Syntaxtoken(GuidanceSignalKind.closebracket, _position++, "]", null);
                }
                else if (Current == ',')
                {
                        return new Syntaxtoken(GuidanceSignalKind.commaToken, _position++, ",", null);
                }
                else if (Current == '=' && Lookahead != '=')
                {
                        return new Syntaxtoken(GuidanceSignalKind.equalsToken, _position++, "=", null);
                }
                else if (Current == '=' && Lookahead == '=')
                {
                        var position = _position;
                        _position += 2;
                        return new Syntaxtoken(GuidanceSignalKind.equalsEqualsToken, position, "==", null);
                }
                else if (Current == '-')
                {
                        return new Syntaxtoken(GuidanceSignalKind.minusToken, _position++, "-", null);
                }
                else if (Current == '/')
                {
                        return new Syntaxtoken(GuidanceSignalKind.slashtoken, _position++, "/", null);
                }
                else if (Current == '(')
                {
                        return new Syntaxtoken(GuidanceSignalKind.openparen, _position++, "(", null);
                }
                else if (Current == ')')
                {
                        return new Syntaxtoken(GuidanceSignalKind.closedparen, _position++, ")", null);
                }
                else if (Current == '*')
                {
                        return new Syntaxtoken(GuidanceSignalKind.timestoken, _position++, "*", null);
                }
                else if (Current == '!')
                {
                        return new Syntaxtoken(GuidanceSignalKind.bangToken, _position++, "!", null);
                }
                else if (Current == '&' && Lookahead == '&')
                {
                        var position = _position;
                        _position += 2;
                        return new Syntaxtoken(GuidanceSignalKind.ampersandAmpersandToken, position, "&&", null);
                }
                else if (Current == '|' && Lookahead == '|')
                {
                        var position = _position;
                        _position += 2;
                        return new Syntaxtoken(GuidanceSignalKind.pipePipeToken, position, "||", null);
                }
                else if (Current == '\'')
                {
                        var start = _position;
                        Next();

                        if (Current == '\0')
                        {
                                _diagnostics.Add("Unterminated char literal.");
                                return new Syntaxtoken(GuidanceSignalKind.badtoken, start, "'", null);
                        }

                        var charValue = Current;

                        Next();

                        if (Current != '\'')
                        {
                                _diagnostics.Add("Char literal must contain exactly one character.");
                                return new Syntaxtoken(GuidanceSignalKind.badtoken, start, _text.Substring(start, _position - start), null);
                        }

                        Next();

                        var text = _text.Substring(start, _position - start);

                        return new Syntaxtoken(GuidanceSignalKind.charvaltoken, _position, text, charValue);
                }

                if (Current == ';')
                {
                        
                        return new Syntaxtoken(GuidanceSignalKind.endoffiletoken, _position++, ";", null);
                }

                _diagnostics.Add($"error bad character input in lexer last line {Current}");
                return new Syntaxtoken(GuidanceSignalKind.badtoken, _position++, _text.Substring(_position - 1, 1), null);
        }
}
