// This file is created by a generator.
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using DemoHandlersUpdated;

namespace Jackfruit
{
    public partial class FranshiseCommand : GeneratedCommandBase<FranshiseCommand.Result>
    {
        private FranshiseCommand() : base("rootCommand")
        {
        }

        public static FranshiseCommand Create()
        {
            var command = new FranshiseCommand();
            command.GreetingArgument = new Argument<string>("greetingArg");
            command.Add(command.GreetingArgument);
            command.StarTrekCommand = StarTrekCommand.Create(command);
            command.Add(command.StarTrekCommand.SystemCommandLineCommand);
            return command;
        }

        public struct Result
        {
            internal Result(FranshiseCommand command, ParseResult parseResult)
            {
                Greeting = parseResult.GetValueForArgument(command.GreetingArgument);
            }
            public string Greeting { get; }
        }

        public override Result GetResult(ParseResult parseResult) => new Result(this, parseResult);

        public Argument<string> GreetingArgument { get; private set; }

        public StarTrekCommand StarTrekCommand { get; private set; }

        public override string Validate(ParseResult parseResult)
        {
            var result = new Result(this, parseResult);
            var messages = new List<string>();
            AddMessageOnFail(messages, Validators.ValidatePoliteness(result.Greeting));
            return String.Join(Environment.NewLine, messages);
        }
    }

    public partial class StarTrekCommand : GeneratedCommandBase<StarTrekCommand.Result, FranshiseCommand>, ICommandHandler
    {
        private StarTrekCommand() : base("StarTrek")
        {
        }

        internal static StarTrekCommand Create(FranshiseCommand parent)
        {
            var command = new StarTrekCommand();
            command.parent = parent;
            command.KirkOption = new Option<bool>("--kirk");
            command.KirkOption.Description = "Whether to greet Captain Kirk";
            command.Add(command.KirkOption);
            command.SpockOption = new Option<bool>("--spock");
            command.SpockOption.Description = "Whether to greet Spock";
            command.Add(command.SpockOption);
            command.UhuraOption = new Option<bool>("--uhura");
            command.UhuraOption.Description = "Whether to greet Lieutenant Uhura";
            command.Add(command.UhuraOption);
            command.NextGenerationCommand = NextGenerationCommand.Create(command);
            command.Add(command.NextGenerationCommand.SystemCommandLineCommand);
            command.Handler = command;
            return command;
        }

        public struct Result
        {
            internal Result(StarTrekCommand command, ParseResult parseResult)
            {
                var parentResult = command.parent.GetResult(parseResult);
                Greeting = parentResult.Greeting;
                Kirk = parseResult.GetValueForOption(command.KirkOption);
                Spock = parseResult.GetValueForOption(command.SpockOption);
                Uhura = parseResult.GetValueForOption(command.UhuraOption);
            }
            public string Greeting { get; }
            public bool Kirk { get; }
            public bool Spock { get; }
            public bool Uhura { get; }
        }

        public override Result GetResult(ParseResult parseResult) => new Result(this, parseResult);

        public Task<int> InvokeAsync(InvocationContext context)
        {
            var result = GetResult(context);
            Handlers.StarTrek(result.Greeting, result.Kirk, result.Spock, result.Uhura);
            return Task.FromResult(context.ExitCode);
        }

        public int Invoke(InvocationContext context)
        {
            var result = GetResult(context);
            Handlers.StarTrek(result.Greeting, result.Kirk, result.Spock, result.Uhura);
            return context.ExitCode;
        }

        public Option<bool> KirkOption { get; private set; }

        public Option<bool> SpockOption { get; private set; }

        public Option<bool> UhuraOption { get; private set; }

        public NextGenerationCommand NextGenerationCommand { get; private set; }
    }

    public partial class NextGenerationCommand : GeneratedCommandBase<NextGenerationCommand.Result, StarTrekCommand>, ICommandHandler
    {
        private NextGenerationCommand() : base("NextGeneration")
        {
        }

        internal static NextGenerationCommand Create(StarTrekCommand parent)
        {
            var command = new NextGenerationCommand();
            command.parent = parent;
            command.PicardOption = new Option<bool>("--picard");
            command.PicardOption.Description = "This is the description for Picard";
            command.Add(command.PicardOption);
            command.DeepSpaceNine = DeepSpaceNineCommand.Create(command);
            command.Add(command.DeepSpaceNine.SystemCommandLineCommand);
            command.Voyager = VoyagerCommand.Create(command);
            command.Add(command.Voyager.SystemCommandLineCommand);
            command.Handler = command;
            return command;
        }

        public struct Result
        {
            public Result(NextGenerationCommand command, ParseResult parseResult)
            {
                var parentResult = command.parent.GetResult(parseResult);
                Greeting = parentResult.Greeting;
                Picard = parseResult.GetValueForOption(command.PicardOption);
            }
            public string Greeting { get; }
            public bool Picard { get; }
        }

        public override Result GetResult(ParseResult parseResult) => new Result(this, parseResult);

        public Task<int> InvokeAsync(InvocationContext context)
        {

            var result = GetResult(context);
            Handlers.NextGeneration(result.Greeting, result.Picard);
            return Task.FromResult(context.ExitCode);
        }

        public int Invoke(InvocationContext context)
        {
            var result = GetResult(context);
            Handlers.NextGeneration(result.Greeting, result.Picard);
            return context.ExitCode;
        }

        public Option<bool> PicardOption { get; private set; }

        public DeepSpaceNineCommand DeepSpaceNine { get; private set; }
        public VoyagerCommand Voyager { get; private set; }
    }

    public partial class DeepSpaceNineCommand : GeneratedCommandBase<DeepSpaceNineCommand.Result, NextGenerationCommand>, ICommandHandler
    {
        private DeepSpaceNineCommand() : base("DeepSpaceNine")
        {
        }

        internal static DeepSpaceNineCommand Create(NextGenerationCommand parent)
        {
            var command = new DeepSpaceNineCommand();
            command.parent = parent;
            command.SiskoOption = new Option<bool>("--sisko");
            command.Add(command.SiskoOption);
            command.OdoOption = new Option<bool>("--odo");
            command.Add(command.OdoOption);
            command.DaxOption = new Option<bool>("--dax");
            command.Add(command.DaxOption);
            command.WorfOption = new Option<bool>("--worf");
            command.Add(command.WorfOption);
            command.OBrienOption = new Option<bool>("--o-brien");
            command.Add(command.OBrienOption);
            command.Handler = command;
            return command;
        }

        public struct Result
        {
            public Result(DeepSpaceNineCommand command, ParseResult parseResult)
            {
                var parentResult = command.parent.GetResult(parseResult);
                Greeting = parentResult.Greeting;
                Sisko = parseResult.GetValueForOption(command.SiskoOption);
                Odo = parseResult.GetValueForOption(command.OdoOption);
                Dax = parseResult.GetValueForOption(command.DaxOption);
                Worf = parseResult.GetValueForOption(command.WorfOption);
                OBrien = parseResult.GetValueForOption(command.OBrienOption);
            }
            public string Greeting { get; }
            public bool Sisko { get; }
            public bool Odo { get; }
            public bool Dax { get; }
            public bool Worf { get; }
            public bool OBrien { get; }
        }

        public override Result GetResult(ParseResult parseResult) => new Result(this, parseResult);

        public Task<int> InvokeAsync(InvocationContext context)
        {
            var result = GetResult(context);
            Handlers.DeepSpaceNine(result.Greeting, result.Sisko, result.Odo, result.Dax, result.Worf, result.OBrien);
            return Task.FromResult(context.ExitCode);
        }

        public int Invoke(InvocationContext context)
        {
            var result = GetResult(context);
            Handlers.DeepSpaceNine(result.Greeting, result.Sisko, result.Odo, result.Dax, result.Worf, result.OBrien);
            return context.ExitCode;
        }

        public Option<bool> SiskoOption { get; private set; }

        public Option<bool> OdoOption { get; private set; }

        public Option<bool> DaxOption { get; private set; }

        public Option<bool> WorfOption { get; private set; }

        public Option<bool> OBrienOption { get; private set; }
    }

    public partial class VoyagerCommand : GeneratedCommandBase<VoyagerCommand.Result, NextGenerationCommand>, ICommandHandler
    {
        private VoyagerCommand() : base("Voyager")
        {
        }

        internal static VoyagerCommand Create(NextGenerationCommand parent)
        {
            var command = new VoyagerCommand();
            command.parent = parent;
            command.JanewayOption = new Option<bool>("--janeway");
            command.Add(command.JanewayOption);
            command.ChakotayOption = new Option<bool>("--chakotay");
            command.Add(command.ChakotayOption);
            command.TorresOption = new Option<bool>("--torres");
            command.Add(command.TorresOption);
            command.TuvokOption = new Option<bool>("--tuvok");
            command.Add(command.TuvokOption);
            command.SevenOfNineOption = new Option<bool>("--sevenOfNine");
            command.Add(command.SevenOfNineOption);
            command.Handler = command;
            return command;
        }

        public struct Result
        {
            public Result(VoyagerCommand command, ParseResult parseResult)
            {
                var parentResult = command.parent.GetResult(parseResult);
                Greeting = parentResult.Greeting;
                Janeway = parseResult.GetValueForOption(command.JanewayOption);
                Chakotay = parseResult.GetValueForOption(command.ChakotayOption);
                Torres = parseResult.GetValueForOption(command.TorresOption);
                Tuvok = parseResult.GetValueForOption(command.TuvokOption);
                SevenOfNine = parseResult.GetValueForOption(command.SevenOfNineOption);
            }
            public string Greeting { get; }
            public bool Janeway { get; }
            public bool Chakotay { get; }
            public bool Torres { get; }
            public bool Tuvok { get; }
            public bool SevenOfNine { get; }
        }

        public override Result GetResult(ParseResult parseResult) => new Result(this, parseResult);

        public Task<int> InvokeAsync(InvocationContext context)
        {
            var result = GetResult(context);
            Handlers.Voyager(result.Greeting, result.Janeway, result.Chakotay, result.Torres, result.Tuvok, result.SevenOfNine);
            return Task.FromResult(context.ExitCode);
        }

        public int Invoke(InvocationContext context)
        {
            var result = GetResult(context);
            Handlers.Voyager(result.Greeting, result.Janeway, result.Chakotay, result.Torres, result.Tuvok, result.SevenOfNine);
            return context.ExitCode;
        }

        public Option<bool> JanewayOption { get; private set; }

        public Option<bool> ChakotayOption { get; private set; }

        public Option<bool> TorresOption { get; private set; }

        public Option<bool> TuvokOption { get; private set; }

        public Option<bool> SevenOfNineOption { get; private set; }
    }

}


