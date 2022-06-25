using Jackfruit;
using Jackfruit.Internal;

namespace DemoHandlers
{


    public partial class RootCommand : RootCommand<RootCommand, RootCommand.Result>
    {
        public new static RootCommand Create(CommandNode cliRoot)
            => (RootCommand)RootCommand<RootCommand, RootCommand.Result>.Create( cliRoot);
    }
}
