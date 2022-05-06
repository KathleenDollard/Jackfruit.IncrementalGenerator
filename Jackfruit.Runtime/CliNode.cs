namespace Jackfruit
{
    public class CliNode
    {
        public Delegate Action;
        public IEnumerable<CliNode> SubCommands;

        public CliNode(Delegate action, List<CliNode> subCommands)
        {
            Action = action;
            SubCommands = subCommands;
        }
        public CliNode(Delegate action)
        {
            Action = action;
            SubCommands = new List<CliNode>();
        }
    }
}
