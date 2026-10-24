using System.ComponentModel;
using System.Transactions;
using syntaxer.scopes;
using bindings;

namespace syntaxer;

public class Parser
{
        private Syntaxtoken Current => Peek(0);

        static bool subparsing = false;
        List<string> _diagnostics = new();
        public IEnumerable<string> Diagnostics => _diagnostics;
        int _position = 0;
        public int Position => _position;
        readonly Syntaxtoken[] _tokens;
        public ScopeParser parentlogic;

        public Parser(string Text)
        {

                Lexer lexer = new Lexer(Text);
                Syntaxtoken syntaxtoken;
                List<Syntaxtoken> tokens = new();

                do
                {
                        syntaxtoken = lexer.Lex();
                        if (syntaxtoken.Kind != GuidanceSignalKind.whitespaceToken && syntaxtoken.Kind != GuidanceSignalKind.badtoken)
                        {
                                tokens.Add(syntaxtoken);

                        }
                } while (syntaxtoken.Kind != GuidanceSignalKind.endoffiletoken);

                _diagnostics.AddRange(lexer.Diagnostics);
                _tokens = tokens.ToArray();
        }

        private Syntaxtoken Peek(int offset)
        {
                // do not use this method with an offsest value higher than 1
                var index = _position + offset;
                if (index >= _tokens.Length)
                        return _tokens[_tokens.Length - 1];

                return _tokens[index];
        }

        private Syntaxtoken Nexttoken()
        {
                var current = Current;
                _position++;
                return current;
        }


        private Syntaxtoken match(GuidanceSignalKind kind, string printable = "")
        {
                if (Current.Kind == kind)
                        return Nexttoken();

                _diagnostics.Add($"Error unexpected token in parser {Current.Kind}");
                return new Syntaxtoken(kind, Current.POsition, null, null);
        }






        public LiteralExpressionsyntax parse()
        {




                return ParseExpression();
        }

        private LiteralExpressionsyntax ParseExpression()
        {


                if (IsTypeKeyword(Current.Kind))
                        return ParseVariableDeclaration();

                if (Current.Kind == GuidanceSignalKind.identifierToken && Peek(1).Kind == GuidanceSignalKind.equalsToken)
                        return ParseAssignmentExpression();
                if (Current.Kind == GuidanceSignalKind.iftkeyword)
                        return ParseIfStatement();
                if (Current.Kind == GuidanceSignalKind.whileToken || Current.Kind == GuidanceSignalKind.forToken)
                        return ParseLoopStatement();
                if (Current.Kind == GuidanceSignalKind.returnToken)
                        return ParseReturnStatement();
                return ParseBinaryExpression();
        }

        private LiteralExpressionsyntax ParseIfStatement()
        {
                var ifToken = match(GuidanceSignalKind.iftkeyword, "if");
                var condition = ParseBinaryExpression();
                // The lexer represents the closing ';' as the end-of-file token, so
                // that is what closes the statement here.
                var semicolon = match(GuidanceSignalKind.endoffiletoken, "semicolon");
                return new Ifsyntax(ifToken, condition, semicolon);
        }

        private LiteralExpressionsyntax ParseLoopStatement()
        {
                var keyword = Nexttoken();
                var hasParentheses = Current.Kind == GuidanceSignalKind.openparen;
                if (hasParentheses)
                        Nexttoken();
                var condition = ParseBinaryExpression();
                if (hasParentheses)
                        match(GuidanceSignalKind.closedparen, "')'");
                match(GuidanceSignalKind.endoffiletoken, "semicolon");
                return new LoopSyntax(keyword, condition);
        }

        private LiteralExpressionsyntax ParseReturnStatement()
        {
                var keyword = Nexttoken();
                LiteralExpressionsyntax? expression = null;
                if (Current.Kind != GuidanceSignalKind.endoffiletoken)
                        expression = ParseBinaryExpression();
                match(GuidanceSignalKind.endoffiletoken, "semicolon");
                return new ReturnSyntax(keyword, expression);
        }

        private LiteralExpressionsyntax ParseVariableDeclaration()
        {
                var typeClause = ParseTypeClause();
                var identifier = match(GuidanceSignalKind.identifierToken, "identifier");
                var equals = match(GuidanceSignalKind.equalsToken, "equal");
                var initializer = ParseBinaryExpression();
                return new VariableDeclarationSyntax(typeClause, identifier, equals, initializer);
        }

        private TypeClauseSyntax ParseTypeClause()
        {
                var keyword = Nexttoken();
                Syntaxtoken? openBracket = null;
                Syntaxtoken? closeBracket = null;

                if (Current.Kind == GuidanceSignalKind.openbracket && Peek(1).Kind == GuidanceSignalKind.closebracket)
                {
                        openBracket = Nexttoken();
                        closeBracket = Nexttoken();
                }

                return new TypeClauseSyntax(keyword, openBracket, closeBracket);
        }

        private static bool IsTypeKeyword(GuidanceSignalKind kind)
        {
                return kind == GuidanceSignalKind.intKeyword || kind == GuidanceSignalKind.boolKeyword || GuidanceSignalKind.charkeyword == kind;
        }

        private LiteralExpressionsyntax ParseAssignmentExpression()
        {
                var identifier = match(GuidanceSignalKind.identifierToken);
                var equals = match(GuidanceSignalKind.equalsToken, "equals");
                var expression = ParseBinaryExpression();
                return new AssignmentExpressionSyntax(identifier, equals, expression);
        }

        private LiteralExpressionsyntax ParseBinaryExpression(int parentPrecedence = 0)
        {
                LiteralExpressionsyntax left;

                var unaryOperatorPrecedence = Precedence.GetUnaryOperatorPrecedence(Current.Kind);
                if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence > parentPrecedence)
                {
                        var operatorToken = Nexttoken();
                        var operand = ParseBinaryExpression(unaryOperatorPrecedence);
                        left = new UnarySyntax(operatorToken, operand);
                }
                else
                {
                        left = ParsePostfixExpression();
                }

                while (true)
                {
                        var precedence = Precedence.GetBinaryOperatorPrecedence(Current.Kind);
                        if (precedence == 0 || precedence <= parentPrecedence)
                                break;

                        var operatorToken = Nexttoken();
                        var right = ParseBinaryExpression(precedence);
                        left = new BynarySyntax(left, operatorToken, right);
                }

                return left;
        }

        private LiteralExpressionsyntax ParsePostfixExpression()
        {
                var left = ParsePrimaryexpression();

                while (Current.Kind == GuidanceSignalKind.openbracket)
                {
                        var openBracket = Nexttoken();
                        var index = ParseExpression();
                        var closeBracket = match(GuidanceSignalKind.closebracket, "close bracket");
                        left = new ArrayIndexExpressionSyntax(left, openBracket, index, closeBracket);
                }

                if (left is NameExpressionSyntax name &&
                    Current.Kind == GuidanceSignalKind.openparen)
                {
                        Nexttoken();
                        var arguments = new List<LiteralExpressionsyntax>();
                        if (Current.Kind != GuidanceSignalKind.closedparen)
                        {
                                do
                                {
                                        arguments.Add(ParseBinaryExpression());
                                        if (Current.Kind != GuidanceSignalKind.commaToken)
                                                break;
                                        Nexttoken();
                                } while (true);
                        }
                        match(GuidanceSignalKind.closedparen, "')'");
                        left = new FunctionCallSyntax(name.IdentifierToken, arguments);
                }

                if (left is NameExpressionSyntax incrementName &&
                    (Current.Kind == GuidanceSignalKind.incrementToken || Current.Kind == GuidanceSignalKind.decrementToken))
                {
                        var op = Nexttoken();
                        var arithmeticToken = new Syntaxtoken(
                                op.Kind == GuidanceSignalKind.incrementToken ? GuidanceSignalKind.plusToken : GuidanceSignalKind.minusToken,
                                op.POsition, op.Text, null);
                        var one = new numberSyntax(new Syntaxtoken(GuidanceSignalKind.numberToken, op.POsition, "1", 1));
                        var updated = new BynarySyntax(new NameExpressionSyntax(incrementName.IdentifierToken), arithmeticToken, one);
                        var equals = new Syntaxtoken(GuidanceSignalKind.equalsToken, op.POsition, "=", null);
                        left = new AssignmentExpressionSyntax(incrementName.IdentifierToken, equals, updated);
                }

                return left;
        }

        private LiteralExpressionsyntax ParsePrimaryexpression()
        {
                if (Current.Kind == GuidanceSignalKind.openbracket)
                {
                        return ParseArrayExpression();
                }
                if (Current.Kind == GuidanceSignalKind.semicolonToken)
                {
                        return new closingsyntax(Nexttoken());
                }

                if (Current.Kind == GuidanceSignalKind.openparen)
                {
                        var left = Nexttoken();
                        var expression = ParseBinaryExpression();
                        var right = match(GuidanceSignalKind.closedparen, "127");
                        return new Parenthessese(left, expression, right);
                }

                if (Current.Kind == GuidanceSignalKind.trueKeyword || Current.Kind == GuidanceSignalKind.falseKeyword)
                {
                        var keywordToken = Nexttoken();
                        return new BooleanSyntax(keywordToken);
                }

                if (Current.Kind == GuidanceSignalKind.identifierToken)
                {
                        var identifierToken = Nexttoken();
                        return new NameExpressionSyntax(identifierToken);
                }
                if (Current.Kind == GuidanceSignalKind.pipePipeToken)
                {
                        // System.Console.WriteLine("pipePipetoken");
                        // var identifierToken=Nexttoken();
                }

                if (Current.Kind == GuidanceSignalKind.charvaltoken)
                {
                        var charToken = Nexttoken();
                        var openQuote = new Syntaxtoken(GuidanceSignalKind.badtoken, charToken.POsition, "'", null);
                        var characterText = charToken.Value?.ToString();
                        var characterValue = new Syntaxtoken(
                                GuidanceSignalKind.charvaltoken,
                                charToken.POsition + 1,
                                characterText,
                                charToken.Value);
                        var closeQuote = new Syntaxtoken(
                                GuidanceSignalKind.badtoken,
                                charToken.POsition + (charToken.Text?.Length ?? 0) - 1,
                                "'",
                                null);

                        return new Charsyntax(openQuote, characterValue, closeQuote);
                }

                var numberToken = match(GuidanceSignalKind.numberToken, "141");
                return new numberSyntax(numberToken);
        }

        private LiteralExpressionsyntax ParseArrayExpression()
        {
                var openBracket = match(GuidanceSignalKind.openbracket, "open bracket");
                var elements = new List<LiteralExpressionsyntax>();

                if (Current.Kind != GuidanceSignalKind.closebracket)
                {
                        while (true)
                        {
                                elements.Add(ParseExpression());

                                if (Current.Kind != GuidanceSignalKind.commaToken)
                                        break;

                                Nexttoken();
                        }
                }

                var closeBracket = match(GuidanceSignalKind.closebracket, "close bracket");
                return new ArrayExpressionSyntax(openBracket, elements, closeBracket);
        }




}
