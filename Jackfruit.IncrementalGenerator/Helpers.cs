using Jackfruit.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using System.Xml.Linq;
using static Jackfruit.IncrementalGenerator.RoslynHelpers;

namespace Jackfruit.IncrementalGenerator
{
    public static class Helpers
    {
        internal const string CliRootName = "CliRoot";
        internal const string AddCommandName = "AddCommand";
        //private const string AddCommandsName = "AddCommands";
        private const string CreateName = "Create";
        private const string CliRoot = "CliRoot";
        internal const string Cli = "Cli";
        private static string[] CreateSources = new string[] { CliRoot, Cli };
        private const string NestedCommandsClassName = "Commands";
        //internal const string AddRootCommand = "SetRootCommand";
        private static readonly string[] names = { AddCommandName };
        internal const string TriggerStyle = "TriggerStyle";
        internal static string GetStyle(CommandDef commandDef)
            => commandDef.GenerationStyleTags.TryGetValue(Helpers.TriggerStyle, out var style)
                 ? style.ToString()
                 : string.Empty;
        private static string MethodFullName(IMethodSymbol method) 
            => $"{method.ContainingType.ToDisplayString()}.{method.Name}";

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
                var name = GetName(invocation.Expression);
                return name == null
                    ? false
                    : name == AddCommandName && argCount == 1
                        ? true
                        : name == CreateName
                            ? argCount == 1 && new string[] { Cli, CliRoot }.Contains( GetCaller(invocation.Expression) )
                            : false;
            }
            return false;

        }

        private static string? GetName(SyntaxNode expression)
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

        private static string? GetCaller(SyntaxNode expression)
            => expression switch
            {
                MemberAccessExpressionSyntax memberAccess when expression.IsKind(SyntaxKind.SimpleMemberAccessExpression)
                    => memberAccess.Expression.ToString(),
                IdentifierNameSyntax identifier
                     => "",
                _ => null
            };

        private static string GetPath(SyntaxNode expression)
        {
            var name = GetName(expression);
            var path = expression switch
            {
                MemberAccessExpressionSyntax memberAccess when expression.IsKind(SyntaxKind.SimpleMemberAccessExpression)
                    => name == CreateName
                        ? ""
                        : name != AddCommandName
                            ? ""
                            : memberAccess.Name is GenericNameSyntax genericName
                                ? $"{CliRoot}.{PathFromGenericTypes(genericName.TypeArgumentList.Arguments.First())}"
                                : CliRoot,

                IdentifierNameSyntax identifier
                     => "",
                _ => ""
            };

            return path;

            static string PathFromGenericTypes(TypeSyntax type)
            {
                var typeName = type.ToString();
                return typeName.StartsWith(NestedCommandsClassName)
                                ? typeName.Substring(NestedCommandsClassName.Length)
                                : typeName;
            }
        }

        public static CommandDef? GetCommandDef(GeneratorSyntaxContext context)
            => context.Node is not InvocationExpressionSyntax invocation
                // Weird, but we do not want to throw
                ? null
                : GetCaller(invocation.Expression) == Cli
                    ? GetCommandDefTreeApproach(invocation, context.SemanticModel)
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

            var commandDetails = GetDetails(methodSymbol);
            if (commandDetails is null)
            { return null; }

            var commandDef = BuildCommandDef(path, MethodFullName(methodSymbol), commandDetails, CliRoot);
            return commandDef;
        }

        private static CommandDef BuildCommandDef(string[] path,
                                                  string methodName,
                                                  CommandDetails? commandDetails,
                                                  string triggerStyle)
        {
            var members = new List<MemberDef>();
            foreach (var memberPair in commandDetails.MemberDetails)
            {
                if (memberPair.Key == CommandKey) { continue; }
                var memberDetail = memberPair.Value;
                members.Add(
                        memberDetail.MemberKind switch
                        {
                            MemberKind.Option => new OptionDef(
                                memberDetail.Id,
                                memberDetail.Name,
                                memberDetail.Description,
                                memberDetail.TypeName,
                                memberDetail.Aliases,
                                memberDetail.ArgDisplayName,
                                memberDetail.Required),
                            MemberKind.Argument => new ArgumentDef(
                                memberDetail.Id,
                                memberDetail.Name,
                                memberDetail.Description,
                                memberDetail.TypeName,
                                memberDetail.Required),
                            MemberKind.Service => new ServiceDef(
                                memberDetail.Id,
                                memberDetail.Name,
                                memberDetail.Description,
                                memberDetail.TypeName),
                            _ => throw new InvalidOperationException("Unexpected member kind")
                        });

            }
            var commandDef = new CommandDef(commandDetails.Detail.Id,
                                            commandDetails.Detail.Name,
                                            string.Join("_", path),
                                            commandDetails.Namespace,
                                            path,
                                            commandDetails.Detail.Description,
                                            commandDetails.Detail.Aliases,
                                            members,
                                            methodName,
                                            new CommandDef[] { },
                                            commandDetails.Detail.TypeName ?? "Unknown");
            commandDef.GenerationStyleTags.Add("TriggerStyle", triggerStyle);
            return commandDef;

        }

        private static CommandDetails? GetDetails(IMethodSymbol methodSymbol)
        {

            var commandDetails = methodSymbol.BasicDetails();
            var xmlComment = (methodSymbol.GetDocumentationCommentXml());
            if (!string.IsNullOrWhiteSpace(xmlComment))
            {
                var xDoc = XDocument.Parse(xmlComment);
                AddDescFromXmlDocComment(xDoc, commandDetails.Detail);
                AddDescFromXmlDocComment(xDoc, commandDetails.MemberDetails);
            }
            AddDetailsFromAttributes(methodSymbol, commandDetails.Detail, commandDetails.MemberDetails);
            return commandDetails;
        }


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

            var commandDetails = GetDetails(methodSymbol);
            if (commandDetails is null)
            { return null; }

            var commandDef =  BuildCommandDef(path, MethodFullName(methodSymbol), commandDetails, Cli);
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
