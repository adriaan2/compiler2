using System.Collections.Generic;
using bindings;
using syntaxer;
using syntaxer.scopes;
using Xunit;
using Xunit.Sdk;

namespace test.Tests;

public class SmokeTests
{



        public static TheoryData<EvaluationCase> BooleanCases => new()
    {
        new("BooleanTrueLiteral", "true", true),
        new("BooleanFalseLiteral", "false", false),
        new("LogicalNot", "!false", true),
        new("LogicalAnd", "true && false", false),
        new("LogicalOr", "true || false", true),
        new("LogicalPrecedence", "!false && true", true),
        new("LogicalParentheses", "!(false || false)", true),
        new("IntegerEqualityTrue", "1 == 1", true),
        new("IntegerEqualityFalse", "1 == 2", false),
        new("ArithmeticEquality", "1 + 2 == 3", true),
        new("BooleanEqualityFalse", "true == false", false),
        new("BooleanEqualityWithUnary", "true == !false", true),
        new("EqualityBeforeLogicalAnd", "1 == 1 && true", true)
    };





        // [Fact]
        // public void ScopeParser_Parse_GroupsTextBetweenCurlyBraces()
        // {
        //     var logic = new ScopeParser("""
        //     {
        //         int d=0;


        //     }
        //     """);

        //     var expressions = logic.Parse();

        //     var block = Assert.IsType<BlockStatementSyntax>(Assert.Single(expressions));
        //     Assert.Single(block.Statements);
        //     Assert.Single(logic.Subblocks);
        //     Assert.Empty(logic.Diagnostics);
        // }

        // [Fact]
        // public void ScopeParser_Parse_ReportsUnclosedCurlyBraceAtEndOfText()
        // {
        //     var logic = new ScopeParser("""
        //     {
        //         1
        //     """);

        //     var expressions = logic.Parse();

        //     Assert.Empty(expressions);
        //     Assert.Contains(logic.Diagnostics, diagnostic => diagnostic.Contains("missing a closing curly brace"));
        // }






        [Theory]
        [MemberData(nameof(BooleanCases))]
        public void Evaluate_BooleanExpressions_ReturnExpectedResult(EvaluationCase testCase)
        {
                AssertEvaluationMatches(testCase);
        }

        [Fact]
        public void Evaluate_BoolDeclaration_ReturnsBooleanValue()
        {
                var result = EvaluateWithBinding("bool b = true");

                Assert.True(result is bool value && value);
        }

        [Fact]
        public void Evaluate_BoolVariable_PersistsAcrossInputs()
        {
                var variables = new Dictionary<VariableSymbol, object>();
                var variableSymbols = new Dictionary<string, VariableSymbol>();

                var declarationResult = EvaluateWithBinding("bool c = false", variableSymbols, variables);
                var readResult = EvaluateWithBinding("c", variableSymbols, variables);

                Assert.False((bool)declarationResult);
                Assert.False((bool)readResult);
        }

        [Fact]
        public void Evaluate_IntArrayDeclaration_ReturnsArrayValue()
        {
                var result = EvaluateWithBinding("int[] numbers = [1, 2, 3]");

                Assert.Equal("[1, 2, 3]", result.ToString());
        }

        [Fact]
        public void Evaluate_ArrayIndex_ReturnsExpectedElement()
        {
                var variables = new Dictionary<VariableSymbol, object>();
                var variableSymbols = new Dictionary<string, VariableSymbol>();

                _ = EvaluateWithBinding("int[] numbers = [4, 5, 6]", variableSymbols, variables);
                var result = EvaluateWithBinding("numbers[1]", variableSymbols, variables);

                Assert.Equal(5, result);
        }

        [Fact]
        public void Evaluate_CharArrayIndex_ReturnsExpectedElement()
        {
                var variables = new Dictionary<VariableSymbol, object>();
                var variableSymbols = new Dictionary<string, VariableSymbol>();

                _ = EvaluateWithBinding("char[] letters = ['a', 'b', 'c']", variableSymbols, variables);
                var result = EvaluateWithBinding("letters[2]", variableSymbols, variables);

                Assert.Equal('c', result);
        }

        [Fact]
        public void Bind_BoolDeclarationWithIntInitializer_ReportsDiagnostic()
        {
                var parser = new Parser("bool b = 5");
                var syntax = parser.parse();
                var binder = new Binder(new Dictionary<string, VariableSymbol>());

                _ = binder.Bind(syntax);

                if (parser.Diagnostics.Any())
                        throw new XunitException($"Expected no parser diagnostics, got: {string.Join(", ", parser.Diagnostics)}");

                Assert.Contains(binder.Diagnostics, diagnostic => diagnostic.Contains("must be of type Boolean"));
        }

        [Fact]
        public void Bind_MixedBooleanAndNumberExpression_ReportsDiagnostic()
        {
                var parser = new Parser("true + 1");
                var syntax = parser.parse();
                var binder = new Binder(new Dictionary<string, VariableSymbol>());

                _ = binder.Bind(syntax);

                if (parser.Diagnostics.Any())
                        throw new XunitException($"Expected no parser diagnostics for 'true + 1', but got: {string.Join(", ", parser.Diagnostics)}");

                if (!binder.Diagnostics.Any())
                        throw new XunitException("Expected a binder diagnostic for 'true + 1', but the binder reported none.");
        }

        [Fact]
        public void Bind_EqualityBetweenDifferentTypes_ReportsDiagnostic()
        {
                var parser = new Parser("1 == true");
                var syntax = parser.parse();
                var binder = new Binder(new Dictionary<string, VariableSymbol>());

                _ = binder.Bind(syntax);

                if (parser.Diagnostics.Any())
                        throw new XunitException($"Expected no parser diagnostics for '1 == true', but got: {string.Join(", ", parser.Diagnostics)}");

                if (!binder.Diagnostics.Any())
                        throw new XunitException("Expected a binder diagnostic for '1 == true', but the binder reported none.");
        }

        [Fact]
        public void Bind_ArrayLiteralWithMixedTypes_ReportsDiagnostic()
        {
                var parser = new Parser("[1, true]");
                var syntax = parser.parse();
                var binder = new Binder(new Dictionary<string, VariableSymbol>());

                _ = binder.Bind(syntax);

                if (parser.Diagnostics.Any())
                        throw new XunitException($"Expected no parser diagnostics for '[1, true]', but got: {string.Join(", ", parser.Diagnostics)}");

                Assert.Contains(binder.Diagnostics, diagnostic => diagnostic.Contains("same type"));
        }

        [Fact]
        public void Bind_ArrayDeclarationWithWrongElementType_ReportsDiagnostic()
        {
                var parser = new Parser("bool[] flags = [true, false, true]");
                var syntax = parser.parse();
                var binder = new Binder(new Dictionary<string, VariableSymbol>());

                _ = binder.Bind(syntax);

                if (parser.Diagnostics.Any())
                        throw new XunitException($"Expected no parser diagnostics, got: {string.Join(", ", parser.Diagnostics)}");

                Assert.DoesNotContain(binder.Diagnostics, diagnostic => diagnostic.Contains("must be of type"));

                var invalidParser = new Parser("bool[] flags = [1, 2]");
                var invalidSyntax = invalidParser.parse();
                var invalidBinder = new Binder(new Dictionary<string, VariableSymbol>());

                _ = invalidBinder.Bind(invalidSyntax);

                if (invalidParser.Diagnostics.Any())
                        throw new XunitException($"Expected no parser diagnostics, got: {string.Join(", ", invalidParser.Diagnostics)}");

                Assert.Contains(invalidBinder.Diagnostics, diagnostic => diagnostic.Contains("must be of type Boolean[]"));
        }

        [Fact]
        public void Bind_ArrayIndexWithNonIntegerExpression_ReportsDiagnostic()
        {
                var variables = new Dictionary<string, VariableSymbol>();
                var binder = new Binder(variables);

                _ = binder.Bind(new Parser("int[] numbers = [1, 2]").parse());

                var parser = new Parser("numbers[true]");
                var syntax = parser.parse();
                var bound = binder.Bind(syntax);

                Assert.NotNull(bound);
                Assert.Contains(binder.Diagnostics, diagnostic => diagnostic.Contains("Array indexes must be of type Int32"));
        }

        [Fact]
        public void Evaluate_ArrayIndexOutOfBounds_Throws()
        {
                var variables = new Dictionary<VariableSymbol, object>();
                var variableSymbols = new Dictionary<string, VariableSymbol>();

                _ = EvaluateWithBinding("int[] numbers = [1, 2]", variableSymbols, variables);

                var exception = Assert.Throws<Exception>(() => EvaluateWithBinding("numbers[5]", variableSymbols, variables));
                Assert.Contains("out of bounds", exception.Message);
        }

        [Fact]
        public void ScopeParser_ExecutesDeclarationsAcrossThreeNestedScopes()
        {
                var scope = new ScopeParser("""
            int q = 9;
            if (true) {
                int entered = 2;
                int d = entered + q;
                if (true) {
                    char s = 'e';
                    if (s == 'e') {
                        char inner = 'e';
                    }
                }
            }
            """);

                scope.Parse();

                Assert.Empty(scope.Diagnostics);
                var firstBlock = Assert.Single(scope.Subblocks);
                Assert.Contains("entered", firstBlock.VariableSymbols.Keys);
                var secondBlock = Assert.Single(firstBlock.Subblocks);
                Assert.Contains("s", secondBlock.VariableSymbols.Keys);
                var thirdBlock = Assert.Single(secondBlock.Subblocks);
                Assert.Contains("inner", thirdBlock.VariableSymbols.Keys);
        }



        [Fact]
        public void ScopeParser_ThrowsWhenVariableIsRedeclaredInNestedScope()
        {
                var scope = new ScopeParser("""
            int value = 1;
            if (true) {
                int value = 2;
            }
            """);

                var exception = Assert.Throws<InvalidOperationException>(() => scope.Parse());

                Assert.Contains("value", exception.Message);
                Assert.Contains("enclosing scope", exception.Message);
        }

        [Fact]
        public void ScopeParser_AllowsSameVariableNameInSiblingScopes()
        {
                var scope = new ScopeParser("""
            if (true) {
                int value = 1;
            }
            if (true) {
                int value = 2;
            }
            """);

                scope.Parse();

                Assert.Empty(scope.Diagnostics);
                Assert.Equal(2, scope.Subblocks.Count);
                Assert.All(scope.Subblocks, child => Assert.Contains("value", child.VariableSymbols.Keys));
        }

        [Fact]
        public void Functions_RegistersAllRequestedReturnTypes()
        {
                var functions = new Functions();

                functions.Add(new FunctionDefinition("integerResult", typeof(int), null, Array.Empty<FunctionParameter>(), "return 1;"));
                functions.Add(new FunctionDefinition("characterResult", typeof(char), null, Array.Empty<FunctionParameter>(), "return 'a';"));
                functions.Add(new FunctionDefinition("integerArrayResult", typeof(ArrayValue), typeof(int), Array.Empty<FunctionParameter>(), "return [1];"));
                functions.Add(new FunctionDefinition("characterArrayResult", typeof(ArrayValue), typeof(char), Array.Empty<FunctionParameter>(), "return ['a'];"));
                functions.Add(new FunctionDefinition("noResult", typeof(void), null, Array.Empty<FunctionParameter>(), "return;"));

                Assert.Equal(5, functions.Definitions.Count);
                Assert.Equal(typeof(int), functions.Definitions["integerResult"].ReturnType);
                Assert.Equal(typeof(char), functions.Definitions["characterResult"].ReturnType);
                Assert.Equal(typeof(int), functions.Definitions["integerArrayResult"].ReturnElementType);
                Assert.Equal(typeof(char), functions.Definitions["characterArrayResult"].ReturnElementType);
                Assert.Equal(typeof(void), functions.Definitions["noResult"].ReturnType);
        }

        [Fact]
        public void Functions_RejectsDuplicateNamesAndUnsupportedReturnTypes()
        {
                var functions = new Functions();
                var definition = new FunctionDefinition("work", typeof(int), null, Array.Empty<FunctionParameter>(), "return 1;");
                functions.Add(definition);

                var duplicate = Assert.Throws<InvalidOperationException>(() => functions.Add(definition));
                Assert.Contains("already declared", duplicate.Message);

                var unsupportedType = Assert.Throws<ArgumentException>(() =>
                    functions.Add(new FunctionDefinition("booleanResult", typeof(bool), null, Array.Empty<FunctionParameter>(), "return true;")));
                Assert.Contains("Unsupported function return type", unsupportedType.Message);
        }

        [Fact]
        public void FunctionScope_SharesFunctionMapAndKeepsLocalsInItsOwnScope()
        {
                var root = new ScopeParser("");
                var function = new FunctionDefinition("getValue", typeof(int), null, Array.Empty<FunctionParameter>(), "int localValue = 7;");
                root.FunctionMap.Add(function);

                var functionScope = new ScopeParser(function.Body, root);
                functionScope.Parse();

                Assert.Same(root.FunctionMap, functionScope.FunctionMap);
                Assert.True(functionScope.FunctionMap.TryGet("getValue", out var registered));
                Assert.Same(function, registered);
                Assert.Contains("localValue", functionScope.VariableSymbols.Keys);
                Assert.DoesNotContain("localValue", root.VariableSymbols.Keys);
        }

        [Fact]
        public void ScopeParser_WhileRunsUntilConditionBecomesFalse()
        {
                var scope = new ScopeParser("""
            int remaining = 3;
            while (remaining > 0) {
                remaining--;
            }
            """);

                scope.Parse();

                Assert.Empty(scope.Diagnostics);
                Assert.Contains("remaining", scope.VariableSymbols.Keys);
                var symbol = scope.VariableSymbols["remaining"];
                Assert.Equal(0, scope.Variables[symbol]);
        }

        [Fact]
        public void ScopeParser_FunctionCallReturnsIntAndUsesParameterScope()
        {
                var scope = new ScopeParser("""
            int Add(int left, int right) {
                int local = left + right;
                return local;
            }
            int result = Add(20, 22);
            """);

                scope.Parse();

                Assert.Empty(scope.Diagnostics);
                Assert.Equal(42, scope.Variables[scope.VariableSymbols["result"]]);
                Assert.DoesNotContain("local", scope.VariableSymbols.Keys);
        }

        [Fact]
        public void ScopeParser_FunctionCallsSupportCharAndArrayReturns()
        {
                var scope = new ScopeParser("""
            char GetCharacter() {
                return 'x';
            }
            int[] GetNumbers() {
                return [3, 4, 5];
            }
            char[] GetLetters() {
                return ['a', 'b'];
            }
            char letter = GetCharacter();
            int[] values = GetNumbers();
            char[] letters = GetLetters();
            """);

                scope.Parse();

                Assert.Empty(scope.Diagnostics);
                Assert.Equal('x', scope.Variables[scope.VariableSymbols["letter"]]);
                Assert.Equal("[3, 4, 5]", scope.Variables[scope.VariableSymbols["values"]].ToString());
                Assert.Equal("[a, b]", scope.Variables[scope.VariableSymbols["letters"]].ToString());
        }

        [Fact]
        public void ScopeParser_VoidFunctionCanBeCalledAsAStatement()
        {
                var scope = new ScopeParser("""
            int calls = 0;
            void Touch() {
                calls = calls + 1;
                
            }
            Touch();
            """);

                scope.Parse();

                Assert.Empty(scope.Diagnostics);
                Assert.Equal(1, scope.Variables[scope.VariableSymbols["calls"]]);
        }

        [Fact]
        public void ScopeParser_FunctionCallThrowsWhenRequiredParameterIsMissing()
        {
                var scope = new ScopeParser("""
            int Identity(int value) {
                return value;
            }
            int result = Identity();
            """);

                var exception = Assert.Throws<InvalidOperationException>(() => scope.Parse());

                Assert.Contains("expects 1 argument(s), received 0", exception.Message);
        }

        [Fact]
        public void ScopeParser_FunctionCallThrowsWhenArgumentsArePassedToParameterlessFunction()
        {
                var scope = new ScopeParser("""
            int GetValue() {
                return 1;
            }
            int result = GetValue(2);
            """);

                var exception = Assert.Throws<InvalidOperationException>(() => scope.Parse());

                Assert.Contains("expects 0 argument(s), received 1", exception.Message);
        }

        private static object EvaluateWithBinding(
            string text,
            Dictionary<string, VariableSymbol>? variableSymbols = null,
            Dictionary<VariableSymbol, object>? variables = null)
        {
                var parser = new Parser(text);
                var syntax = parser.parse();

                if (parser.Diagnostics.Any())
                        throw new XunitException($"Parser failed for '{text}' with diagnostics: {string.Join(", ", parser.Diagnostics)}");

                variableSymbols ??= new Dictionary<string, VariableSymbol>();
                variables ??= new Dictionary<VariableSymbol, object>();

                var binder = new Binder(variableSymbols);
                var boundExpression = binder.Bind(syntax);

                if (binder.Diagnostics.Any())
                        throw new XunitException($"Binder failed for '{text}' with diagnostics: {string.Join(", ", binder.Diagnostics)}");

                var evaluator = new Evaluator(boundExpression, variables);
                return evaluator.Evaluate();
        }

        private static void AssertEvaluationMatches(EvaluationCase testCase)
        {
                var result = EvaluateWithBinding(testCase.Text);
                Assert.Equal(testCase.Expected, result);
        }

        public sealed record EvaluationCase(string Name, string Text, object Expected)
        {
                public override string ToString() => $"{Name}: {Text}";
        }
}
