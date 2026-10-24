namespace syntaxer.scopes;

using bindings;

/// <summary>One parsed function signature and its source body.</summary>
public sealed record FunctionDefinition(
    string Name,
    Type ReturnType,
    Type? ReturnElementType,
    IReadOnlyList<FunctionParameter> Parameters,
    string Body,
    ScopeParser? DeclaringScope = null);

public sealed record FunctionParameter(string Name, Type Type, Type? ElementType = null);

/// <summary>Function declarations visible to a compilation's scopes.</summary>
public sealed class Functions
{
        private readonly Dictionary<string, FunctionDefinition> _definitions = new(StringComparer.Ordinal);

        public IReadOnlyDictionary<string, FunctionDefinition> Definitions => _definitions;

        public bool TryGet(string name, out FunctionDefinition definition) =>
            _definitions.TryGetValue(name, out definition!);

        public void Add(FunctionDefinition definition, ScopeParser? declaringScope = null)
        {
                ArgumentNullException.ThrowIfNull(definition);
                ValidateReturnType(definition.ReturnType, definition.ReturnElementType);
                foreach (var parameter in definition.Parameters)
                        ValidateValueType(parameter.Type, parameter.ElementType, $"parameter '{parameter.Name}'");

                if (declaringScope is not null)
                        definition = definition with { DeclaringScope = declaringScope };
                if (!_definitions.TryAdd(definition.Name, definition))
                        throw new InvalidOperationException($"Function '{definition.Name}' is already declared.");
        }

        internal object? Invoke(FunctionDefinition definition, object[] arguments)
        {
                if (arguments.Length != definition.Parameters.Count)
                        throw new InvalidOperationException($"Function '{definition.Name}' expects {definition.Parameters.Count} argument(s), received {arguments.Length}.");

                var parent = definition.DeclaringScope ?? new ScopeParser(string.Empty);
                var execution = new FunctionExecutionContext(definition);
                var functionScope = new ScopeParser(definition.Body, parent, execution);
                for (var i = 0; i < arguments.Length; i++)
                {
                        var parameter = definition.Parameters[i];
                        functionScope.AddParameter(parameter, arguments[i]);
                }

                functionScope.Parse();
                if (functionScope.Diagnostics.Any())
                        throw new InvalidOperationException(string.Join(Environment.NewLine, functionScope.Diagnostics));

                if (definition.ReturnType == typeof(void))
                        return null;
                if (!execution.HasReturned)
                        throw new InvalidOperationException($"Function '{definition.Name}' did not return a value.");
                return execution.Value;
        }

        public static void ValidateReturnType(Type type, Type? elementType)
        {
                if (type == typeof(void))
                {
                        if (elementType is not null)
                                throw new ArgumentException("A void function cannot have an element type.");
                        return;
                }

                ValidateValueType(type, elementType, "function return type");
        }

        private static void ValidateValueType(Type type, Type? elementType, string description)
        {
                if (type == typeof(int) || type == typeof(char))
                {
                        if (elementType is not null)
                                throw new ArgumentException($"The {description} cannot have an array element type.");
                        return;
                }

                if (type == typeof(ArrayValue) && (elementType == typeof(int) || elementType == typeof(char)))
                        return;

                throw new ArgumentException($"Unsupported {description}. Supported types are int, char, int[], char[], and void for returns.");
        }
}

internal sealed class FunctionExecutionContext
{
        public FunctionExecutionContext(FunctionDefinition definition) => Definition = definition;
        public FunctionDefinition Definition { get; }
        public bool HasReturned { get; set; }
        public object? Value { get; set; }
}
