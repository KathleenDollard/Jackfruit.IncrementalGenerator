using System;
using System.Collections.Generic;
using System.Text;

namespace Jackfruit.IncrementalGenerator.Lib
{
    [System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    sealed class OptionArgumentNameAttribute : Attribute
    {
        public OptionArgumentNameAttribute(string argumentName)
        {
            ArgumentName = argumentName;
        }

        public string ArgumentName { get; }
    }
}
