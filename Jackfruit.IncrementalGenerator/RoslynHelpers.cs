using Microsoft.CodeAnalysis;
using System.Xml.Linq;

namespace Jackfruit.IncrementalGenerator
{
    internal static class RoslynHelpers
    {
        public enum MemberKind
        {
            Option = 0,
            Argument,
            Service
        }

        public class Detail
        {
            private string description = "";
            private MemberKind memberKind;
            private string typeName = "";
            private string argDisplayId = "";
            private bool required;

            public Detail(string id, string name, string? typeName = null)
            {
                Id = id;
                Name = name;
                TypeName = typeName;
            }

            public string Id { get; }
            public string Name { get; set; }
            public string Description
            {
                get => description;
                set
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        description = value;
                    }
                }
            }
            public string[] Aliases { get; set; } = new string[] { };
            public MemberKind MemberKind
            {
                get => memberKind;
                set
                {
                    if (value != MemberKind.Option)
                    {
                        memberKind = value;
                    }
                }
            }
            public string TypeName
            {
                get => typeName;
                set
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        typeName = value;
                    }
                }
            }
            public string ArgDisplayName
            {
                get => argDisplayId;
                set
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        argDisplayId = value;
                    }
                }
            }
            public bool Required
            {
                get => required;
                set
                {
                    if (value)
                    {
                        required = value;
                    }
                }
            }
        }

        public const string CommandKey = "__commandKey__";

        public static IMethodSymbol? MethodOrCandidateSymbol(SemanticModel semanticModel, SyntaxNode? expression)
        {
            if (expression == null) { return null; }
            var symbolInfo = semanticModel.GetSymbolInfo(expression);
            if (symbolInfo.Symbol is IMethodSymbol methodSymbol) { return methodSymbol; }
            return symbolInfo.CandidateSymbols.FirstOrDefault() is IMethodSymbol candidate
                    ? candidate
                    : null;
        }

        public static (Detail commandDetail, Dictionary<string, Detail> memberDetail) BasicDetails(this IMethodSymbol methodSymbol)
        {
            var commandDetail = new Detail(methodSymbol.ToDisplayString(), methodSymbol.Name, methodSymbol.ReturnType.ToString());

            var details = new Dictionary<string, Detail>();

            foreach (var param in methodSymbol.Parameters)
            {
                details[param.Name] = new Detail(param.Name, param.Name, param.Type.ToString());
                if (param.Name.EndsWith("Arg"))
                {
                    details[param.Name].MemberKind = MemberKind.Argument;
                    details[param.Name].Name = param.Name.Substring(0, param.Name.Length - 3);
                }
                else if (param.Type.IsAbstract)  // Test that this is true for interfaces
                {
                    details[param.Name].MemberKind = MemberKind.Service;
                }
            }
            return (commandDetail, details);
        }

        public static void AddDescFromXmlDocComment(XDocument xDoc, Dictionary<string, Detail> details)
        {
            foreach (var element in xDoc.Root.Elements("param"))
            {
                var paramName = element.Attribute("name");
                if (paramName is not null)
                {
                    if (details.TryGetValue(paramName.Value, out var paramDetail))
                    { paramDetail.Description = element.Value.Trim(); }
                }
            }
        }

        public static void AddDescFromXmlDocComment(XDocument xDoc, Detail commandDetail)
        {
            var summaryElement = xDoc.Root.Element("summary");
            commandDetail.Description =
                    summaryElement is null
                    ? commandDetail.Description
                    : summaryElement.Value.Trim();
        }

        public static Dictionary<string, Detail> AddDetailsFromAttributes(
            IMethodSymbol methodSymbol,
            Dictionary<string, Detail> details)
        {
            AddToDetail(methodSymbol.GetAttributes(), details, CommandKey);
            foreach (var param in methodSymbol.Parameters)
            {
                AddToDetail(param.GetAttributes(), details, param.Name);
            }

            return details;
        }

        public static void AddToDetail(
            IEnumerable<AttributeData> attributes,
            Dictionary<string, Detail> details,
            string key)
        {
            if (details.TryGetValue(key, out var detail))
            {
                foreach (var attrib in attributes)
                {
                    if (attrib.AttributeClass is null) { continue; }
                    switch (attrib.AttributeClass.Name)
                    {
                        case "DescriptionAttribute":
                            var arg = attrib.ConstructorArguments.FirstOrDefault();
                            detail.Description = arg.Value?.ToString() ?? "";
                            break;

                        case "AliasesAttribute":
                            var arg1 = attrib.ConstructorArguments.FirstOrDefault();
                            detail.Aliases = arg1.Values.Select(x => x.ToString()).ToArray();
                            break;

                        case "ArgumentAttribute":
                            detail.MemberKind = MemberKind.Argument;
                            break;

                        case "OptionArgumentNameAttribute":
                            var arg3 = attrib.ConstructorArguments.FirstOrDefault();
                            detail.ArgDisplayName = arg3.Value?.ToString() ?? "";
                            break;

                        case "RequiredAttribute":
                            detail.Required = true;
                            break;

                        default:
                            break;
                    }
                }
            }
        }
    }
}
