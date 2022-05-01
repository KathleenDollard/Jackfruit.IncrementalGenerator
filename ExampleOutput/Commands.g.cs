// This file is created by a generator.
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Jackfruit;

namespace DemoHandlersUpdated
{
    public partial class CliRoot : GeneratedCommandBase<CliRoot, CliRoot.Result>
    {
        public static CliRoot Create(Delegate methodToRun)
        {
            return Create();
        }

        private CliRoot() : base("rootCommand")
        {
        }

        public static CliRoot Create()
        {
            var command = new CliRoot();
            command.GreetingArgument = new Argument<string>("greetingArg");
            command.Add(command.GreetingArgument);
            command.StarTrekCommand = Commands.StarTrek.Create(command);
            command.AddCommandToScl(command.StarTrekCommand);
            command.SystemCommandLineCommand.AddValidator(command.Validate);
            return command;
        }

        public struct Result
        {
            internal Result(CliRoot command, CommandResult commandResult)
            {
                Greeting = commandResult.GetValueForArgument(command.GreetingArgument);
            }
            public string Greeting { get; }
        }

        public override Result GetResult(CommandResult commandResult) => new Result(this, commandResult);

        public Argument<string> GreetingArgument { get; private set; }

        public Commands.StarTrek StarTrekCommand { get; private set; }

        public override void Validate(CommandResult commandResult)
        {
            var result = GetResult(commandResult);
            var messages = new List<string>();
            AddMessageOnFail(messages, Validators.ValidatePoliteness(result.Greeting));
            commandResult.ErrorMessage += String.Join(Environment.NewLine, messages);
        }
    }

    public class Commands
    {
        public partial class StarTrek : GeneratedCommandBase<StarTrek, StarTrek.Result, CliRoot>, ICommandHandler
        {
            private StarTrek(CliRoot parent) : base("StarTrek", parent)
            {
            }

            internal static StarTrek Create(CliRoot parent)
            {
                var command = new StarTrek(parent);
                command.KirkOption = new Option<bool>("--kirk");
                command.KirkOption.Description = "Whether to greet Captain Kirk";
                command.Add(command.KirkOption);
                command.SpockOption = new Option<bool>("--spock");
                command.SpockOption.Description = "Whether to greet Spock";
                command.Add(command.SpockOption);
                command.UhuraOption = new Option<bool>("--uhura");
                command.UhuraOption.Description = "Whether to greet Lieutenant Uhura";
                command.Add(command.UhuraOption);
                command.NextGenerationCommand = NextGeneration.Create(command);
                command.AddCommandToScl(command.NextGenerationCommand);
                command.SystemCommandLineCommand.AddValidator(command.Validate);
                command.Handler = command;
                return command;
            }

            public struct Result
            {
                internal Result(StarTrek command, CommandResult commandResult)
                {
                    var parentResult = command.Parent.GetResult(commandResult);
                    Greeting = parentResult.Greeting;
                    Kirk = commandResult.GetValueForOption(command.KirkOption);
                    Spock = commandResult.GetValueForOption(command.SpockOption);
                    Uhura = commandResult.GetValueForOption(command.UhuraOption);
                }
                public string Greeting { get; }
                public bool Kirk { get; }
                public bool Spock { get; }
                public bool Uhura { get; }
            }

            public override Result GetResult(CommandResult commandResult) => new Result(this, commandResult);

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

            public NextGeneration NextGenerationCommand { get; private set; }


            public partial class NextGeneration : GeneratedCommandBase<NextGeneration, NextGeneration.Result, StarTrek>, ICommandHandler
            {
                private NextGeneration(StarTrek parent) : base("NextGeneration", parent)
                {
                }

                internal static NextGeneration Create(StarTrek parent)
                {
                    var command = new NextGeneration(parent);
                    command.PicardOption = new Option<bool>("--picard");
                    command.PicardOption.Description = "This is the description for Picard";
                    command.Add(command.PicardOption);
                    command.DeepSpaceNineCommand = DeepSpaceNine.Create(command);
                    command.AddCommandToScl(command.DeepSpaceNineCommand);
                    command.VoyagerCommand = Voyager.Create(command);
                    command.AddCommandToScl(command.VoyagerCommand);
                    command.SystemCommandLineCommand.AddValidator(command.Validate);
                    command.Handler = command;
                    return command;
                }

                public struct Result
                {
                    public Result(NextGeneration command, CommandResult commandResult)
                    {
                        var parentResult = command.Parent.GetResult(commandResult);
                        Greeting = parentResult.Greeting;
                        Picard = commandResult.GetValueForOption(command.PicardOption);
                    }
                    public string Greeting { get; }
                    public bool Picard { get; }
                }

                public override Result GetResult(CommandResult commandResult) => new Result(this, commandResult);

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

                public DeepSpaceNine DeepSpaceNineCommand { get; private set; }
                public Voyager VoyagerCommand { get; private set; }


                public partial class DeepSpaceNine : GeneratedCommandBase<DeepSpaceNine, DeepSpaceNine.Result, NextGeneration>, ICommandHandler
                {
                    private DeepSpaceNine(NextGeneration parent) : base("DeepSpaceNine", parent)
                    {
                    }

                    internal static DeepSpaceNine Create(NextGeneration parent)
                    {
                        var command = new DeepSpaceNine(parent);
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
                        command.SystemCommandLineCommand.AddValidator(command.Validate);
                        command.Handler = command;
                        return command;
                    }

                    public struct Result
                    {
                        public Result(DeepSpaceNine command, CommandResult commandResult)
                        {
                            var parentResult = command.Parent.GetResult(commandResult);
                            Greeting = parentResult.Greeting;
                            Sisko = commandResult.GetValueForOption(command.SiskoOption);
                            Odo = commandResult.GetValueForOption(command.OdoOption);
                            Dax = commandResult.GetValueForOption(command.DaxOption);
                            Worf = commandResult.GetValueForOption(command.WorfOption);
                            OBrien = commandResult.GetValueForOption(command.OBrienOption);
                        }
                        public string Greeting { get; }
                        public bool Sisko { get; }
                        public bool Odo { get; }
                        public bool Dax { get; }
                        public bool Worf { get; }
                        public bool OBrien { get; }
                    }

                    public override Result GetResult(CommandResult commandResult) => new Result(this, commandResult);

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

                public partial class Voyager : GeneratedCommandBase<Voyager, Voyager.Result, NextGeneration>, ICommandHandler
                {
                    private Voyager(NextGeneration parent) : base("Voyager", parent)
                    {
                    }

                    internal static Voyager Create(NextGeneration parent)
                    {
                        var command = new Voyager(parent);
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
                        command.SystemCommandLineCommand.AddValidator(command.Validate);
                        command.Handler = command;
                        return command;
                    }

                    public struct Result
                    {
                        public Result(Voyager command, CommandResult commandResult)
                        {
                            var parentResult = command.Parent.GetResult(commandResult);
                            Greeting = parentResult.Greeting;
                            Janeway = commandResult.GetValueForOption(command.JanewayOption);
                            Chakotay = commandResult.GetValueForOption(command.ChakotayOption);
                            Torres = commandResult.GetValueForOption(command.TorresOption);
                            Tuvok = commandResult.GetValueForOption(command.TuvokOption);
                            SevenOfNine = commandResult.GetValueForOption(command.SevenOfNineOption);
                        }
                        public string Greeting { get; }
                        public bool Janeway { get; }
                        public bool Chakotay { get; }
                        public bool Torres { get; }
                        public bool Tuvok { get; }
                        public bool SevenOfNine { get; }
                    }

                    public override Result GetResult(CommandResult commandResult) => new Result(this, commandResult);

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
        }
    }
}



