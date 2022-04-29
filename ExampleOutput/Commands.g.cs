// This file is created by a generator.
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Security.Cryptography.X509Certificates;
using DemoHandlers;

namespace Jackfruit
{


    public partial class StarTrekCommand : GeneratedCommandBase, ICommandHandler
    {

        private StarTrekCommand() : base("rootCommand")
        {
        }

        public void AddCommand(Delegate handler)
        { }

        public static StarTrekCommand Create()
        {
            var command = new StarTrekCommand();
            command.greetingArgument = new Argument<string>("greetingArg");
            command.Add(command.greetingArgument);
            command.kirkOption = new Option<bool>("--kirk");
            command.kirkOption.Description = "Whether to greet Captain Kirk";
            command.Add(command.kirkOption);
            command.spockOption = new Option<bool>("--spock");
            command.spockOption.Description = "Whether to greet Spock";
            command.Add(command.spockOption);
            command.uhuraOption = new Option<bool>("--uhura");
            command.uhuraOption.Description = "Whether to greet Lieutenant Uhura";
            command.Add(command.uhuraOption);
            command.NextGenerationCommand = NextGenerationCommand.Create();
            command.Add(command.NextGenerationCommand.Command);
            command.Handler = command;
            return command;
        }

        public Task<int> InvokeAsync(InvocationContext context)
        {
            Handlers.StarTrek(greetingArgumentResult(context), kirkOptionResult(context), spockOptionResult(context), uhuraOptionResult(context));
            return Task.FromResult(context.ExitCode);
        }

        public int Invoke(InvocationContext context)
        {
            Handlers.StarTrek(greetingArgumentResult(context), kirkOptionResult(context), spockOptionResult(context), uhuraOptionResult(context));
            return context.ExitCode;
        }

        public Argument<string> greetingArgument { get; set; }
        public string greetingArgumentResult(InvocationContext context)
        {
            return context.ParseResult.GetValueForArgument<string>(greetingArgument);
        }

        public Option<bool> kirkOption { get; set; }
        public bool kirkOptionResult(InvocationContext context)
        {
            return context.ParseResult.GetValueForOption<bool>(kirkOption);
        }

        public Option<bool> spockOption { get; set; }
        public bool spockOptionResult(InvocationContext context)
        {
            return context.ParseResult.GetValueForOption<bool>(spockOption);
        }

        public Option<bool> uhuraOption { get; set; }
        public bool uhuraOptionResult(InvocationContext context)
        {
            return context.ParseResult.GetValueForOption<bool>(uhuraOption);
        }

        public NextGenerationCommand NextGenerationCommand { get; set; }
    }

    public partial class NextGenerationCommand : GeneratedCommandBase, ICommandHandler
    {
        private NextGenerationCommand() : base("NextGeneration")
        {
        }
        public void AddCommand(Delegate handler)
        { }

        // asdf.SetValidator(Delegate x)
        public void Validate(string picard)
        { }

        public static NextGenerationCommand Create()
        {
            var command = new NextGenerationCommand();
            command.greetingArgument = new Argument<string>("GREETING");
            command.Add(command.greetingArgument);
            command.PicardOption = new Option<bool>("--picard");
            command.PicardOption.Description = "This is the description for Picard";
            command.Add(command.PicardOption);
            command.DeepSpaceNine = DeepSpaceNineCommand.Create();
            command.Add(command.DeepSpaceNine.Command);
            command.Handler = command;
            return command;
        }

        public Task<int> InvokeAsync(InvocationContext context)
        {
            Handlers.NextGeneration(greetingArgumentResult(context), PicardOptionResult(context));
            return Task.FromResult(context.ExitCode);
        }


        public int Invoke(InvocationContext context)
        {
            Handlers.NextGeneration(greetingArgumentResult(context), PicardOptionResult(context));
            return context.ExitCode;
        }

        public Argument<string> greetingArgument { get; set; }
        public string greetingArgumentResult(InvocationContext context)
        {
            return context.ParseResult.GetValueForArgument<string>(greetingArgument);
        }

        public Option<bool> PicardOption { get; private set; }
        public bool PicardOptionResult(InvocationContext context)
        {
            return context.ParseResult.GetValueForOption<bool>(PicardOption);
        }

        public DeepSpaceNineCommand DeepSpaceNine { get; private set; }
        public VoyagerCommand Voyager { get; private set; }
    }

    public partial class DeepSpaceNineCommand : GeneratedCommandBase, ICommandHandler
    {
        private DeepSpaceNineCommand() : base("DeepSpaceNine")
        {
        }

        public void AddCommand(Delegate handler)
        { }

        public static DeepSpaceNineCommand Create()
        {
            var command = new DeepSpaceNineCommand();
            command.greetingOption = new Option<string>("--greeting");
            command.Add(command.greetingOption);
            command.siskoOption = new Option<bool>("--sisko");
            command.Add(command.siskoOption);
            command.odoOption = new Option<bool>("--odo");
            command.Add(command.odoOption);
            command.daxOption = new Option<bool>("--dax");
            command.Add(command.daxOption);
            command.worfOption = new Option<bool>("--worf");
            command.Add(command.worfOption);
            command.oBrienOption = new Option<bool>("--oBrien");
            command.Add(command.oBrienOption);
            command.Handler = command;
            return command;
        }

        public Task<int> InvokeAsync(InvocationContext context)
        {
            Handlers.DeepSpaceNine(greetingOptionResult(context), siskoOptionResult(context), odoOptionResult(context), daxOptionResult(context), worfOptionResult(context), oBrienOptionResult(context));
            return Task.FromResult(context.ExitCode);
        }

        public Option<string> greetingOption { get; set; }
        public string greetingOptionResult(InvocationContext context)
        {
            return context.ParseResult.GetValueForOption<string>(greetingOption);
        }

        public Option<bool> siskoOption { get; set; }
        public bool siskoOptionResult(InvocationContext context)
        {
            return context.ParseResult.GetValueForOption<bool>(siskoOption);
        }

        public Option<bool> odoOption { get; set; }
        public bool odoOptionResult(InvocationContext context)
        {
            return context.ParseResult.GetValueForOption<bool>(odoOption);
        }

        public Option<bool> daxOption { get; set; }
        public bool daxOptionResult(InvocationContext context)
        {
            return context.ParseResult.GetValueForOption<bool>(daxOption);
        }

        public Option<bool> worfOption { get; set; }
        public bool worfOptionResult(InvocationContext context)
        {
            return context.ParseResult.GetValueForOption<bool>(worfOption);
        }

        public Option<bool> oBrienOption { get; set; }
        public bool oBrienOptionResult(InvocationContext context)
        {
            return context.ParseResult.GetValueForOption<bool>(oBrienOption);
        }

    }

    public partial class VoyagerCommand : GeneratedCommandBase, ICommandHandler
    {
        private VoyagerCommand() : base("Voyager")
        {
        }

        public void AddCommand(Delegate handler)
        { }

        public static VoyagerCommand Create()
        {
            var command = new VoyagerCommand();
            command.greetingOption = new Option<string>("--greeting");
            command.Add(command.greetingOption);
            command.janewayOption = new Option<bool>("--janeway");
            command.Add(command.janewayOption);
            command.chakotayOption = new Option<bool>("--chakotay");
            command.Add(command.chakotayOption);
            command.torresOption = new Option<bool>("--torres");
            command.Add(command.torresOption);
            command.tuvokOption = new Option<bool>("--tuvok");
            command.Add(command.tuvokOption);
            command.sevenOfNineOption = new Option<bool>("--sevenOfNine");
            command.Add(command.sevenOfNineOption);
            command.Handler = command;
            return command;
        }

        public Task<int> InvokeAsync(InvocationContext context)
        {
            Handlers.Voyager(greetingOptionResult(context), janewayOptionResult(context), chakotayOptionResult(context), torresOptionResult(context), tuvokOptionResult(context), sevenOfNineOptionResult(context));
            return Task.FromResult(context.ExitCode);
        }

        public Option<string> greetingOption { get; set; }
        public string greetingOptionResult(InvocationContext context)
        {
            return context.ParseResult.GetValueForOption<string>(greetingOption);
        }

        public Option<bool> janewayOption { get; set; }
        public bool janewayOptionResult(InvocationContext context)
        {
            return context.ParseResult.GetValueForOption<bool>(janewayOption);
        }

        public Option<bool> chakotayOption { get; set; }
        public bool chakotayOptionResult(InvocationContext context)
        {
            return context.ParseResult.GetValueForOption<bool>(chakotayOption);
        }

        public Option<bool> torresOption { get; set; }
        public bool torresOptionResult(InvocationContext context)
        {
            return context.ParseResult.GetValueForOption<bool>(torresOption);
        }

        public Option<bool> tuvokOption { get; set; }
        public bool tuvokOptionResult(InvocationContext context)
        {
            return context.ParseResult.GetValueForOption<bool>(tuvokOption);
        }

        public Option<bool> sevenOfNineOption { get; set; }
        public bool sevenOfNineOptionResult(InvocationContext context)
        {
            return context.ParseResult.GetValueForOption<bool>(sevenOfNineOption);
        }

    }

}


