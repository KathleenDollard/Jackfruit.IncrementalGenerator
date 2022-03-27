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
        public void Initialize(IncrementalGeneratorInitializationContext initContext)
        {

            // Gather invocations from the dummy static methods for creating the console app
            // and adding subcommands. Then find the specified delegate and turn into a CommandDef
            // TODO: Pipe locations through so any later diagnostics work
            var commandDefs = initContext.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => Helpers.IsSyntaxInteresting(s),
                    transform: static (ctx, _) => Helpers.GetCommandDef(ctx))
                .Where(static m => m is not null)
                .Select(static (m, _) => m!);

            // Create a tree in the shape of the CLI. We will use both the listand the and tree to generate
            var rootCommandDef = commandDefs
                .Collect()
                .Select(static (x, _) => x.TreeFromList(0));

            // Generate the console app, including the nested classes that provide access for adding subcommands
            // TODO: Support an empty CommandDef so that we can generated the default ConsoleApp for Intellisense
            var consoleCodeFileModel = rootCommandDef
                .Select(static (x, _) => CreateSource.GetConsoleApp(x));

            // Generate classes for each command. This code creates the System.CommandLine tree and includes the handler
            // It also collects the classes together, then adds the root so we know the namespace and can name the file we output
            var commandsCodeFileModel = commandDefs
                .Combine(rootCommandDef)
                .Select(static (x, _) => CreateSource.GetCommandClass(x.Item1, x.Item2)) 
                .Collect()
                .Combine(rootCommandDef)
                .Select(static (x, _) => CreateSource.WrapClassesInCodefile(x.Item1, x.Item2));

            // And finally, we output two files/sources
            initContext.RegisterSourceOutput(consoleCodeFileModel,
                static (context, codeFileModel) => OutputGenerated(codeFileModel, context));
            initContext.RegisterSourceOutput(commandsCodeFileModel,
                static (context, codeFileModel) => OutputGenerated(codeFileModel, context));

        }


        private static void OutputGenerated(CodeFileModel codeFileModel, SourceProductionContext context)
        {
            var writer = new StringBuilderWriter(3);
            var language = new LanguageCSharp(writer);
            language.AddCodeFile(codeFileModel);
            context.AddSource(codeFileModel.Name, writer.Output());

        }

    }
}
