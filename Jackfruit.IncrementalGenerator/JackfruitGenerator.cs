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
            // How do I kick off a different generation if the user has not called interesting syntax.
            // is there some node that is always retrieved and I could create an empty CommandDef and 
            // carry it along and generate the alternate if it is the only CommandDef
            initContext.RegisterPostInitializationOutput(ctx => ctx.AddSource(
                "ConsoleApplication.g.cs",
                SourceText.From(Helpers.ConsoleClass, Encoding.UTF8)));

            IncrementalValuesProvider<CommandDef> commandDefs = initContext.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => Helpers.IsSyntaxInteresting(s),
                    transform: static (ctx, _) => Helpers.GetCommandDef(ctx))
                .Where(static m => m is not null)!;

            //IncrementalValueProvider<ImmutableArray<CommandDef>> collected = commandDefs.Collect();
            IncrementalValueProvider<(Compilation, ImmutableArray<CommandDef>)> collected
                    = initContext.CompilationProvider.Combine(commandDefs.Collect());
            // IncrementalValueProvider<ImmutableArray<CommandDef>> collected = commandDefs.Collect().Select(x=>x);
            IncrementalValueProvider<CommandDef> rootCommand = collected.Select(static (x, _) => BuildCli(x.Item2));

            initContext.RegisterSourceOutput(rootCommand,
                static (context, commandDef) => GenerateConsoleApp(commandDef, context));

            initContext.RegisterSourceOutput(commandDefs,
                static (context, commandDef) => GenerateCommands(commandDef, context));

        }

        private static CommandDef BuildCli(ImmutableArray<CommandDef> commandDefs)
        {
            return commandDefs.TreeFromList(0);
        }

        private static void GenerateConsoleApp(CommandDef commandDef, SourceProductionContext context)
        {
            var joinedPath = string.Join("", commandDef.Path);
            // var output = $"// {commandDef.Id} - {joinedPath} - Description: {commandDef.Description}";

            var codeFileModel = CreateSource.GetCustomApp(commandDef);
            OutputGenerated($"{joinedPath}.g.cs",codeFileModel, context);
        }

        private static void GenerateCommands(CommandDef commandDef, SourceProductionContext context)
        {
            var joinedPath = string.Join("", commandDef.Path);
            // var output = $"// {commandDef.Id} - {joinedPath} - Description: {commandDef.Description}";

            //var codeFileModel = CreateSource.GetCustomApp(commandDef);
            //OutputGenerated($"{joinedPath}.g.cs", codeFileModel, context);
        }

        private static void OutputGenerated(string hintName,CodeFileModel codeFileModel, SourceProductionContext context)
        {
            var writer = new StringBuilderWriter(3);
            var language = new LanguageCSharp(writer);
            language.AddCodeFile(codeFileModel);
            context.AddSource(hintName, writer.Output());

        }

    }
}
