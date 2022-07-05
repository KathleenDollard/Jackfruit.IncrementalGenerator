namespace Jackfruit
{
    public class SubCommand
    {
        // I do not see a use case for exposing these
        internal Delegate Action { get; }
        internal Delegate? Validator { get; }
        internal IEnumerable<SubCommand> SubCommands { get; }

        public static SubCommand Create(Delegate handlerAction, params SubCommand[] subCommands)
        { return new SubCommand(handlerAction, subCommands); }

        public static SubCommand Create(Delegate handlerAction, Delegate resultValidator, params SubCommand[] subCommands)
        { return new SubCommand(handlerAction,resultValidator, subCommands); }

        /// <summary>
        /// Represents a single command in a CLI and contains the delegate that 
        /// will be run and the CliNodes for the subcommands. This is used only 
        /// for generation.
        /// </summary>
        /// <param name="action">A delegate in the form of the method name without parentheses. 
        /// This form is currently required for generation to work correctly (no lambdas).
        /// </param>
        /// <param name="subCommands">CliNodes for the subcommands of the command</param>
        public SubCommand(Delegate action, params SubCommand[] subCommands) 
            : this (action, null, subCommands)
        { }

        public SubCommand(Delegate action, Delegate? validator, params SubCommand[] subCommands)
        {
            Action = action;
            Validator = validator;
            SubCommands = subCommands;
        }
    }
}
