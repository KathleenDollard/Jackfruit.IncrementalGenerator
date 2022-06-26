// This file is created by a generator.
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Jackfruit.Internal;
using System.Threading.Tasks;

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
        public class Result : StarTrek.Result
        {
            internal Result(NextGeneration command, InvocationContext invocationContext)
                : this(command, invocationContext.ParseResult.CommandResult)
            {
            }

            private protected Result(NextGeneration command, CommandResult commandResult)
                : base(command.Parent, commandResult)
            {
                Picard = GetValueForSymbol(command.PicardOption, commandResult);
            }

            public bool Picard { get; }
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
