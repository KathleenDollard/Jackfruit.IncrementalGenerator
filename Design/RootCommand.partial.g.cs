using Jackfruit.Internal;

namespace Jackfruit
{
    /// <summary>
    /// This is the main class for the Jackfruit generator. After you call the 
    /// Create command, the returned RootCommand will contain your CLI. If you 
    /// need multiple root commands in your application differentiate them with &gt;T&lt;
    /// </summary>
    public partial class RootCommand : RootCommand<RootCommand, RootCommand.Result>
    {
        public new static RootCommand Create(CommandNode rootNode)
        { 
            return (RootCommand)RootCommand<RootCommand, RootCommand.Result>.Create(rootNode);
        }
    }
}
