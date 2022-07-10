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
            return context.SemanticModel.GetOperation(context.Node, cancellationToken) switch
            {
                IInvocationOperation rootCommandCreateInvocation => CommandDefFromInvocation(rootCommandCreateInvocation, cancellationToken),
                IInvalidOperation invalidOp => CommandDefWithParams(invalidOp, cancellationToken),
                _ => null
            };

            static CommandDefNode? CommandDefFromInvocation(IInvocationOperation rootCommandCreateInvocation, CancellationToken cancellationToken)
                => GetCommandDefNode(new string[] { }, null, Enumerable.Empty<MemberDef>(), rootCommandCreateInvocation, cancellationToken);

            static CommandDefNode? CommandDefWithParams(IInvalidOperation invalidOp, CancellationToken cancellationToken)
            {
                var invocationOp = invalidOp.ChildOperations.OfType<IInvocationOperation>().FirstOrDefault();
                return invocationOp is null
                    ? null
                    : GetCommandDefNode(new string[] { }, null, Enumerable.Empty<MemberDef>(), invocationOp, cancellationToken);
            }

        }

        private static IEnumerable<IInvocationOperation?>? InvocationsFromArg(IArgumentOperation argOp)
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
            CliNodeNewParts(IInvocationOperation? invocationOp)
        {
            if (invocationOp is null || !invocationOp.Arguments.Any())
            { return (null, null, Enumerable.Empty<IInvocationOperation>()); }

            var handlerArg =
                    invocationOp.Arguments[0].Parameter?.Type.ToString() == "System.Delegate"
                    ? invocationOp.Arguments[0]
                    : null;
            var validatorArg =
                    invocationOp.Arguments.Length > 1 && invocationOp.Arguments[1].Parameter?.Type.ToString() == "System.Delegate"
                    ? invocationOp.Arguments[1]
                    : null;
            var subCommandArgs =
                    handlerArg is null
                    ? invocationOp.Arguments
                    : validatorArg is null
                        ? invocationOp.Arguments.Skip(1)
                        : invocationOp.Arguments.Skip(2);

            var argCount = invocationOp.Arguments.Count();
            var handler =
                    handlerArg is null
                    ? null
                    : MethodFromArg(handlerArg);

            var validator =
                    validatorArg is null
                    ? null
                    : MethodFromArg(validatorArg);

            var subCommands = subCommandArgs
                .SelectMany(x => InvocationsFromArg(x))
                .Where(x => x is not null)
                .Select(x => x!)
                .ToList();

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
                                                         IInvocationOperation? invocationOp,
                                                         CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return null;
            }

            var (handlerSymbol, validateSymbol, subCommandsOps) = CliNodeNewParts(invocationOp);

            var commandDetails = Helpers.GetDetails(handlerSymbol, parentNode is null);
            if (commandDetails is null)
            { return null; }

            var commandDef = Helpers.BuildCommandDef(path,
                                                     parentNode,
                                                     CommonHelpers.MethodFullName(handlerSymbol),
                                                     commandDetails,
                                                     ancestorMembers,
                                                     CommonHelpers.RootCommand);
            if (commandDef is null)
            { return null; }

            var validatorDef = Helpers.GetValidatorDef(validateSymbol, commandDef);
            commandDef.Validator = validatorDef;
            var newPath = path.Union(new string[] { handlerSymbol?.Name ?? "UNKNOWN" }).ToArray();

            var commandDefNode = new CommandDefNode(commandDef);
            var subCommandNodes = subCommandsOps
                    .Select(x => GetCommandDefNode(newPath, commandDefNode, ancestorMembers.Union(commandDef.Members), x, cancellationToken))
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
