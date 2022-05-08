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
        internal const string CreateName = "Create";
        internal const string CliRoot = "CliRoot";
        internal const string Cli = "Cli";
        internal static string[] CreateSources = new string[] { CliRoot, Cli };
        internal const string NestedCommandsClassName = "Commands";
        internal static readonly string[] names = { AddCommandName };
        internal const string TriggerStyle = "TriggerStyle";
        internal static string GetStyle(CommandDef commandDef)
            => commandDef.GenerationStyleTags.TryGetValue(Helpers.TriggerStyle, out var style)
                 ? style.ToString()
                 : string.Empty;
        internal static string MethodFullName(IMethodSymbol method) 
            => $"{method.ContainingType.ToDisplayString()}.{method.Name}";

        internal static string? GetName(SyntaxNode expression)
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

        internal static string? GetCaller(SyntaxNode expression)
            => expression switch
            {
                MemberAccessExpressionSyntax memberAccess when expression.IsKind(SyntaxKind.SimpleMemberAccessExpression)
                    => memberAccess.Expression.ToString(),
                IdentifierNameSyntax identifier
                     => "",
                _ => null
            };

        internal static CommandDef? BuildCommandDef(string[] path,
                                                  string methodName,
                                                  CommandDetails? commandDetails,
                                                  string triggerStyle)
        {
            var members = new List<MemberDef>();
            if (commandDetails == null)
            { return null;  }
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

        internal static CommandDetails? GetDetails(IMethodSymbol methodSymbol)
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

    }
}
