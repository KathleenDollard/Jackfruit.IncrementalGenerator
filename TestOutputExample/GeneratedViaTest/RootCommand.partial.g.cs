
using Jackfruit.Internal;
using System;

namespace Jackfruit
{
    /// <summary>
    /// This is the main class for the Jackfruit generator. After you call the 
    /// Create command, the returned RootCommand will contain your CLI. If you 
    /// need multiple root commands in your application differentiate them with &gt;T&lt;
    /// </summary>
    public partial class RootCommand : RootCommand<RootCommand, RootCommand.Result>
    {
        public static RootCommand Create(params SubCommand[] subCommands)
            => (RootCommand)RootCommand<RootCommand, RootCommand.Result>.Create(null, subCommands);

        public new static RootCommand Create(Delegate runHandler, params SubCommand[] subCommands)
            => (RootCommand)RootCommand<RootCommand, RootCommand.Result>.Create(runHandler, subCommands);


        public partial class Result
        { }
    }
}
