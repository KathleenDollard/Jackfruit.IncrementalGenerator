using Microsoft.CodeAnalysis;

namespace Jackfruit.Common
{
    public class CommonHelpers
    {
        public const string CliRootName = "CliRoot";
        public const string RootCommand = "RootCommand";
        public const string CreateName = "Create";
        public const string AddCommandName = "Create";
        public const string CliRoot = "CliRoot";
        public static string[] CreateSources = new string[] { CliRoot, RootCommand };
        public const string NestedCommandsClassName = "Commands";
        public static readonly string[] names = { AddCommandName };
        public const string TriggerStyle = "TriggerStyle";
        public static string GetStyle(CommandDef commandDef)
            => commandDef.GenerationStyleTags.TryGetValue(CommonHelpers.TriggerStyle, out var style)
                 ? style.ToString()
                 : string.Empty;
        public static string MethodFullName(IMethodSymbol method)
            => $"{method.ContainingType.ToDisplayString()}.{method.Name}";
    }
}
