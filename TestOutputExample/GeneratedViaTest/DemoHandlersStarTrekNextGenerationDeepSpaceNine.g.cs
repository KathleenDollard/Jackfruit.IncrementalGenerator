// This file is created by a generator.
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Jackfruit.Internal;
using System.Threading.Tasks;

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
        public class Result: NextGeneration.Result
        {
            internal Result(DeepSpaceNine command, InvocationContext invocationContext) 
                : this(command, invocationContext.ParseResult.CommandResult)
            {
            }

            private protected Result(DeepSpaceNine command, CommandResult commandResult)
                :base(command.Parent, commandResult)
            {
                Sisko = GetValueForSymbol(command.SiskoOption, commandResult);
                Odo = GetValueForSymbol(command.OdoOption, commandResult);
                Dax = GetValueForSymbol(command.DaxOption, commandResult);
                Worf = GetValueForSymbol(command.WorfOption, commandResult);
                OBrien = GetValueForSymbol(command.OBrienOption, commandResult);
            }

            public bool Sisko { get;  }
            public bool Odo { get;  }
            public bool Dax { get;  }
            public bool Worf { get;  }
            public bool OBrien { get;  }

            /// <summary>
            /// Get an instance of the Result class for the NextGeneration command.
            /// </summary>
            /// <param name="invocationContext">The System.CommandLine InvocationContext used to retrieve values.</param>
            internal static Result GetResult(DeepSpaceNine command, InvocationContext invocationContext)
            {
                return new Result(command, invocationContext.ParseResult.CommandResult);
            }
        }

        /// <summary>
        /// The handler invoked by System.CommandLine. This will not be public when generated is more sophisticated.
        /// </summary>
        /// <param name="invocationContext">The System.CommandLine Invocation context used to retrieve values.</param>
        public int Invoke(InvocationContext invocationContext)
        {
            var result = Result.GetResult(this, invocationContext); 
            DemoHandlers.Handlers.DeepSpaceNine(result.Greeting, result.Sisko, result.Odo, result.Dax, result.Worf, result.OBrien);
            return invocationContext.ExitCode;
        }

        /// <summary>
        /// The handler invoked by System.CommandLine. This will not be public when generated is more sophisticated.
        /// </summary>
        /// <param name="invocationContext">The System.CommandLine Invocation context used to retrieve values.</param>
        public Task<int> InvokeAsync(InvocationContext invocationContext)
        {
            var result = Result.GetResult(this, invocationContext);
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
