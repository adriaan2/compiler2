using bindings;
using System.Collections.Generic;
using syntaxer;
using syntaxer.scopes;

public class Program
{
        public static void Main()
        {
                Console.ForegroundColor = ConsoleColor.White;
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

                var scopeParser = new ScopeParser(sourceText);
                scopeParser.Parse();

                if (scopeParser.Diagnostics.Any())
                {
                        Console.ForegroundColor = ConsoleColor.Red;
                        foreach (var item2 in scopeParser.Diagnostics)
                        {
                                System.Console.WriteLine(item2);
                        }
                }
        }

}
