// This file is created by a generator.
using System;
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Jackfruit.Internal;
using Jackfruit.DemoHandlersSubCommands;

namespace Jackfruit
{
    public partial class RootCommand : ICommandHandler
    {
        public RootCommand()
        {
            Name = "Franchise";
            GreetingArgument = new Argument<string>("greetingArg");
            Add(GreetingArgument);
            StarTrek = StarTrek.Build(this);
            AddCommandToScl(StarTrek);
            AddValidator(Validate);
            Handler = this;
        }

        /// <summary>
        /// The result class for the Franchise command.
        /// </summary>
        public class Result
        {
            internal Result(RootCommand command, InvocationContext invocationContext)
                : this(command, invocationContext.ParseResult.CommandResult)
            {
            }

            internal Result(RootCommand command, CommandResult commandResult)
            {
                Greeting = GetValueForSymbol(command.GreetingArgument, commandResult);
            }

            public string Greeting { get; set; }
        }

        /// <summary>
        /// Get an instance of the Result class for the Franchise command.
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
            DemoHandlers.Handlers.Franchise(result.Greeting);
            return invocationContext.ExitCode;
        }

        /// <summary>
        /// The handler invoked by System.CommandLine. This will not be public when generated is more sophisticated.
        /// </summary>
        /// <param name="invocationContext">The System.CommandLine Invocation context used to retrieve values.</param>
        public Task<int> InvokeAsync(InvocationContext invocationContext)
        {
            var result = GetResult(invocationContext);
            DemoHandlers.Handlers.Franchise(result.Greeting);
            return Task.FromResult(invocationContext.ExitCode);
        }

        /// <summary>
        /// The validate method invoked by System.CommandLine.
        /// </summary>
        /// <param name="commandResult">The System.CommandLine CommandResult used to retrieve values for validation and it will hold any errors.</param>
        public override void Validate(InvocationContext invocationContext)
        {
            base.Validate(invocationContext);
            var result = GetResult(invocationContext);
            var err = string.Join(Environment.NewLine, DemoHandlers.Validators.FranchiseValidate(result.Greeting));
            if (!(string.IsNullOrWhiteSpace(err)))
            {
                invocationContext.ParseResult.CommandResult.ErrorMessage = err;
            }
        }

        public Argument<string> GreetingArgument { get; set; }
        public StarTrek StarTrek { get; set; }
    }
}