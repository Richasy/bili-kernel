using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BiliKernel.SourceGenerator;

/// <summary>
/// RelayCommandAotSourceGenerator.
/// </summary>
[Generator]
public sealed class AotSourceGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var syntaxProvider = context.SyntaxProvider.CreateSyntaxProvider(
            (syntaxNode, _) => syntaxNode is ClassDeclarationSyntax,
            (ctx, cancellationToken) => ctx.SemanticModel.GetDeclaredSymbol(ctx.Node, cancellationToken));

        // remove duplicate symbols from syntax provider
        var distinctSyntaxProvider = syntaxProvider.Collect()
            .SelectMany((symbols, _) => symbols.Distinct(SymbolEqualityComparer.Default));

        context.RegisterSourceOutput(distinctSyntaxProvider, (ctx, symbol) =>
        {
            if (symbol is not INamedTypeSymbol type || type.IsGenericType || type.IsAbstract)
            {
                return;
            }

            var baseType = type.BaseType;
            var candidate = false;
            while (baseType != null)
            {
                if (baseType.Name == "ViewModelBase")
                {
                    candidate = true;
                    break;
                }

                baseType = baseType.BaseType;
            }

            if (!candidate)
            {
                return;
            }

            var members = type.GetMembers();
            var relayCommands = members.Where(
                m => m.Kind == SymbolKind.Method && m.GetAttributes().Any(a => a.AttributeClass.Name == "RelayCommandAttribute"))
                .Cast<IMethodSymbol>()
                .ToImmutableArray();
            var observableProperties = members.Where(
                m => m.Kind == SymbolKind.Field && m.GetAttributes().Any(a => a.AttributeClass.Name == "ObservablePropertyAttribute"))
                .Cast<IFieldSymbol>()
                .ToImmutableArray();
            var properties = members.Where(
                m => m.Kind == SymbolKind.Property)
                .Cast<IPropertySymbol>()
                .ToImmutableArray();

            var sb = new StringBuilder();

            if (type.ContainingNamespace != null)
            {
                sb.AppendLine($"namespace {type.ContainingNamespace};");
            }

            sb.AppendLine($"partial class {type.Name} : global::Microsoft.UI.Xaml.Data.IBindableCustomPropertyImplementation");
            sb.AppendLine("{");
            sb.AppendLine("    global::Microsoft.UI.Xaml.Data.BindableCustomProperty global::Microsoft.UI.Xaml.Data.IBindableCustomPropertyImplementation.GetProperty(string name)");
            sb.AppendLine("    {");

            foreach (var relayCommand in relayCommands)
            {
                var isAsync = relayCommand.IsAsync || relayCommand.Name.EndsWith("Async", StringComparison.Ordinal);
                var commandName = isAsync
                    ? relayCommand.Name.EndsWith("Async", StringComparison.Ordinal)
                        ? relayCommand.Name.Substring(0, relayCommand.Name.Length - 5)
                        : relayCommand.Name
                    : relayCommand.Name;
                sb.AppendLine($$"""
                            if (name == "{{commandName}}Command")
                            {
                                return new global::Microsoft.UI.Xaml.Data.BindableCustomProperty(
                                    true,
                                    false,
                                    "{{commandName}}Command",
                                    typeof(global::CommunityToolkit.Mvvm.Input.{{(isAsync ? "IAsyncRelayCommand" : "IRelayCommand")}}),
                                    static (instance) => (({{type}})instance).{{commandName}}Command,
                                    null,
                                    null,
                                    null
                                );
                            }
                    """);
            }

            foreach (var observableProperty in observableProperties)
            {
                var propertyNameChars = observableProperty.Name.ToCharArray().AsSpan();
                while (propertyNameChars.Length >= 1 && propertyNameChars[0] == '_')
                {
                    propertyNameChars = propertyNameChars.Slice(1);
                }

                if (propertyNameChars.Length == 0)
                {
                    continue;
                }

                propertyNameChars[0] = char.ToUpperInvariant(propertyNameChars[0]);
                var propertyName = new string(propertyNameChars.ToArray());
                var propertyType = observableProperty.Type.WithNullableAnnotation(NullableAnnotation.NotAnnotated);

                sb.AppendLine($$"""
                            if (name == "{{propertyName}}")
                            {
                                return new global::Microsoft.UI.Xaml.Data.BindableCustomProperty(
                                    true,
                                    true,
                                    "{{propertyName}}",
                                    typeof({{propertyType}}),
                                    static (instance) => (({{type}})instance).{{propertyName}},
                                    static (instance, value) => (({{type}})instance).{{propertyName}} = ({{propertyType}})value,
                                    null,
                                    null
                                );
                            }
                    """);
            }

            foreach (var property in properties)
            {
                var canGet = property.GetMethod is not null;
                var canSet = property.SetMethod is not null
                    && !property.SetMethod.IsInitOnly
                    && property.SetMethod.DeclaredAccessibility != Accessibility.Private;
                var propertyType = property.Type.WithNullableAnnotation(NullableAnnotation.NotAnnotated);

                sb.AppendLine($$"""
                            if (name == "{{property.Name}}")
                            {
                                return new global::Microsoft.UI.Xaml.Data.BindableCustomProperty(
                                    {{(canGet ? "true" : "false")}},
                                    {{(canSet ? "true" : "false")}},
                                    "{{property.Name}}",
                                    typeof({{propertyType}}),
                                    {{(canGet ? $"static (instance) => (({type})instance).{property.Name}" : "null")}},
                                    {{(canSet ? $"static (instance, value) => (({type})instance).{property.Name} = ({propertyType})value" : "null")}},
                                    null,
                                    null
                                );
                            }
                    """);
            }

            sb.AppendLine("        return default;");
            sb.AppendLine("    }");

            sb.AppendLine("""
                    global::Microsoft.UI.Xaml.Data.BindableCustomProperty global::Microsoft.UI.Xaml.Data.IBindableCustomPropertyImplementation.GetProperty(global::System.Type indexParameterType)
                    {
                        return default;
                    }
                """);

            sb.AppendLine("}");

            ctx.AddSource($"{type}.BindableCustomProperty.g.cs", sb.ToString());
        });
    }
}
