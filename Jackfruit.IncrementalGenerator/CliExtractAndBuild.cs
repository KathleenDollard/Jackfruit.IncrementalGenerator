using Jackfruit.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using System.Xml.Linq;
using static Jackfruit.IncrementalGenerator.RoslynHelpers;

namespace Jackfruit.IncrementalGenerator
{
    public static class CliExtractAndBuild
    {

        public static CommandDef? GetCommandDef(GeneratorSyntaxContext context)
            =>            context.SemanticModel.GetOperation(context.Node) is not IInvocationOperation cliCreateInvocation
                // Weird, but we do not want to throw
                ? null
                : GetCommandDefTreeApproach(cliCreateInvocation, context.SemanticModel);

        private static CommandDef? GetCommandDefTreeApproach(IInvocationOperation cliCreateInvocation, SemanticModel semanticModel)
        {
            // Transform1: (using the mode)
            //      * Get the single parameter, which is the root of an explicit tree
            //      * Traverse the tree, depth first and for each node - build the path on traversal:
            //          * Extract the delegate and details from it
            //          * Extract XML comments (as an XML blob)
            //          * Extract known attributes from method declaration and parameters
            //          * Create command and member defs

            string[] path = { };
            var cliNodeCreate = cliCreateInvocation.Arguments[0];
            if (cliNodeCreate.Value is IConversionOperation conversionOp)
            {

                if (conversionOp.Operand is IObjectCreationOperation objectCreationOp)
                { return GetCommandDefTree(path, objectCreationOp); }
            }
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
            if (commandDef is null)
            { return null; }

            var subCommandOperations = GetSubCommandOperations(objectCreationOp);
            var newPath = path.Union(new string[] { methodSymbol.Name }).ToArray();
            var subCommands = subCommandOperations
                    .Select(x => GetCommandDefTree(newPath, x))
                    .Where(x => x is not null)
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
            { return Enumerable.Empty<IObjectCreationOperation>(); }
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
