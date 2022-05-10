namespace Jackfruit
{
    public class CliNode
    {
        public Delegate Action;
        public IEnumerable<CliNode> SubCommands;

        public CliNode(Delegate action, params CliNode[] subCommands)
        {
            Action = action;
            SubCommands = subCommands;
        }
    }
}
