using Jackfruit.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Xml.Linq;
using static Jackfruit.IncrementalGenerator.RoslynHelpers;

namespace Jackfruit.IncrementalGenerator
{
    public static class Helpers
    {
        internal const string ConsoleAppClassName = "ConsoleApplication";
        internal const string AddSubCommand = "AddSubCommand";
        internal const string CreateWithRootCommand = "CreateWithRootCommand";
        private static readonly string[] names = { AddSubCommand, CreateWithRootCommand };

        public const string ConsoleClass = @"
using System;

namespace Jackfruit
{
    public class ConsoleApplication
    {
        public static ConsoleApplication CreateWithRootCommand(Delegate rootCommandHandler) { }
    }

    public class CliCommand
    {
        public static CliCommand AddCommand(Delegate CommandHandler)
    }
}
";

        public static bool IsSyntaxInteresting(SyntaxNode node)
        {
            // Select1:
            //      * Extract all method invocations and filter by:
            //      * Name: comparing with expected list
            //      * Parameter count of 1
            if (node is InvocationExpressionSyntax invocation)
            {
                if (invocation.ArgumentList.Arguments.Count != 1)
                { return false; }
                var name = GetName(invocation.Expression);
                return name is not null && names.Contains(name);
            }
            else
            { return false; }

        }

        private static string? GetName(SyntaxNode expression)
        {
            var name = expression switch
            {
                MemberAccessExpressionSyntax memberAccess when expression.IsKind(SyntaxKind.SimpleMemberAccessExpression)
                    => memberAccess.Name.ToString(),
                IdentifierNameSyntax identifier
                     => identifier.ToString(),
                _ => null
            };

            return name;
        }

        private static string GetPath(SyntaxNode expression)
        {
            var path = expression switch
            {
                MemberAccessExpressionSyntax memberAccess when expression.IsKind(SyntaxKind.SimpleMemberAccessExpression)
                    => memberAccess.Expression.ToString(),
                IdentifierNameSyntax identifier
                     => "",
                _ => ""
            };

            return path.Replace(ConsoleAppClassName, "");
        }

        public static CommandDef? GetCommandDef(GeneratorSyntaxContext context)
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
            var path = GetPath(invocation.Expression).Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            var delegateArg = invocation.ArgumentList.Arguments[0].Expression;
            var methodSymbol = MethodOrCandidateSymbol(context.SemanticModel, delegateArg);
            if (methodSymbol is null) { return null; }
            var nspace = methodSymbol.ContainingNamespace.ToString();
            if (methodSymbol is null) { return null; }

            var (commandDetail, memberDetails) = methodSymbol.BasicDetails();
            var xmlComment = (methodSymbol.GetDocumentationCommentXml());
            if (!string.IsNullOrWhiteSpace(xmlComment))
            {
                var xDoc = XDocument.Parse(xmlComment);
                AddDescFromXmlDocComment(xDoc, commandDetail);
                AddDescFromXmlDocComment(xDoc, memberDetails);
            }
            AddDetailsFromAttributes(methodSymbol, memberDetails);


            var members = new List<MemberDef>();
            foreach (var memberPair in memberDetails)
            {
                if (memberPair.Key == CommandKey) { continue; }
                var memberDetail = memberPair.Value;
                members.Add(
                        memberDetail.MemberKind switch
                        {
                            MemberKind.Option => new OptionDef(
                                memberDetail.Id,
                                memberDetail.Description,
                                memberDetail.TypeName,
                                memberDetail.Aliases,
                                memberDetail.ArgDisplayName,
                                memberDetail.Required),
                            MemberKind.Argument => new ArgumentDef(
                                memberDetail.Id,
                                memberDetail.Description,
                                memberDetail.TypeName,
                                memberDetail.Required),
                            MemberKind.Service => new ServiceDef(
                                memberDetail.Id,
                                memberDetail.Description,
                                memberDetail.TypeName),
                            _ => throw new InvalidOperationException("Unexpected member kind")
                        });

            }
            return new CommandDef(
                delegateArg.ToString(),
                string.Join("_", path),
                nspace,
                path,
                commandDetail.Description,
                commandDetail.Aliases,
                members,
                delegateArg.ToString(),
                new CommandDef[] { },
                commandDetail.ReturnType ?? "Unknown");
        }

        public static CommandDefBase TreeFromList(this IEnumerable<CommandDef> commandDefs, int pos)
        {
            if (pos > 10) { throw new InvalidOperationException("Runaway recursion suspected"); }
            // This throws on badly formed trees. not sure whether to just let that happen and catch, or do more work here
            var roots = commandDefs.Where(x => GroupKey(x, pos) is null);
            var root = roots.FirstOrDefault();
            if (root is null) { return new EmptyCommandDef(); }

            var remaining = commandDefs.Except(roots);
            if (remaining.Any())
            {
                var groups = remaining.GroupBy(x => GroupKey(x, pos));
                var subCommands = new List<CommandDefBase>();
                foreach (var group in groups)
                {
                    var subCommand = group.TreeFromList(pos + 1);
                    subCommands.Add(subCommand);
                }
                root.SubCommands = subCommands;
            }
            return root;

            static string? GroupKey(CommandDef commandDef, int pos)
                => commandDef.Path.Skip(pos).FirstOrDefault();
        }
    }
}
