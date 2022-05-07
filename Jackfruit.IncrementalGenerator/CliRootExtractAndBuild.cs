using Jackfruit.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using System.Xml.Linq;
using static Jackfruit.IncrementalGenerator.RoslynHelpers;

namespace Jackfruit.IncrementalGenerator
{
    public static class CliRootExtractAndBuild
    {
        public static bool IsSyntaxInteresting(SyntaxNode node)
        {
            // Select1:
            //      * Extract all method invocations and filter by:
            //      * Name: comparing with expected list
            //      * Parameter count of 1
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
                            ? argCount == 1 && Helpers.GetCaller(invocation.Expression) ==  Helpers.CliRoot
                            : false;
            }
            return false;

        }

        private static string GetPath(SyntaxNode expression)
        {
            var name = Helpers.GetName(expression);
            var path = expression switch
            {
                MemberAccessExpressionSyntax memberAccess when expression.IsKind(SyntaxKind.SimpleMemberAccessExpression)
                    => name == Helpers.CreateName
                        ? ""
                        : name != Helpers.AddCommandName
                            ? ""
                            : memberAccess.Name is GenericNameSyntax genericName
                                ? $"{Helpers.CliRoot}.{PathFromGenericTypes(genericName.TypeArgumentList.Arguments.First())}"
                                : Helpers.CliRoot,

                IdentifierNameSyntax identifier
                     => "",
                _ => ""
            };

            return path;

            static string PathFromGenericTypes(TypeSyntax type)
            {
                var typeName = type.ToString();
                return typeName.StartsWith(Helpers.NestedCommandsClassName)
                                ? typeName.Substring(Helpers.NestedCommandsClassName.Length)
                                : typeName;
            }
        }

        public static CommandDef? GetCommandDef(GeneratorSyntaxContext context)
            => context.Node is not InvocationExpressionSyntax invocation
                // Weird, but we do not want to throw
                ? null
                : GetCommandDefGenericApproach(invocation, context.SemanticModel);

        private static CommandDef? GetCommandDefGenericApproach(InvocationExpressionSyntax invocation, SemanticModel semanticModel)
        {
            // Transform1: (using the mode)
            //      * Check the path and namespace if available
            //      * Lookup the delegate method 
            //      * Extract XML comments (as an XML blob)
            //      * Extract known attributes from method declaration and parameters
            //      * Create command and member defs
            var path = GetPath(invocation.Expression).Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            //We create details first because at this point we don't know what is an argument or option
            var delegateArg = invocation.ArgumentList.Arguments[0].Expression;

            var methodSymbol = MethodOrCandidateSymbol(semanticModel, delegateArg);
            if (methodSymbol is null) { return null; }

            var commandDetails = Helpers.GetDetails(methodSymbol);
            if (commandDetails is null)
            { return null; }

            var commandDef = Helpers.BuildCommandDef(path, Helpers.MethodFullName(methodSymbol), commandDetails, Helpers.CliRoot);
            return commandDef;
        }

        public static CommandDefBase TreeFromList(this IEnumerable<CommandDef> commandDefs, int pos = 0)
            => TreeFromListInternal(commandDefs, pos).FirstOrDefault() ?? new EmptyCommandDef();

        public static IEnumerable<CommandDefBase> TreeFromListInternal(this IEnumerable<CommandDef> commandDefs, int pos)
        {
            if (pos > 10) { throw new InvalidOperationException("Runaway recursion suspected"); }
            // This throws on badly formed trees. not sure whether to just let that happen and catch, or do more work here
            var roots = commandDefs.Where(x => GroupKey(x, pos) is null);

            foreach (var root in roots)
            {
                if (Helpers.GetStyle(root) != Helpers.Cli)
                {
                    var subCommands = ProcessRoot(pos, commandDefs, roots, root);
                    root.SubCommands = subCommands;
                }
            }


            return roots;

            static string? GroupKey(CommandDef commandDef, int pos)
                => commandDef.Path.Skip(pos).FirstOrDefault();

            static IEnumerable<CommandDefBase> ProcessRoot(int pos, IEnumerable<CommandDef> commandDefs, IEnumerable<CommandDef> roots, CommandDefBase root)
            {
                var subCommands = new List<CommandDefBase>();
                var remaining = commandDefs.Except(roots);
                if (remaining.Any())
                {
                    var groups = remaining.GroupBy(x => GroupKey(x, pos));
                    foreach (var group in groups)
                    {
                        var newSubCommands = group.TreeFromListInternal(pos + 1);
                        subCommands.AddRange(newSubCommands);
                    }
                }
                return subCommands;
            }
        }
    }
}
