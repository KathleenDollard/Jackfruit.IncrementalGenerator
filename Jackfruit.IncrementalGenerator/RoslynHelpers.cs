using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Collections.Immutable;
using System.Reflection.Metadata;
using System.ComponentModel;

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

            public Detail(string id, string? returnType = null)
            {
                Id = id;
                ReturnType = returnType;
            }

            public string Id { get; }
            public string? ReturnType { get; } 
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
            if (symbolInfo.CandidateSymbols.FirstOrDefault() is IMethodSymbol candidate) { return candidate; }
            return null;
        }

        public static Dictionary<string, Detail> BasicDetails(this IMethodSymbol methodSymbol)
        {
            var details = new Dictionary<string, Detail>();
            details[CommandKey] = new Detail(methodSymbol.Name, methodSymbol.ReturnType.ToString());
            foreach (var param in methodSymbol.Parameters)
            {
                details[param.Name] = new Detail(param.Name);
                if (param.Name.EndsWith("Arg"))
                { details[param.Name].MemberKind = MemberKind.Argument; }
                else if (param.Type.IsAbstract)  // Test that this is true for interfaces
                { details[param.Name].MemberKind = MemberKind.Service; }
            }
            return details;
        }

        public static Dictionary<string, Detail> DescFromXmlDocComment(string? xmlComment, Dictionary<string, Detail> details)
        {
            if (string.IsNullOrWhiteSpace(xmlComment)) { return new Dictionary<string, Detail>(); }
            var xDoc = XDocument.Parse(xmlComment);
            var summaryElement = xDoc.Root.Element("summary");
            var commandDetail = GetOrCreate(CommandKey, details);
            commandDetail.Description =
                    summaryElement is null
                    ? ""
                    : summaryElement.Value.Trim();

            foreach (var element in xDoc.Root.Elements("param"))
            {
                var paramName = element.Attribute("name");
                if (paramName is not null)
                {
                    var paramDetail = GetOrCreate(paramName.Value, details);
                    paramDetail.Description = element.Value.Trim();
                }
            }
            return details;
        }

        private static Detail GetOrCreate(string key, Dictionary<string, Detail> details)
        {
            if (!details.TryGetValue(key, out var detail))
            {
                detail = new Detail(key);
                details[key] = detail;
            }
            return detail;
        }
        public static Dictionary<string, Detail> DescFromAttributes(
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
            var detail = GetOrCreate(key, details);
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
