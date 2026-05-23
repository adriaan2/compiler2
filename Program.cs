using bindings;
using System.Collections.Generic;
using syntaxer;

public  class Program
{
    public static void Main()
    {
        Console.ForegroundColor = ConsoleColor.White;
        var variables = new Dictionary<VariableSymbol, object>();
        var variableSymbols = new Dictionary<string, VariableSymbol>();
        var programPath = Path.Combine(Directory.GetCurrentDirectory(), "program.txt");

        if (!File.Exists(programPath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Missing source file: {programPath}");
            return;
        }

        var sourceText = File.ReadAllText(programPath);
        if (string.IsNullOrWhiteSpace(sourceText))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Source file is empty: {programPath}");
            return;
        }

        
                syntaxer.Parser parser = new(sourceText);

            var   expression = parser.parse();

          //  var expression=parser.ParseCompilationUnit().getchildren();
           
        PrettyPrint( (SyntaxNode)expression);        
        //var expression=parser.ParseCompilationUnit();
        
        
        if (parser.Diagnostics.Any())
        {
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (var item2 in parser.Diagnostics)
            {
                System.Console.WriteLine(item2);
            }
        }
        else
        {
            var binder = new Binder(variableSymbols);
          
            var boundExpression = binder.Bind(expression);

            if (binder.Diagnostics.Any())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                foreach (var item3 in binder.Diagnostics)
                {
                    System.Console.WriteLine(item3);
                }

                return;
            }

            BoundEvaluator evaluator = new(boundExpression, variables);
            var result = evaluator.Evaluate();
            System.Console.WriteLine(result);
        }
    }

    public static void PrettyPrint(SyntaxNode node, string indent = "", bool isLast = true)
    {
        var marker = indent == "" ? "" : (isLast ? "+--" : "+--");
        Console.WriteLine($"{indent}{marker}{GetNodeLabel(node)}");

        indent += indent == "" ? "    " : (isLast ? "    " : "¦   ");

        var children = node.getchildren().ToArray();
        for (int i = 0; i < children.Length; i++)
        {
            PrettyPrint(children[i], indent, i == children.Length - 1);
        }
    }

    private static string GetNodeLabel(SyntaxNode node)
    {
        return node.Kind switch
        {
            SyntaxKind.binaryexpression => "BinaryExpression",
            SyntaxKind.unaryexpression => "UnaryExpression",
            SyntaxKind.numberexpression => "NumberExpression",
            SyntaxKind.booleanexpression => "BooleanExpression",
            SyntaxKind.arrayexpression => "ArrayExpression",
            SyntaxKind.arrayindexexpression => "ArrayIndexExpression",
            SyntaxKind.typeclause => "TypeClause",
            SyntaxKind.nameexpression => "NameExpression",
            SyntaxKind.assignmentexpression => "AssignmentExpression",
            SyntaxKind.variabledeclarationexpression => "VariableDeclarationExpression",
            SyntaxKind.numberToken => $"NumberToken {(node as Syntaxtoken)?.Value}",
            SyntaxKind.openbracket => "OpenBracketToken",
            SyntaxKind.closebracket => "CloseBracketToken",
            SyntaxKind.commaToken => "CommaToken",
            SyntaxKind.identifierToken => $"IdentifierToken {(node as Syntaxtoken)?.Text}",
            SyntaxKind.trueKeyword => "TrueKeyword",
            SyntaxKind.falseKeyword => "FalseKeyword",
            SyntaxKind.intKeyword => "IntKeyword",
            SyntaxKind.boolKeyword => "BoolKeyword",
            SyntaxKind.charkeyword => "CharKeyword",
            SyntaxKind.equalsToken => "EqualsToken",
            SyntaxKind.equalsEqualsToken => "EqualsEqualsToken",
            SyntaxKind.plusToken => "PlusToken",
            SyntaxKind.minusToken => "MinusToken",
            SyntaxKind.timestoken => "StarToken",
            SyntaxKind.slashtoken => "SlashToken",
            SyntaxKind.bangToken => "BangToken",
            SyntaxKind.ampersandAmpersandToken => "AmpersandAmpersandToken",
            SyntaxKind.pipePipeToken => "PipePipeToken",
            _ => node.Kind.ToString()
        };
}}


