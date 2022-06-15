using Microsoft.CodeAnalysis;

namespace Jackfruit.Common
{
    public class CommonHelpers
    {
        public const string CliRootName = "CliRoot";
        public const string AddCommandName = "AddCommand";
        public const string CreateName = "Create";
        public const string CliRoot = "CliRoot";
        public const string Cli = "Cli";
        public static string[] CreateSources = new string[] { CliRoot, Cli };
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
