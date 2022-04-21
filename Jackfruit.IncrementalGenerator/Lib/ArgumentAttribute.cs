using System;
using System.Collections.Generic;
using System.Text;

namespace Jackfruit
{
    [System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class ArgumentAttribute : Attribute
    {
        public ArgumentAttribute(bool isArgument = false)
        {
            IsArgument = isArgument;
        }

        public bool IsArgument { get; }
    }
}
