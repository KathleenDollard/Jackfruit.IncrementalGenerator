using Jackfruit.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using System.Xml.Linq;

namespace Jackfruit.IncrementalGenerator
{
    public static class CliExtractAndBuild
    {
        // temporary 
        private static SemanticModel semanticModel { get; set; }
        public static CommandDef? GetCommandDef(GeneratorSyntaxContext context)
        {
            semanticModel = context.SemanticModel;
            if (context.SemanticModel.GetOperation(context.Node) is not IInvocationOperation cliCreateInvocation)
            {
                // Weird, but we do not want to throw
                return null;
            }
            // Transform1: (using the mode)
            //      * Get the single parameter, which is the root of an explicit tree
            //      * Traverse the tree, depth first and for each node - build the path on traversal:
            //          * Extract the delegate and details from it
            //          * Extract XML comments (as an XML blob)
            //          * Extract known attributes from method declaration and parameters
            //          * Create command and member defs

            string[] path = { };
            var objectCreationOps = ObjectCreationFromArg(cliCreateInvocation.Arguments[0]);
            return objectCreationOps is null
                ? null
                : objectCreationOps.Any()
                    ? GetCommandDef(path, objectCreationOps.First())
                    : null;
        }

        private static IEnumerable<IObjectCreationOperation>? ObjectCreationFromArg(IArgumentOperation argOp)
        {
            var parentCreate = argOp.Parent as IObjectCreationOperation;
            var creationOps = argOp.Value switch
            {
                IObjectCreationOperation objectCreateOp => new List<IObjectCreationOperation> { objectCreateOp },
                IConversionOperation conversionOp =>
                    conversionOp.Operand switch
                    {
                        IObjectCreationOperation objectCreationOp => new List<IObjectCreationOperation> { objectCreationOp },
                        _ => null
                    },
                IArrayCreationOperation arrayCreateOp =>
                    arrayCreateOp.Initializer switch
                    {
                        IArrayInitializerOperation arrayInitOp =>
                            arrayInitOp.ElementValues
                            .Select(x => ObjectCreationFromInit(x))
                            .Where(x => x is not null)!,
                        _ => null
                    },
                _ => null
            };

            return creationOps.Where(x => x is not null)!;

            static IObjectCreationOperation? ObjectCreationFromInit(IOperation op)
                => op switch
                {
                    IObjectCreationOperation createOp => createOp,
                    IConversionOperation conversionOp =>
                        conversionOp.Operand switch
                        {
                            IObjectCreationOperation createOp => createOp,
                            _ => null
                        },
                    _ => null
                };
        }

        private static (IMethodSymbol? handlerMethodSymbol, IMethodSymbol? validatorMethodSymbol, IEnumerable<IObjectCreationOperation> subCommands) CliNodeNewParts(IObjectCreationOperation objectCreationOp)
        {
            // Change this to a collection switch when we get them. it will be beautiful!
            if (!objectCreationOp.Arguments.Any())
            { return (null, null, Enumerable.Empty<IObjectCreationOperation>()); }
            var argCount = objectCreationOp.Arguments.Count();
            var handler = MethodFromArg(objectCreationOp.Arguments[0]);
            IMethodSymbol? validator = null;
            IEnumerable<IObjectCreationOperation>? subCommands = null;

            if (argCount > 1)
            {
                var pos = 1;
                var arg = objectCreationOp.Arguments[pos];
                // second arg could be a validator or a CliNode
                if (arg.Value is Delegate)  // this will never be true and is a placeholder while I am in this logic
                {
                    validator = MethodFromArg(objectCreationOp.Arguments[pos]);
                    pos++;
                }

                IEnumerable<IArgumentOperation> temp = objectCreationOp.Arguments
                                    .Skip(pos);
                // This SelectMany only to collapse paramarray
                subCommands = temp
                    .SelectMany(x => ObjectCreationFromArg(x))
                    .Where(x => x is not null)
                    .ToList();

            }
            return (handler, validator, subCommands ?? Enumerable.Empty<IObjectCreationOperation>());

            // TODO: Add Validation for handler and validator return types
            static IMethodSymbol? MethodFromArg(IArgumentOperation argOp)
                // TODO: Be explicit here
                => argOp
                         .Descendants()
                         .OfType<IDelegateCreationOperation>()
                         .FirstOrDefault()
                         ?.Target switch
                {
                    IMethodReferenceOperation methodRefOp => methodRefOp.Method,
                    _ => null
                };
        }

        private static CommandDef? GetCommandDef(string[] path, IObjectCreationOperation objectCreationOp)
        {
            var (handlerSymbol, validateSymbol, subCommandsOps) = CliNodeNewParts(objectCreationOp);
            if (handlerSymbol is null)
            { return null; }

            var commandDetails = Helpers.GetDetails(handlerSymbol);
            if (commandDetails is null)
            { return null; }

            var commandDef = Helpers.BuildCommandDef(path, Helpers.MethodFullName(handlerSymbol), commandDetails, Helpers.Cli);
            if (commandDef is null)
            { return null; }

            var newPath = path.Union(new string[] { handlerSymbol.Name }).ToArray();
            var subCommands = subCommandsOps
                    .Select(x => GetCommandDef(newPath, x))
                    .Where(x => x is not null)
                    .ToList();
            commandDef.SubCommands = subCommands!;
            return commandDef;
        }
    }
}
