# Compiler frontend architecture

GLMRS is a small C# expression-language compiler frontend. Its current pipeline is:

1. `Lexer` turns source text into tokens.
2. `Parser` builds syntax nodes from tokens.
3. `Binder` validates types and resolves variables into bound expressions.
4. `Evaluator` evaluates bound expressions.
5. `ScopeParser` recursively parses nested blocks and maintains parent-linked scopes.

Syntax files live under `syntaxanalysis/syntaxer/`; binding and evaluation files live under `syntaxanalysis/bindings/`; block parsing and scope management live under `syntaxanalysis/scopes/`.
