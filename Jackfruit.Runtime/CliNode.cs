namespace Jackfruit
{
    public class CliNode
    {
        // I do not see a use case for exposing these
        internal Delegate Action { get; }
        internal Delegate Validator { get; }
        internal IEnumerable<CliNode> SubCommands { get; }

        /// <summary>
        /// Represents a single command in a CLI and contains the delegate that 
        /// will be run and the CliNodes for the subcommands. This is used only 
        /// for generation.
        /// </summary>
        /// <param name="action">A delegate in the form of the method name without parentheses. 
        /// This form is currently required for generation to work correctly (no lambdas).
        /// </param>
        /// <param name="subCommands">CliNodes for the subcommands of the command</param>
        public CliNode(Delegate action, params CliNode[] subCommands)
        {
            Action = action;
            SubCommands = subCommands;
        }

        public CliNode(Delegate action, Delegate validator, params CliNode[] subCommands)
        {
            Action = action;
            Validator = validator;
            SubCommands = subCommands;
        }
    }
}
