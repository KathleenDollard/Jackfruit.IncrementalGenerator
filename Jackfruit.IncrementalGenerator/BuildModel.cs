using Jackfruit.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using System.Xml.Linq;

namespace Jackfruit.IncrementalGenerator
{
    public static class BuildModel
    {
        public static (CommandDef RootCommandDef, IEnumerable<CommandDef> CommandDefs) FlattenWithRoot(
            CommandDefNode? commandDefNode, CancellationToken cancellationToken)
        {
            if (commandDefNode is null)
            { throw new ArgumentNullException(nameof(commandDefNode)); }
            List<CommandDef> commandDefs = GetChildren(commandDefNode);
            return new(commandDefNode.CommandDef, commandDefs);

            static List<CommandDef> GetChildren(CommandDefNode node)
            {
                var meAndMyChildren = new List<CommandDef> { node.CommandDef };
                foreach (var child in node.SubCommandNodes)
                { meAndMyChildren.AddRange(GetChildren(child)); }
                return meAndMyChildren;
            }
        }

        public static CommandDefNode? GetCommandDef(GeneratorSyntaxContext context, CancellationToken cancellationToken)
        {
            if (context.SemanticModel.GetOperation(context.Node, cancellationToken) is not IInvocationOperation cliCreateInvocation)
            {
                // Should not occur
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
            var invocationOps = InvocationFromArg(cliCreateInvocation.Arguments[0]);
            return invocationOps is null
                ? null
                : invocationOps.Any()
                    ? GetCommandDefNode(path, null, Enumerable.Empty<MemberDef>(), invocationOps.First(), cancellationToken)
                    : null;
        }

        private static IEnumerable<IInvocationOperation?>? InvocationFromArg(IArgumentOperation argOp)
        {
            var parentCreate = argOp.Parent as IInvocationOperation;
            var invocationOps = argOp.Value switch
            {
                IInvocationOperation invocationoOp => new List<IInvocationOperation> { invocationoOp },
                IArrayCreationOperation arrayCreateOp =>
                      arrayCreateOp.Initializer switch
                      {
                          IArrayInitializerOperation arrayInitOp =>
                              arrayInitOp.ElementValues
                              .Select(x => InvocationFromInit(x))
                              .Where(x => x is not null)!,
                          _ => null
                      },
                // Other approaches, such as supporting vars would be here
                _ => null
            };

            return invocationOps is null
                ? Enumerable.Empty<IInvocationOperation>()
                : invocationOps.Where(x => x is not null)!;

            static IInvocationOperation? InvocationFromInit(IOperation op)
                => op switch
                {
                    IInvocationOperation invocationoOp => invocationoOp,
                    _ => null
                };
        }

        private static (IMethodSymbol? handlerMethodSymbol, IMethodSymbol? validatorMethodSymbol,
                        IEnumerable<IInvocationOperation> subCommands)
            CliNodeNewParts(IInvocationOperation invocationOp)
        {
            // Change this to a collection switch when we get them. it will be beautiful!
            if (!invocationOp.Arguments.Any())
            { return (null, null, Enumerable.Empty<IInvocationOperation>()); }
            var argCount = invocationOp.Arguments.Count();
            var handler = MethodFromArg(invocationOp.Arguments[0]);
            IMethodSymbol? validator = null;
            IEnumerable<IInvocationOperation>? subCommands = null;

            if (argCount > 1)
            {
                var pos = 1;
                var arg = invocationOp.Arguments[pos];
                // second arg could be a validator or a CliNode
                validator = MethodFromArg(invocationOp.Arguments[pos]);
                if (validator is not null)
                { pos++; }

                IEnumerable<IArgumentOperation> temp = invocationOp.Arguments
                                    .Skip(pos);
                // This SelectMany only to collapse paramarray
                subCommands = temp
                    .SelectMany(x => InvocationFromArg(x))
                    .Where(x => x is not null)
                    .Select(x=>x!)
                    .ToList();

            }
            return (handler, validator, subCommands ?? Enumerable.Empty<IInvocationOperation>());
        }

        // TODO: Add Validation for handler and validator return types
        static IMethodSymbol? MethodFromArg(IArgumentOperation argOp)
            => argOp.Value is not IConversionOperation conversion
                ? null
                : conversion.Operand is not IDelegateCreationOperation delegateOp
                    ? null
                    : delegateOp.Target switch
                    {
                        IMethodReferenceOperation methodRefOp => methodRefOp.Method,
                        _ => null
                    };

        private static CommandDefNode? GetCommandDefNode(string[] path,
                                                         CommandDefNode? parentNode, 
                                                         IEnumerable<MemberDef> ancestorMembers,
                                                         IInvocationOperation invocationOp,
                                                         CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return null;
            }
            var (handlerSymbol, validateSymbol, subCommandsOps) = CliNodeNewParts(invocationOp);
            if (handlerSymbol is null)
            { return null; }

            var commandDetails = Helpers.GetDetails(handlerSymbol);
            if (commandDetails is null)
            { return null; }

            var commandDef = Helpers.BuildCommandDef(path,
                                                     parentNode?.CommandDef.Name,
                                                     CommonHelpers.MethodFullName(handlerSymbol),
                                                     commandDetails,
                                                     ancestorMembers,
                                                     CommonHelpers.RootCommand);
            if (commandDef is null)
            { return null; }


            var validatorDef = Helpers.GetValidatorDef(validateSymbol, commandDef);
            commandDef.Validator = validatorDef;
            var newPath = path.Union(new string[] { handlerSymbol.Name }).ToArray();

            var commandDefNode = new CommandDefNode(commandDef);
            var subCommandNodes = subCommandsOps
                    .Select(x => GetCommandDefNode(newPath, commandDefNode, ancestorMembers, x, cancellationToken))
                    .Where(x => x is not null)
                    .ToList();
            commandDef.SubCommandNames = subCommandNodes
                    .Where(n => n is not null)
                    .Select(n => n!.CommandDef.Name);
            commandDefNode.AddSubCommandNodes(subCommandNodes!);

            return commandDefNode;
        }
    }
}
