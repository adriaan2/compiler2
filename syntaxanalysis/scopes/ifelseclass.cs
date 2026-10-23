// namespace syntaxer.scopes;

// using syntaxer;

// public sealed class IfStatementSyntax :LiteralExpressionsyntax
// {
//     public IfStatementSyntax(
//         Syntaxtoken ifKeyword,
//         Syntaxtoken openParen,
//         LiteralExpressionsyntax condition,
//         Syntaxtoken closeParen,
//         ElseClauseSyntax? elseClause)
//     {
//         IfKeyword = ifKeyword;
//         OpenParen = openParen;
//         Condition = condition;
//         CloseParen = closeParen;
//         ElseClause = elseClause;
//     }

//     public Syntaxtoken IfKeyword { get; }
//     public Syntaxtoken OpenParen { get; }
//     public LiteralExpressionsyntax Condition { get; }
//     public Syntaxtoken CloseParen { get; }
//     public ElseClauseSyntax? ElseClause { get; }

//     public override GuidanceSignalKind Kind => GuidanceSignalKind.ifexpression;

//     public override IEnumerable<GuidanceSignalNode> getchildren()
//     {
//         yield return IfKeyword;
//         yield return OpenParen;
//         yield return Condition;
//         yield return CloseParen;

//         if (ElseClause is not null)
//             yield return ElseClause;
//     }
// }

// public sealed class ElseClauseSyntax : BlockStatementSyntax
// {
//     public ElseClauseSyntax(Syntaxtoken elseKeyword, GuidanceSignalNode elseStatement)
//     {
//         ElseKeyword = elseKeyword;
//         ElseStatement = elseStatement;
//     }

//     public Syntaxtoken ElseKeyword { get; }
//     public GuidanceSignalNode ElseStatement { get; }

//     public override GuidanceSignalKind Kind => GuidanceSignalKind.elsekeyword;

//     public override IEnumerable<GuidanceSignalNode> getchildren()
//     {
//         yield return ElseKeyword;
//         yield return ElseStatement;
//     }
// }
