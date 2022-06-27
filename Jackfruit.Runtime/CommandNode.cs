namespace Jackfruit
{
    public class CommandNode
    {
        // I do not see a use case for exposing these
        internal Delegate Action { get; }
        internal Delegate? Validator { get; }
        internal IEnumerable<CommandNode> SubCommands { get; }

        public static CommandNode Create(Delegate action, params CommandNode[] subCommands)
        { return new CommandNode(action, subCommands); }

        public static CommandNode Create(Delegate action, Delegate validator, params CommandNode[] subCommands)
        { return new CommandNode(action,validator, subCommands); }

        /// <summary>
        /// Represents a single command in a CLI and contains the delegate that 
        /// will be run and the CliNodes for the subcommands. This is used only 
        /// for generation.
        /// </summary>
        /// <param name="action">A delegate in the form of the method name without parentheses. 
        /// This form is currently required for generation to work correctly (no lambdas).
        /// </param>
        /// <param name="subCommands">CliNodes for the subcommands of the command</param>
        public CommandNode(Delegate action, params CommandNode[] subCommands) 
            : this (action, null, subCommands)
        { }

        public CommandNode(Delegate action, Delegate? validator, params CommandNode[] subCommands)
        {
            Action = action;
            Validator = validator;
            SubCommands = subCommands;
        }
    }
}
