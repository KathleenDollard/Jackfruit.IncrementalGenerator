using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Jackfruit.Models;
using System.Collections.Immutable;
using Jackfruit.IncrementalGenerator;
using Jackfruit.IncrementalGenerator.Output;
using Jackfruit.IncrementalGenerator.CodeModels;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;

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
        /// <summary>
        /// This is the entry point for the Jackfruit generator. At present it 'jumps namespaces' 
        /// after first use, moving from Jackfruit to the namespace of your root handler. After 
        /// generation, it will include a static property to access your root by name.
        /// </summary>
        public partial class Cli
        {
            /// <summary>
            /// This method builds a tree that defines your CLI.  
            /// </summary>
            /// <param name=""cliRoot"">A CliNode pointing to your root handler.</param>
            public static void Create(CliNode cliRoot)
            { }
        }
    }
";
        public void Initialize(IncrementalGeneratorInitializationContext initContext)
        {
 
            // *** Cli approach
            // To be a partial, this must be in the same namespace and assembly as the generated part
            initContext.RegisterPostInitializationOutput(ctx => ctx.AddSource($"{Helpers.Cli}.partial.g.cs", cliClassCode));

            // Gather invocations from the dummy static methods for creating the console app
            // and adding subcommands. Then find the specified delegate and turn into a CommandDef
            // TODO: Pipe locations through so any later diagnostics work
            var cliCommandDefs = initContext.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => IsCliCreateInvocation(s),
                    transform: static (ctx, _) => CliExtractAndBuild.GetCommandDef(ctx))
                .Where(static m => m is not null)!;

            // Generate classes for each command. This code creates the System.CommandLine tree and includes the handler
            // It also collects the classes together, then adds the root so we know the namespace and can name the file we output
            var commandscliCodeFileModel = cliCommandDefs
                .Select((x, _) => CreateSource.GetCommandCodeFile(x));

            var cliPartialCodeFileModel = cliCommandDefs
                .Collect()
                .Select((x, _) => CreateSource.GetCliPartialCodeFile(x));

            initContext.RegisterSourceOutput(cliPartialCodeFileModel,
                static (context, codeFileModel) => OutputGenerated(codeFileModel, context, Helpers.Cli));

            // And finally, we output files/sources
            initContext.RegisterSourceOutput(commandscliCodeFileModel,
                static (context, codeFileModel) => OutputGenerated(codeFileModel, context, codeFileModel.Name));
        }

        // currently public because used by CommandDef generator that is used by testing
        // We may merge generators or put that generator in this assembly
        public static bool IsCliCreateInvocation(SyntaxNode node)
        {
            // Select1:
            //      * Extract all method invocations and filter by:
            //      * Name: comparing with expected list
            //      * Parameter count of 1 and caller is Cli
            if (node is InvocationExpressionSyntax invocation)
            {

                int argCount = invocation.ArgumentList.Arguments.Count;
                if (argCount == 0)
                { return false; }
                var name = GetName(invocation.Expression);
                return name == null
                    ? false
                    : name == Helpers.AddCommandName && argCount == 1
                        ? true
                        : name == Helpers.CreateName
                            ? argCount == 1 && GetCaller(invocation.Expression) == Helpers.Cli
                            : false;
            }
            return false;

        }


        private static void OutputGenerated(CodeFileModel? codeFileModel, SourceProductionContext context, string hintName)
        {
            if(codeFileModel == null)
            { return; }
            var writer = new StringBuilderWriter(3);
            var language = new LanguageCSharp(writer);
            language.AddCodeFile(codeFileModel);
            context.AddSource($"{hintName}.g.cs", writer.Output());
        }

        internal static string? GetName(SyntaxNode expression)
            => expression switch
            {
                MemberAccessExpressionSyntax memberAccess when expression.IsKind(SyntaxKind.SimpleMemberAccessExpression)
                    => memberAccess.Name is GenericNameSyntax genericName
                        ? genericName.Identifier.ValueText
                        : memberAccess.Name.ToString(),
                IdentifierNameSyntax identifier
                     => identifier.ToString(),
                _ => null
            };

        internal static string? GetCaller(SyntaxNode expression)
            => expression switch
            {
                MemberAccessExpressionSyntax memberAccess when expression.IsKind(SyntaxKind.SimpleMemberAccessExpression)
                    => memberAccess.Expression.ToString(),
                IdentifierNameSyntax identifier
                     => "",
                _ => null
            };
    }
}
