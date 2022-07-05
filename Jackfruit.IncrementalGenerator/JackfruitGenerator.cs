using Microsoft.CodeAnalysis;
using Jackfruit.Common;
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
    
    // Perf
    // Does the predicate need a cancellation token?
    // Assume commandDef does because of GetOperation
    // TODO: Does using class (over struct) undermine caching? suggest records (IEquatable) and solve deep equality
    // TODO: How much work before considering cancellation in my code? When unbounded    // 

    // use same name for context to avoid closure


    [Generator]
    public class Generator : IIncrementalGenerator
    {
        private const string cliClassCode = @"
using Jackfruit.Internal;

namespace Jackfruit
{
    /// <summary>
    /// This is the main class for the Jackfruit generator. After you call the 
    /// Create command, the returned RootCommand will contain your CLI. If you 
    /// need multiple root commands in your application differentiate them with &gt;T&lt;
    /// </summary>
    public partial class RootCommand : RootCommand<RootCommand, RootCommand.Result>
    {
        public new static RootCommand Create(SubCommand subCommand)
            => (RootCommand)RootCommand<RootCommand, RootCommand.Result>.Create(subCommand);

        public partial class Result
        { }
    }
}
";
        public void Initialize(IncrementalGeneratorInitializationContext initContext)
        {
 
            // *** Cli approach
            // To be a partial, this must be in the same namespace and assembly as the generated part
            initContext.RegisterPostInitializationOutput(ctx => ctx.AddSource($"{CommonHelpers.RootCommand}.partial.g.cs", cliClassCode));

            // Build command defs and return as a tree for each rootcommand node
            var commandDefNodes = initContext.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => IsCliCreateInvocation(s),
                    transform: static (ctx, cancellationToken) => BuildModel.GetCommandDef(ctx, cancellationToken))
                .WhereNotNull();

            // Flatten per rootcommand node
            var commandDefCollection = commandDefNodes
                .Select((node, cancellationToken) => BuildModel.FlattenWithRoot(node, cancellationToken));

            var roots = commandDefCollection
                .Select((t, _) => t.RootCommandDef);

            var commands = commandDefCollection
                .SelectMany((t, _) => t.CommandDefs);


            // Generate classes for each command. This code creates the System.CommandLine tree and includes the handler
            // It also collects the classes together, then adds the root so we know the namespace and can name the file we output
            var commandsCodeFileModel = commands
                .Select((x, _) => CreateSource.GetCommandCodeFile(x));

            //var rootCommandCodeFileModel = roots
            //    .Select((x, _) => CreateSource.GetRootCommandPartialCodeFile(x));

            // And finally, we output files/sources
            //initContext.RegisterSourceOutput(rootCommandCodeFileModel,
            //    static (context, codeFileModel) => OutputGenerated(codeFileModel, context, codeFileModel?.Name ?? ""));

            initContext.RegisterSourceOutput(commandsCodeFileModel,
                static (context, codeFileModel) => OutputGenerated(codeFileModel, context, codeFileModel?.Name ?? ""));
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
                var (className, methodName) = GetName(invocation.Expression);
                return className == CommonHelpers.RootCommand && methodName == CommonHelpers.AddCommandName ;
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

        internal static (string? className, string? methodName) GetName(SyntaxNode expression)
            => expression switch
            {
                MemberAccessExpressionSyntax memberAccess when expression.IsKind(SyntaxKind.SimpleMemberAccessExpression)
                    => (memberAccess.Expression.ToString(),
                        memberAccess.Name is GenericNameSyntax genericName
                            ? genericName.Identifier.ValueText
                            : memberAccess.Name.ToString()),
                _ => (null,null)
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
