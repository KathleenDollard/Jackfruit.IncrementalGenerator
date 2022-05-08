using Jackfruit.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using System.Xml.Linq;
using static Jackfruit.IncrementalGenerator.RoslynHelpers;

namespace Jackfruit.IncrementalGenerator
{
    public static class CliExtractAndBuild
    {
        public static bool IsSyntaxInteresting(SyntaxNode node)
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
                var name = Helpers.GetName(invocation.Expression);
                return name == null
                    ? false
                    : name == Helpers.AddCommandName && argCount == 1
                        ? true
                        : name == Helpers.CreateName
                            ? argCount == 1 && Helpers.GetCaller(invocation.Expression) == Helpers.Cli
                            : false;
            }
            return false;

        }


        public static CommandDef? GetCommandDef(GeneratorSyntaxContext context)
            => context.Node is not InvocationExpressionSyntax invocation
                // Weird, but we do not want to throw
                ? null
                : GetCommandDefTreeApproach(invocation, context.SemanticModel);

        private static CommandDef? GetCommandDefTreeApproach(InvocationExpressionSyntax invocation, SemanticModel semanticModel)
        {
            // Transform1: (using the mode)
            //      * Get the single parameter, which is the root of an explicit tree
            //      * Traverse the tree, depth first and for each node - build the path on traversal:
            //          * Extract the delegate and details from it
            //          * Extract XML comments (as an XML blob)
            //          * Extract known attributes from method declaration and parameters
            //          * Create command and member defs

            string[] path = { };
            var cliNodeArg = invocation.ArgumentList.Arguments[0].Expression;
            if (cliNodeArg is not ImplicitObjectCreationExpressionSyntax cliNodeCreate)
            { return null; }

            var operation2 = semanticModel.GetOperation(cliNodeCreate);
            if (operation2 is IObjectCreationOperation objectCreationOp)
            { return GetCommandDefTree(path, objectCreationOp); }
            return null;
        }

        private static CommandDef? GetCommandDefTree(string[] path, IObjectCreationOperation objectCreationOp)
        {
            var methodSymbol = GetMethodFromObjectCreationOp(objectCreationOp);
            if (methodSymbol is null) { return null; }

            var commandDetails = Helpers.GetDetails(methodSymbol);
            if (commandDetails is null)
            { return null; }

            var commandDef = Helpers.BuildCommandDef(path, Helpers.MethodFullName(methodSymbol), commandDetails, Helpers.Cli);
            if (commandDef is null )
            { return null; }

            var subCommandOperations = GetSubCommandOperations(objectCreationOp);
            var newPath = path.Union(new string[] {methodSymbol.Name}).ToArray();
            var subCommands = subCommandOperations
                    .Select(x => GetCommandDefTree(newPath, x))
                    .Where(x=>x is not null)
                    .ToList();
            commandDef.SubCommands = subCommands!;
            return commandDef;
        }

        private static IMethodSymbol? GetMethodFromObjectCreationOp(IObjectCreationOperation objectCreationOp)
            => !objectCreationOp.Arguments.Any()
                ? null
                : objectCreationOp.Arguments[0]
                     .Descendants()
                     .OfType<IDelegateCreationOperation>()
                     .FirstOrDefault()
                     ?.Target switch
                {
                    IMethodReferenceOperation methodRefOp => methodRefOp.Method,
                    _ => null
                };

        private static IEnumerable<IObjectCreationOperation> GetSubCommandOperations(IObjectCreationOperation objectCreationOp)
        {
            if (objectCreationOp.Arguments.Count() != 2)
            { return Enumerable.Empty<IObjectCreationOperation>(); }

            var subCommandArg = objectCreationOp.Arguments[1];
            var initializerOp = subCommandArg
                    .Descendants()
                    .OfType<IObjectOrCollectionInitializerOperation>()
                    .Where(x => x.Type?.ToString() == "System.Collections.Generic.List<Jackfruit.CliNode>")
                    .FirstOrDefault();
            if (initializerOp is null)
            {  return Enumerable.Empty<IObjectCreationOperation>(); }
            var list = new List<IObjectCreationOperation>();
            foreach (var initializer in initializerOp.Initializers)
            {
                var subCommandCreationOp = initializer.Descendants()
                        .OfType<IObjectCreationOperation>()
                        .FirstOrDefault();
                list.Add(subCommandCreationOp);
            }
            return list;
        }
    }
}
