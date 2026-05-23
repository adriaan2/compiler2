# Style Guide For Agents

Core logic should be written in the Linux kernel style: small focused functions, clear control flow, direct naming, and code that has 8 indents in codeblocks if a codeblock gets long delete it favors readability over clever abstractions.

For boilerplate, generated scaffolding, simple data holders, and mechanical glue code, strict styling is not important. Keep it understandable and consistent with nearby files, but do not spend time polishing boilerplate for style alone.

When deciding whether a change is core logic or boilerplate, treat parsing, binding, evaluation, diagnostics, type rules, operator behavior, and runtime state as core logic.
