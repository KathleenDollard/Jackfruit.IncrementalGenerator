// This file is created by a generator.
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Jackfruit.Internal;
using System.Threading.Tasks;

namespace Jackfruit.DemoHandlersSubCommands
{
    /// <summary>
    /// The wrapper class for the StarTrek command.
    /// </summary>
    public class StarTrek : GeneratedCommandBase<StarTrek, StarTrek.Result, RootCommand>, ICommandHandler
    {
        internal static StarTrek Build(RootCommand parent)
        {
            var command = new StarTrek();
            command.Parent = parent;
            command.Name = "StarTrek";
            command.KirkOption = new Option<bool>("--Kirk");
            command.KirkOption.Description = "Whether to greet Captain Kirk";
            command.Add(command.KirkOption);
            command.SpockOption = new Option<bool>("--Spock");
            command.SpockOption.Description = "Whether to greet Spock";
            command.Add(command.SpockOption);
            command.UhuraOption = new Option<bool>("--Uhura");
            command.UhuraOption.Description = "Whether to greet Lieutenant Uhura";
            command.Add(command.UhuraOption);
            command.NextGeneration = NextGeneration.Build(command);
            command.AddCommandToScl(command.NextGeneration);
            command.AddValidator(command.Validate);
            command.Handler = command;
            return command;
        }

        /// <summary>
        /// The result class for the StarTrek command.
        /// </summary>
        public class Result : RootCommand.Result
        {
            internal Result(StarTrek command, InvocationContext invocationContext)
                : this(command, invocationContext.ParseResult.CommandResult)
            {
            }

            private protected Result(StarTrek command, CommandResult commandResult)
                : base(command.Parent, commandResult)
            {
                Kirk = GetValueForSymbol(command.KirkOption, commandResult);
                Spock = GetValueForSymbol(command.SpockOption, commandResult);
                Uhura = GetValueForSymbol(command.UhuraOption, commandResult);
            }

            public bool Kirk { get;  }
            public bool Spock { get;  }
            public bool Uhura { get;  }
        }

        /// <summary>
        /// Get an instance of the Result class for the StarTrek command.
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
            DemoHandlers.Handlers.StarTrek(result.Greeting, result.Kirk, result.Spock, result.Uhura);
            return invocationContext.ExitCode;
        }

        /// <summary>
        /// The handler invoked by System.CommandLine. This will not be public when generated is more sophisticated.
        /// </summary>
        /// <param name="invocationContext">The System.CommandLine Invocation context used to retrieve values.</param>
        public Task<int> InvokeAsync(InvocationContext invocationContext)
        {
            var result = GetResult(invocationContext);
            DemoHandlers.Handlers.StarTrek(result.Greeting, result.Kirk, result.Spock, result.Uhura);
            return Task.FromResult(invocationContext.ExitCode);
        }

        public Option<bool> KirkOption { get; set; }
        public Option<bool> SpockOption { get; set; }
        public Option<bool> UhuraOption { get; set; }
        public NextGeneration NextGeneration { get; set; }
    }
}
