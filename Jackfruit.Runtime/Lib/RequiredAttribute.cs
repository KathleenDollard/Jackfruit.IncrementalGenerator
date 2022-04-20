using System;
using System.Collections.Generic;
using System.Text;

namespace Jackfruit.IncrementalGenerator.Lib
{
    [System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    sealed class RequiredAttribute : Attribute
    {
        public RequiredAttribute(bool isRequired = false)
        {
            IsRequired = isRequired;
        }

        public bool IsRequired { get; }
    }
}
