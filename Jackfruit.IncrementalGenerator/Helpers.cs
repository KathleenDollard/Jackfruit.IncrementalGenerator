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
        internal static CommandDef? BuildCommandDef(string[] path,
                                                    CommandDefNode? parent,
                                                    string methodName,
                                                    CommandDetails? commandDetails,
                                                    IEnumerable<MemberDef> ancestorMembers,
                                                    string triggerStyle)
        {
            var parentName = parent?.CommandDef.Name;
            var isParentRoot = string.IsNullOrWhiteSpace(parent?.CommandDef.Parent);

            var members = new List<MemberDef>();
            if (commandDetails == null)
            { return null; }

            foreach (var memberPair in commandDetails.MemberDetails)
            {
                if (memberPair.Key == CommandKey) { continue; }
                var memberDetail = memberPair.Value;

                var isOnRoot = ancestorMembers.Any(m => m.Name == memberDetail.Name);

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
                                memberDetail.Required,
                                isOnRoot),
                            MemberKind.Argument => new ArgumentDef(
                                memberDetail.Id,
                                memberDetail.Name,
                                memberDetail.Description,
                                memberDetail.TypeName,
                                memberDetail.Required,
                                isOnRoot),
                            MemberKind.Service => new ServiceDef(
                                memberDetail.Id,
                                memberDetail.Name,
                                memberDetail.Description,
                                memberDetail.TypeName,
                                isOnRoot),
                            _ => throw new InvalidOperationException("Unexpected member kind")
                        });

            }
            var commandDef = new CommandDef(commandDetails.Detail.Id,
                                            commandDetails.Detail.Name,
                                            string.Join("_", path),
                                            commandDetails.Namespace,
                                            path,
                                            parentName,
                                            isParentRoot,
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
