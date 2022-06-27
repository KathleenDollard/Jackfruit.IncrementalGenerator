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
using Jackfruit.Internal;

namespace Jackfruit
{
    /// <summary>
    /// This is the main class for the Jackfruit generator. After you call the 
    /// Create command, the returned RootCommand will contain your CLI. If you 
    /// need multiple root commands in your application differentiate them with &gt;T&lt;
    /// </summary>
    public partial class RootCommand : RootCommand<RootCommand, RootCommand.Result>
    {
        public new static RootCommand Create(CommandNode rootNode)
        { 
            return (RootCommand)RootCommand<RootCommand, RootCommand.Result>.Create(rootNode);
        }
    }
}";
        public void Initialize(IncrementalGeneratorInitializationContext initContext)
        {
            // To be a partial, this must be in the same namespace and assembly as the generated part
            initContext.RegisterPostInitializationOutput(ctx => ctx.AddSource("Cli.partial.g.cs", cliClassCode));

            IncrementalValuesProvider<CommandDefNode> commandDefs = initContext.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => Generator.IsCliCreateInvocation(s),
                    transform: static (ctx, cancellationToken) => BuildModel.GetCommandDef(ctx, cancellationToken))
                .Where(static m => m is not null)!;

            initContext.RegisterSourceOutput(commandDefs,
                static (context, commandDef) => Generate(commandDef, context));
        }

        public static void Generate(CommandDefNode commandDefNode, SourceProductionContext context)
        {
            var commandDef = commandDefNode.CommandDef;
            var joinedPath = string.Join("", commandDef.Path);

            var writer = new StringBuilderWriter(3);
            writer.AddLine("/*");
            OutputCommand(writer, commandDefNode);
            writer.AddLine("*/");

            context.AddSource($"{joinedPath}{commandDef.Name}.g.cs", writer.Output());

            static void OutputCommand(IWriter writer, CommandDefNode commandDefNode)
            {
                var commandDef = commandDefNode.CommandDef;
                var joinedPath = string.Join("", commandDef.Path);
                var path = string.Join(".", commandDef.Path);
                writer.AddLine($"//Key:         {joinedPath}");
                writer.AddLine($"//Id:          {commandDef.Id}");
                writer.AddLine($"//Handler:     {commandDef.HandlerMethodName}");
                writer.AddLine($"//Path:        {path}");
                writer.AddLine($"//Parent:      {commandDef.Parent}");
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
                foreach (var subCommandDef in commandDefNode.SubCommandNodes)
                {
                    if (subCommandDef is CommandDefNode subCommand)
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
