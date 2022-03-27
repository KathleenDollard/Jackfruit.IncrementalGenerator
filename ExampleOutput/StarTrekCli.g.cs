

//// *******************************

// This file is created by a generator.
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;

namespace DemoHandlers
{
    public partial class NextGenerationCommand : RootCommand, ICommandHandler
    {
        private NextGenerationCommand()
        {
        }

        public static NextGenerationCommand Create()
        {
            var command = new NextGenerationCommand();
            command.greetingArgument = new Argument<string>("greetingArg");
            command.Add(command.greetingArgument);
            command.picardOption = new Option<bool>("picard");
            command.picardOption.Description = "This is the description for Picard";
            command.Add(command.picardOption);
            command.Handler = command;
            return command;
        }

        public Task<int> InvokeAsync(InvocationContext context)
        {
         .Handlers.NextGeneration(.greetingArgumentResult, Jackfruit.IncrementalGenerator.CodeModels.SymbolModel(), .picardOptionResult, Jackfruit.IncrementalGenerator.CodeModels.SymbolModel());
            return Task.FromResult(context.Exitcode);
        }

        public Argument<string> greetingArgument { get; set; }
        public string greetingArgumentResult(InvocationContext context)
        {
            return context.ParseResult.GetValueForArgument<string>(greetingArgument);
        }

        public Option<bool> picardOption { get; set; }
        public bool picardOptionResult(InvocationContext context)
        {
            return context.ParseResult.GetValueForOption<bool>(picardOption);
        }

    }

}