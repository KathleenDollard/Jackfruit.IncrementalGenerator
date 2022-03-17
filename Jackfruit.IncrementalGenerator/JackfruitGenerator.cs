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
        private static string[] names = { "AddSubCommand", "CreateWithRootCommand" };
        public void Initialize(IncrementalGeneratorInitializationContext initContext)
        {
            initContext.RegisterPostInitializationOutput(ctx => ctx.AddSource(
                "ConsoleApplication.g.cs",
                SourceText.From(Helpers.ConsoleClass, Encoding.UTF8)));

            IncrementalValuesProvider<CommandDef> commandDefs = initContext.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => IsSyntaxInteresting(s),
                    transform: static (ctx, _) => GetCommandDef(ctx))
                .Where(static m => m is not null)!;

            initContext.RegisterSourceOutput(commandDefs,
                static (context, commandDef) => Generate(commandDef, context));

        }


        private static bool IsSyntaxInteresting(SyntaxNode node)
        {
            // Select1:
            //      * Extract all method invocations and filter by:
            //      * Name: comparing with expected list
            //      * Parameter count of 1
            if (node is InvocationExpressionSyntax invocation)
            {
                if (invocation.ArgumentList.Arguments.Count != 1)
                { return false; }
                var (path, name) = GetNameAndTarget(invocation.Expression);
                return (name is not null && names.Contains(name));
            }
            else
            { return false; }

        }

        private static (string path, string? name) GetNameAndTarget(SyntaxNode expression)
        {
            return expression switch
            {
                MemberAccessExpressionSyntax memberAccess when expression.IsKind(SyntaxKind.SimpleMemberAccessExpression)
                    => (memberAccess.Expression.ToString(), memberAccess.Name.ToString()),
                IdentifierNameSyntax identifier
                     => ("", identifier.ToString()),
                _ => ("", null)
            };
        }

        private static CommandDef? GetCommandDef(GeneratorSyntaxContext context)
        {
            // Transform1: (using the mode)
            //      * Check the path and namespace if available
            //      * Lookup the delegate method 
            //      * Extract XML comments (as an XML blob)
            //      * Extract known attributes from method declaration and parameters
            //      * Create command and member defs
            if (context.Node is not InvocationExpressionSyntax invocation)
            {
                // Weird, but we do not want to throw
                return null;
            }
            var (nspace, path) = GetNamespaceAndPath(context, invocation.Expression);

            var delegateArg = invocation.ArgumentList.Arguments[0].Expression;
            var methodSymbol = MethodOrCandidateSymbol(context.SemanticModel, delegateArg);
            if (methodSymbol is null) { return null; }

            var details = BasicDetails(methodSymbol);
            details = DescFromXmlDocComment(methodSymbol.GetDocumentationCommentXml(), details);
            details = DescFromAttributes(methodSymbol, details);



            var arg = invocation.ArgumentList.Arguments[0];
            var desc = 
                    details.TryGetValue(CommandKey, out var detail)
                    ? detail.Description
                    : "";
            return new CommandDef(delegateArg.ToString(), path, desc);

            static (string nspace, IEnumerable<string> path) GetNamespaceAndPath(GeneratorSyntaxContext context, ExpressionSyntax callingExpression)
            {
                var symbol = context.SemanticModel.GetSymbolInfo(callingExpression).Symbol;
                return symbol switch
                {
                    IMethodSymbol callingMethodSymbol
                            => (callingMethodSymbol.ContainingNamespace.ToString(),
                                callingMethodSymbol.ContainingType
                                        .ToDisplayParts()
                                        .Select(x => x.ToString())),
                    _
                            => ("", GetNameAndTarget(callingExpression).path.ToString().Split('.'))
                };
            }
        }


        private static void Generate(CommandDef commandDef, SourceProductionContext context)
        {
            var joinedPath = string.Join("", commandDef.Path);
            var output = $"// {commandDef.Id} - {joinedPath} - Description: {commandDef.Description}";
            context.AddSource($"{joinedPath}.g.cs", output);
        }

    }
}
