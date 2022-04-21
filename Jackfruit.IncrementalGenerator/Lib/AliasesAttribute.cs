using System;
using System.Collections.Generic;
using System.Text;

namespace Jackfruit
{
    [System.AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, 
                    Inherited = false, AllowMultiple = true)]
    public sealed class AliasesAttribute : Attribute
    {
        public AliasesAttribute(params string[] aliases)
        {
            Values = aliases;
        }

        public string[] Values { get; }
    }
}
