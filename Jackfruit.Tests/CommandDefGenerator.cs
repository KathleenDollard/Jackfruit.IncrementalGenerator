using Microsoft.CodeAnalysis;
using Jackfruit.Common;
using Jackfruit.IncrementalGenerator;
using Jackfruit.IncrementalGenerator.Output;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace Jackfruit.Tests
{
    [Generator]
    public class CommandDefGenerator : IIncrementalGenerator
    {
        private const string cliClassCode = @"
using Jackfruit;

public partial class Cli
{
    public static void Create(CliNode cliRoot)
    { }
}";
        public void Initialize(IncrementalGeneratorInitializationContext initContext)
        {
            // To be a partial, this must be in the same namespace and assembly as the generated part
            initContext.RegisterPostInitializationOutput(ctx => ctx.AddSource("Cli.partial.g.cs", cliClassCode));

            IncrementalValuesProvider<CommandDef> commandDefs = initContext.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => Generator.IsCliCreateInvocation(s),
                    transform: static (ctx, cancellationToken) => CliExtractAndBuild.GetCommandDef(ctx, cancellationToken))
                .Where(static m => m is not null)!;

            initContext.RegisterSourceOutput(commandDefs,
                static (context, commandDef) => Generate(commandDef, context));
        }

        public static void Generate(CommandDef commandDef, SourceProductionContext context)
        {
            var joinedPath = string.Join("", commandDef.Path);

            var writer = new StringBuilderWriter(3);
            writer.AddLine("/*");
            OutputCommand(writer, commandDef);
            writer.AddLine("*/");

            context.AddSource($"{joinedPath}{commandDef.Name}.g.cs", writer.Output());

            static void OutputCommand(IWriter writer, CommandDef commandDef)
            {
                var joinedPath = string.Join("", commandDef.Path);
                var path = string.Join(".", commandDef.Path);
                writer.AddLine($"//Key:         {joinedPath}");
                writer.AddLine($"//Id:          {commandDef.Id}");
                writer.AddLine($"//Handler:     {commandDef.HandlerMethodName}");
                writer.AddLine($"//Path:        {path}");
                writer.AddLine($"//Parent:      {commandDef.Parent?.Name}");
                if (commandDef.Validator is not null)
                {
                    writer.AddLine("//Validator:");
                    writer.AddLine($"//   Namespace:   {commandDef.Validator.Namespace}");
                    writer.AddLine($"//   MethodName:  {commandDef.Validator.MethodName}");
                    writer.AddLine($"//   Members:     {string.Join(", ", commandDef.Validator.Members.Select(x=>x.Id))}");
                }
                writer.AddLine($"//Description: {commandDef.Description}");
                writer.AddLine($"//Aliases:     {string.Join(", ", commandDef.Aliases)}");
                writer.AddLine($"//Namespace:   {commandDef.Namespace}");
                writer.AddLine($"//Members:     ");
                writer.IncreaseIndent();
                foreach (var member in commandDef.Members)
                {
                    switch (member)
                    {
                        case OptionDef option: OutputOption(writer, option); break;
                        case ArgumentDef arg: OutputArgument(writer, arg); break;
                        case ServiceDef service: OutputService(writer, service); break;
                    }
                }
                writer.DecreaseIndent();
                writer.AddLine($"//SubCommands:     ");
                writer.IncreaseIndent();
                foreach (var subCommandDef in commandDef.SubCommands)
                {
                    if (subCommandDef is CommandDef subCommand)
                    {
                        OutputCommand(writer, subCommand);
                    }
                }
                writer.DecreaseIndent();
                writer.AddLine($"//**************************************    ");
                writer.AddLine("");

            }

            static void OutputOption(IWriter writer, OptionDef option)
            {
                writer.AddLine("//Option:");
                writer.IncreaseIndent();
                writer.AddLine($"//Option Id:      {option.Id}");
                writer.AddLine($"//Name:           {option.Name}");
                writer.AddLine($"//TypeName:       {option.TypeName}");
                writer.AddLine($"//Description:    {option.Description}");
                writer.AddLine($"//Aliases:        {string.Join(", ", option.Aliases)}");
                writer.AddLine($"//ArgDisplayName: {option.ArgDisplayName}");
                writer.AddLine($"//Required:       {option.Required}");
                writer.DecreaseIndent();
            }

            static void OutputArgument(IWriter writer, ArgumentDef option)
            {
                writer.AddLine("//Argument:");
                writer.IncreaseIndent();
                writer.AddLine($"//Argumet Id:     {option.Id}");
                writer.AddLine($"//Name:           {option.Name}");
                writer.AddLine($"//TypeName:       {option.TypeName}");
                writer.AddLine($"//Description:    {option.Description}");
                writer.AddLine($"//Required:       {option.Required}");
                writer.DecreaseIndent();
            }

            static void OutputService(IWriter writer, ServiceDef option)
            {
                writer.AddLine("//Service:");
                writer.IncreaseIndent();
                writer.AddLine($"//Service Id:     {option.Id}");
                writer.AddLine($"//Name:           {option.Name}");
                writer.AddLine($"//TypeName:       {option.TypeName}");
                writer.AddLine($"//Description:    {option.Description}");
                writer.DecreaseIndent();
            }
        }

    }
}
