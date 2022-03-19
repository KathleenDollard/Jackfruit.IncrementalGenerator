using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Jackfruit.Models;
using Microsoft.CodeAnalysis.CSharp;
using static Jackfruit.IncrementalGenerator.RoslynHelpers;

// Next Steps:
// * Create a CommandDef test generator that just looks at CommandDef
// * Add Tests for
//   * Parameter descriptions
//   * Aliases, ArgumentDisplayName, and Required
//   * Arguments both as Arg suffix and as attribute
// * Get the return type name and add tests for that
// * Docuent were we may want to add analyzers later for attributes on the wrong symbol type

// Once that is good to go, figure out how to generate code in C# :-( (I liked my F# DSL a lot)

namespace Jackfruit.IncrementalGenerator
{
    [Generator]
    public class Generator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext initContext)
        {
            initContext.RegisterPostInitializationOutput(ctx => ctx.AddSource(
                "ConsoleApplication.g.cs",
                SourceText.From(Helpers.ConsoleClass, Encoding.UTF8)));

            IncrementalValuesProvider<CommandDef> commandDefs = initContext.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => Helpers.IsSyntaxInteresting(s),
                    transform: static (ctx, _) => Helpers.GetCommandDef(ctx))
                .Where(static m => m is not null)!;

            initContext.RegisterSourceOutput(commandDefs,
                static (context, commandDef) => Generate(commandDef, context));

        }

        private static void Generate(CommandDef commandDef, SourceProductionContext context)
        {
            var joinedPath = string.Join("", commandDef.Path);
            var output = $"// {commandDef.Id} - {joinedPath} - Description: {commandDef.Description}";
            context.AddSource($"{joinedPath}.g.cs", output);
        }

    }
}
