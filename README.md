# GLMRS compiler frontend

A small C# expression-language compiler frontend. It tokenizes source, builds a syntax tree, binds types and variables, and evaluates bound expressions.

## Main components

- `Lexer` converts source text into tokens.
- `Parser` builds syntax nodes from tokens.
- `Binder` resolves variables and validates expression types.
- `Evaluator` evaluates bound expressions.
- `ScopeParser` parses nested blocks and resolves names through parent scopes.
- `VariableSymbol` stores a variable's name and type information.

## Language features

The language currently supports integer, boolean, and character literals; typed variable declarations, references, and assignments; arithmetic and boolean operators; equality; arrays and indexing; unary operators; parentheses; and `if` conditions.

The current pipeline is source text -> tokens -> syntax -> bound expressions -> evaluation.

## Run

```sh
dotnet run
```

The sample source is read from `program.txt` in the current directory.
