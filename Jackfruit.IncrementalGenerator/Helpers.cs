using Microsoft.CodeAnalysis;
using Jackfruit.Common;
using Microsoft.CodeAnalysis.Operations;
using System.Xml.Linq;
using static Jackfruit.IncrementalGenerator.RoslynHelpers;
using System.Linq;

namespace Jackfruit.IncrementalGenerator
{
    public static class Helpers
    {
        //internal const string CliRootName = "CliRoot";
        //internal const string AddCommandName = "AddCommand";
        //internal const string CreateName = "Create";
        //internal const string CliRoot = "CliRoot";
        //internal const string Cli = "Cli";
        //internal static string[] CreateSources = new string[] { CliRoot, Cli };
        //internal const string NestedCommandsClassName = "Commands";
        //internal static readonly string[] names = { AddCommandName };
        //internal const string TriggerStyle = "TriggerStyle";
        //internal static string GetStyle(CommandDef commandDef)
        //    => commandDef.GenerationStyleTags.TryGetValue(Helpers.TriggerStyle, out var style)
        //         ? style.ToString()
        //         : string.Empty;
        //internal static string MethodFullName(IMethodSymbol method)
        //    => $"{method.ContainingType.ToDisplayString()}.{method.Name}";



        internal static CommandDef? BuildCommandDef(string[] path,
                                                    string? parent,
                                                    string methodName,
                                                    CommandDetails? commandDetails,
                                                    string triggerStyle)
        {
            var members = new List<MemberDef>();
            if (commandDetails == null)
            { return null; }
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
                                            parent,
                                            commandDetails.Detail.Description,
                                            commandDetails.Detail.Aliases,
                                            members,
                                            methodName,
                                            new string[] { },
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

        internal static ValidatorDef? GetValidatorDef(IMethodSymbol? validatorSymbol, CommandDef commandDef)
        {
            if (validatorSymbol is null)
            { return null; }
            var details = validatorSymbol.BasicDetails();
            // This is N of M, but expect the number of members to be small
            var members = new List<MemberDef>();

            foreach (var member in details.MemberDetails)
            {
                var match = LookupMember(member.Key, commandDef.Members);
                if (match is null)
                {
                    // TODO: This needs to be a diagnostic, so shows where we need to pipe diagnostics
                    match = new UnknownMemberDef(member.Key);
                }
                members.Add(match);
                // Normal type mismatch exception expected to be adequate if types do not match
            }
            var methodName = CommonHelpers.MethodFullName(validatorSymbol);
            return new ValidatorDef(methodName, details.Namespace, members);

            static MemberDef? LookupMember(string name, IEnumerable<MemberDef> members)
            {
                var match = members.FirstOrDefault(m => m.Id == name);
                if (match is null && !name.EndsWith("Arg"))
                {
                    var test = $"{name}Arg";
                    match = members.FirstOrDefault(m => m.Id == test);
                }
                return match;
            }
        }
    }
}
