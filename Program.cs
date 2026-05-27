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
        var syntax = compulatinglogic.Parse();

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
            object? result = null;

            foreach (var expression in syntax.Statements)
            {
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
                result = evaluator.Evaluate();
            }
            
        }
    }

}
