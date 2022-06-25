// This file is created by a generator.
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Jackfruit.Internal;

namespace Jackfruit.DemoHandlersSubCommands
{
    /// <summary>
    /// The wrapper class for the DeepSpaceNine command.
    /// </summary>
    public class DeepSpaceNine : GeneratedCommandBase<DeepSpaceNine, DeepSpaceNine.Result, NextGeneration>, ICommandHandler
    {
        internal static DeepSpaceNine Build(NextGeneration parent)
        {
            var command = new DeepSpaceNine();
            command.Parent = parent;
            command.Name = "DeepSpaceNine";
            command.SiskoOption = new Option<bool>("--Sisko");
            command.Add(command.SiskoOption);
            command.OdoOption = new Option<bool>("--Odo");
            command.Add(command.OdoOption);
            command.DaxOption = new Option<bool>("--Dax");
            command.Add(command.DaxOption);
            command.WorfOption = new Option<bool>("--Worf");
            command.Add(command.WorfOption);
            command.OBrienOption = new Option<bool>("--OBrien");
            command.Add(command.OBrienOption);
            command.AddValidator(command.Validate);
            command.Handler = command;
            return command;
        }

        /// <summary>
        /// The result class for the DeepSpaceNine command.
        /// </summary>
        public class Result
        {
            internal Result(DeepSpaceNine command, InvocationContext invocationContext) : this(command, invocationContext.ParseResult.CommandResult, command.Parent.GetResult(invocationContext))
            {
            }

            internal Result(DeepSpaceNine command, CommandResult result) : this(command, result, command.Parent.GetResult(result))
            {
            }

            private Result(DeepSpaceNine command, CommandResult commandResult, NextGeneration.Result parentResult)
            {
                Greeting = parentResult.Greeting;
                Kirk = parentResult.Kirk;
                Spock = parentResult.Spock;
                Uhura = parentResult.Uhura;
                Picard = parentResult.Picard;
                Sisko = GetValueForSymbol(command.SiskoOption, commandResult);
                Odo = GetValueForSymbol(command.OdoOption, commandResult);
                Dax = GetValueForSymbol(command.DaxOption, commandResult);
                Worf = GetValueForSymbol(command.WorfOption, commandResult);
                OBrien = GetValueForSymbol(command.OBrienOption, commandResult);
            }

            public string Greeting { get; set; }
            public bool Kirk { get; set; }
            public bool Spock { get; set; }
            public bool Uhura { get; set; }
            public bool Picard { get; set; }
            public bool Sisko { get; set; }
            public bool Odo { get; set; }
            public bool Dax { get; set; }
            public bool Worf { get; set; }
            public bool OBrien { get; set; }
        }

        /// <summary>
        /// Get an instance of the Result class for the DeepSpaceNine command.
        /// </summary>
        /// <param name="invocationContext">The System.CommandLine InvocationContext used to retrieve values.</param>
        public override Result GetResult(InvocationContext invocationContext)
        {
            return new Result(this, invocationContext);
        }

        /// <summary>
        /// Get an instance of the Result class for the DeepSpaceNine command that will not include any services.
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
            DemoHandlers.Handlers.DeepSpaceNine(result.Greeting, result.Sisko, result.Odo, result.Dax, result.Worf, result.OBrien);
            return invocationContext.ExitCode;
        }

        /// <summary>
        /// The handler invoked by System.CommandLine. This will not be public when generated is more sophisticated.
        /// </summary>
        /// <param name="invocationContext">The System.CommandLine Invocation context used to retrieve values.</param>
        public Task<int> InvokeAsync(InvocationContext invocationContext)
        {
            var result = GetResult(invocationContext);
            DemoHandlers.Handlers.DeepSpaceNine(result.Greeting, result.Sisko, result.Odo, result.Dax, result.Worf, result.OBrien);
            return Task.FromResult(invocationContext.ExitCode);
        }

        public Option<bool> SiskoOption { get; set; }
        public Option<bool> OdoOption { get; set; }
        public Option<bool> DaxOption { get; set; }
        public Option<bool> WorfOption { get; set; }
        public Option<bool> OBrienOption { get; set; }
    }
}
