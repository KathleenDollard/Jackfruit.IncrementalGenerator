using System;
using System.Collections.Generic;
using System.Text;

namespace Jackfruit.Runtime
{
    public abstract class RootCommand
    {
        public abstract void Define();
        public void AddSubCommand() { }
        public void SetAction(Delegate handlerAction) { }
    }
}
