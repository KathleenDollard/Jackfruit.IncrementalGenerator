using Jackfruit.Internal;

namespace Jackfruit
{
    public partial class RootCommand : RootCommand<RootCommand, RootCommand.Result>
    {
        public new static RootCommand Create(CommandNode cliRoot)
            => (RootCommand)RootCommand<RootCommand, RootCommand.Result>.Create( cliRoot);
    }
}
