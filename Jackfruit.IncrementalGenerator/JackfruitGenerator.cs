using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Jackfruit.Models;
using System.Collections.Immutable;
using Jackfruit.IncrementalGenerator;
using Jackfruit.IncrementalGenerator.Output;
using Jackfruit.IncrementalGenerator.CodeModels;

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
        private const string cliClassCode = @"
namespace Jackfruit
{
    public partial class Cli
    {
        public static void Create(CliNode cliRoot)
        { }
    }
}";
        public void Initialize(IncrementalGeneratorInitializationContext initContext)
        {
            // To be a partial, this must be in the same namespace and assembly as the generated part
            initContext.RegisterPostInitializationOutput(ctx => ctx.AddSource($"{Helpers.Cli}.partial.g.cs", cliClassCode));

            // Gather invocations from the dummy static methods for creating the console app
            // and adding subcommands. Then find the specified delegate and turn into a CommandDef
            // TODO: Pipe locations through so any later diagnostics work
            var commandDefs = initContext.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => Helpers.IsSyntaxInteresting(s),
                    transform: static (ctx, _) => Helpers.GetCommandDef(ctx))
                .Where(static m => m is not null)!;

            // Create a tree in the shape of the CLI. We will use both the listand the and tree to generate
            var rootCommandDef = commandDefs
                .Collect()
                .Select(static (x, _) => x.TreeFromList());

            // Generate classes for each command. This code creates the System.CommandLine tree and includes the handler
            // It also collects the classes together, then adds the root so we know the namespace and can name the file we output
            var commandsCodeFileModel = rootCommandDef
                .Select((x, _) => CreateSource.GetCommandCodeFile(x));

            var cliPartialCodeFileModel = rootCommandDef
                .Select((x, _) => CreateSource.GetCliPartialCodeFile(x));

            // And finally, we output files/sources
            initContext.RegisterSourceOutput(cliPartialCodeFileModel,
                static (context, codeFileModel) => OutputCliPartial(codeFileModel, context));

            initContext.RegisterSourceOutput(commandsCodeFileModel,
                static (context, codeFileModel) => OutputGenerated(codeFileModel, context));
        }


        private static void OutputGenerated(CodeFileModel codeFileModel, SourceProductionContext context)
        {
            var writer = new StringBuilderWriter(3);
            var language = new LanguageCSharp(writer);
            language.AddCodeFile(codeFileModel);
            context.AddSource($"{codeFileModel.Name}.g.cs", writer.Output());
        }

        private static void OutputCliPartial(CodeFileModel? codeFileModel, SourceProductionContext context)
        {
            if (codeFileModel == null)
            { return; }
            var writer = new StringBuilderWriter(3);
            var language = new LanguageCSharp(writer);
            language.AddCodeFile(codeFileModel);
            context.AddSource($"{Helpers.Cli}.g.cs", writer.Output());
        }

    }
}
