using System;
using System.Collections.Generic;
using System.Text;

namespace Jackfruit.IncrementalGenerator.Lib
{
    [System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    sealed class ArgumentAttribute : Attribute
    {
        public ArgumentAttribute(bool isArgument = false)
        {
            IsArgument = isArgument;
        }

        public bool IsArgument { get; }
    }
}
