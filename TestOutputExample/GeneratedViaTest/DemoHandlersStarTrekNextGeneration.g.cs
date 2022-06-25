// This file is created by a generator.
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Jackfruit.Internal;

namespace Jackfruit.DemoHandlersSubCommands
{
    /// <summary>
    /// The wrapper class for the NextGeneration command.
    /// </summary>
    public class NextGeneration : GeneratedCommandBase<NextGeneration, NextGeneration.Result, StarTrek>, ICommandHandler
    {
        internal static NextGeneration Build(StarTrek parent)
        {
            var command = new NextGeneration();
            command.Parent = parent;
            command.Name = "NextGeneration";
            command.PicardOption = new Option<bool>("--Picard");
            command.PicardOption.Description = "This is the description for Picard";
            command.PicardOption.AddAlias("-p");
            command.Add(command.PicardOption);
            command.DeepSpaceNine = DeepSpaceNine.Build(command);
            command.AddCommandToScl(command.DeepSpaceNine);
            command.Voyager = Voyager.Build(command);
            command.AddCommandToScl(command.Voyager);
            command.AddValidator(command.Validate);
            command.Handler = command;
            return command;
        }

        /// <summary>
        /// The result class for the NextGeneration command.
        /// </summary>
        public class Result
        {
            internal Result(NextGeneration command, InvocationContext invocationContext) : this(command, invocationContext.ParseResult.CommandResult, command.Parent.GetResult(invocationContext))
            {
            }

            internal Result(NextGeneration command, CommandResult result) : this(command, result, command.Parent.GetResult(result))
            {
            }

            private Result(NextGeneration command, CommandResult commandResult, StarTrek.Result parentResult)
            {
                Greeting = parentResult.Greeting;
                Kirk = parentResult.Kirk;
                Spock = parentResult.Spock;
                Uhura = parentResult.Uhura;
                Picard = GetValueForSymbol(command.PicardOption, commandResult);
            }

            public string Greeting { get; set; }
            public bool Kirk { get; set; }
            public bool Spock { get; set; }
            public bool Uhura { get; set; }
            public bool Picard { get; set; }
        }

        /// <summary>
        /// Get an instance of the Result class for the NextGeneration command.
        /// </summary>
        /// <param name="invocationContext">The System.CommandLine InvocationContext used to retrieve values.</param>
        public override Result GetResult(InvocationContext invocationContext)
        {
            return new Result(this, invocationContext);
        }

        /// <summary>
        /// Get an instance of the Result class for the NextGeneration command that will not include any services.
        /// </summary>
        /// <param name="result">The System.CommandLine CommandResult used to retrieve values.</param>
        public override Result GetResult(CommandResult result)
        {
            return new Result(this, result);
        }

        /// <summary>
        /// The handler invoked by System.CommandLine. This will not be public when generated is more sophisticated.
        /// </summary>
        /// <param name="invocationContext">The System.CommandLine Invocation context used to retrieve values.</param>
        public int Invoke(InvocationContext invocationContext)
        {
            var result = GetResult(invocationContext);
            DemoHandlers.Handlers.NextGeneration(result.Greeting, result.Picard);
            return invocationContext.ExitCode;
        }

        /// <summary>
        /// The handler invoked by System.CommandLine. This will not be public when generated is more sophisticated.
        /// </summary>
        /// <param name="invocationContext">The System.CommandLine Invocation context used to retrieve values.</param>
        public Task<int> InvokeAsync(InvocationContext invocationContext)
        {
            var result = GetResult(invocationContext);
            DemoHandlers.Handlers.NextGeneration(result.Greeting, result.Picard);
            return Task.FromResult(invocationContext.ExitCode);
        }

        public Option<bool> PicardOption { get; set; }
        public DeepSpaceNine DeepSpaceNine { get; set; }
        public Voyager Voyager { get; set; }
    }
}
