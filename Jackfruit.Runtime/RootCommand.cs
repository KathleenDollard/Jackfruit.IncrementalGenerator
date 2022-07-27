using System;
using System.Collections.Generic;
using System.Text;

namespace Jackfruit.Runtime
{
    public abstract class RootCommand
    {
        public abstract void Define();
        protected void AddSubCommand(SubCommand subCommand) { }
        protected void SetAction(Delegate handlerAction) { }
        }
}
