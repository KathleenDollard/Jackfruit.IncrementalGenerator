using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Jackfruit.Models;
using Jackfruit.IncrementalGenerator;
using Newtonsoft.Json;

namespace Jackfruit.Tests
{
    [Generator]
    public class CommandDefGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext initContext)
        {
            initContext.RegisterPostInitializationOutput(ctx => ctx.AddSource(
                "ConsoleApplication.g.cs",
                SourceText.From(Helpers.ConsoleClass, Encoding.UTF8)));

            IncrementalValuesProvider<CommandDef> commandDefs = initContext.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => Helpers.IsSyntaxInteresting(s),
                    transform: static (ctx, _) => Helpers.GetCommandDef(ctx))
                .Where(static m => m is not null)!;

            initContext.RegisterSourceOutput(commandDefs,
                static (context, commandDef) => Generate(commandDef, context));
        }

        private static void Generate(CommandDef commandDef, SourceProductionContext context)
        {
            var joinedPath = string.Join("", commandDef.Path);
            var sb = new StringBuilder();
            sb.AppendLine($@"// 
Key:         {joinedPath}
Id:          {commandDef.Id}
Path:        {string.Join(".", commandDef.Path)}
Description: {commandDef.Description}
Members:     "); 
            foreach (var member in commandDef.Members)
            {
                switch (member)
                {
                    case OptionDef option:  sb.AppendLine(OutputOption(option));break;
                    case ArgumentDef arg:   sb.AppendLine(OutputArgument(arg)); break;
                    case ServiceDef service:sb.AppendLine(OutputService(service)); break;
                }

            }
            context.AddSource($"{joinedPath}.g.cs", sb.ToString());

            static string OutputOption(OptionDef option)
            => $@"
         Option Id:      {option.Id}
         Name:           {option.Name}
         TypeName:       {option.TypeName}
         Description:    {option.Description}
         Aliases:        {string.Join(", ", option.Aliases)}
         ArgDisplayName: {option.ArgDisplayName}
         Required:       {option.Required}";

            static string OutputArgument(ArgumentDef option)
            => $@"
         Argumet Id:     {option.Id}
         Name:           {option.Name}
         TypeName:       {option.TypeName}
         Description:    {option.Description}
         Required:       {option.Required}";

            static string OutputService(ServiceDef option)
            => $@"
         Service Id:     {option.Id}
         Name:           {option.Name}
         TypeName:       {option.TypeName}
         Description:    {option.Description}";
        }

    }
}
