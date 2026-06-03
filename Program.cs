using bindings;
using System.Collections.Generic;
using syntaxer;
using syntaxer.scopes;

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

        var compulatinglogic = new compulatinglogic(sourceText);
        var expressions = compulatinglogic.Parse();

        if (compulatinglogic.Diagnostics.Any())
        {
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (var item2 in compulatinglogic.Diagnostics)
            {
                System.Console.WriteLine(item2);
            }
        }
        else
        {
            var binder = new Binder(variableSymbols);
            var boundExpressions = new List<Boundexpression>();

            foreach (var expression in expressions)
            {
                var boundExpression = binder.Bind(expression);
                boundExpressions.Add(boundExpression);
            }

            if (binder.Diagnostics.Any())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                foreach (var item3 in binder.Diagnostics)
                {
                    System.Console.WriteLine(item3);
                }

                return;
            }

            object ? result = null;
            foreach (var boundExpression in boundExpressions)
            {
                BoundEvaluator evaluator = new(boundExpression, variables);
                result = evaluator.Evaluate();
                System.Console.WriteLine($"{result}");
            }
            
        }
    }

}
