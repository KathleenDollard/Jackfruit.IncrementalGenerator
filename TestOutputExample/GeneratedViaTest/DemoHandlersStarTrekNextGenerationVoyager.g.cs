// This file is created by a generator.
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Jackfruit.Internal;

namespace Jackfruit.DemoHandlersSubCommands
{
    /// <summary>
    /// The wrapper class for the Voyager command.
    /// </summary>
    public class Voyager : GeneratedCommandBase<Voyager, Voyager.Result, NextGeneration>, ICommandHandler
    {
        internal static Voyager Build(NextGeneration parent)
        {
            var command = new Voyager();
            command.Parent = parent;
            command.Name = "Voyager";
            command.JanewayOption = new Option<bool>("--Janeway");
            command.Add(command.JanewayOption);
            command.ChakotayOption = new Option<bool>("--Chakotay");
            command.Add(command.ChakotayOption);
            command.TorresOption = new Option<bool>("--Torres");
            command.Add(command.TorresOption);
            command.TuvokOption = new Option<bool>("--Tuvok");
            command.Add(command.TuvokOption);
            command.SevenOfNineOption = new Option<bool>("--SevenOfNine");
            command.Add(command.SevenOfNineOption);
            command.AddValidator(command.Validate);
            command.Handler = command;
            return command;
        }

        /// <summary>
        /// The result class for the Voyager command.
        /// </summary>
        public class Result : NextGeneration.Result
        {
            internal Result(Voyager command, InvocationContext invocationContext)
                : this(command, invocationContext.ParseResult.CommandResult)
            {
                Console = GetService<System.CommandLine.IConsole>(invocationContext);
            }

            private protected Result(Voyager command, CommandResult commandResult)
                : base(command.Parent, commandResult)
            {
                Janeway = GetValueForSymbol(command.JanewayOption, commandResult);
                Chakotay = GetValueForSymbol(command.ChakotayOption, commandResult);
                Torres = GetValueForSymbol(command.TorresOption, commandResult);
                Tuvok = GetValueForSymbol(command.TuvokOption, commandResult);
                SevenOfNine = GetValueForSymbol(command.SevenOfNineOption, commandResult);
            }

            public System.CommandLine.IConsole Console { get;  }
            public bool Janeway { get;  }
            public bool Chakotay { get;  }
            public bool Torres { get;  }
            public bool Tuvok { get;  }
            public bool SevenOfNine { get;  }
        }

        /// <summary>
        /// Get an instance of the Result class for the Voyager command.
        /// </summary>
        /// <param name="invocationContext">The System.CommandLine InvocationContext used to retrieve values.</param>
        public override Result GetResult(InvocationContext invocationContext)
        {
            return new Result(this, invocationContext);
        }

        /// <summary>
        /// The handler invoked by System.CommandLine. This will not be public when generated is more sophisticated.
        /// </summary>
        /// <param name="invocationContext">The System.CommandLine Invocation context used to retrieve values.</param>
        public int Invoke(InvocationContext invocationContext)
        {
            var result = GetResult(invocationContext);
            DemoHandlers.Handlers.Voyager(result.Console, result.Greeting, result.Janeway, result.Chakotay, result.Torres, result.Tuvok, result.SevenOfNine);
            return invocationContext.ExitCode;
        }

        /// <summary>
        /// The handler invoked by System.CommandLine. This will not be public when generated is more sophisticated.
        /// </summary>
        /// <param name="invocationContext">The System.CommandLine Invocation context used to retrieve values.</param>
        public Task<int> InvokeAsync(InvocationContext invocationContext)
        {
            var result = GetResult(invocationContext);
            DemoHandlers.Handlers.Voyager(result.Console, result.Greeting, result.Janeway, result.Chakotay, result.Torres, result.Tuvok, result.SevenOfNine);
            return Task.FromResult(invocationContext.ExitCode);
        }

        public Option<bool> JanewayOption { get; set; }
        public Option<bool> ChakotayOption { get; set; }
        public Option<bool> TorresOption { get; set; }
        public Option<bool> TuvokOption { get; set; }
        public Option<bool> SevenOfNineOption { get; set; }
    }
}
