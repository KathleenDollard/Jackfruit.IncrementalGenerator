using System;
using System.Collections.Generic;
using System.Text;

namespace Jackfruit.Models
{
    public class ValidatorDef
    {
        // Find use of AddValidator method
        // Find type of instance it is called on
        // Find delegate
        // Error if it does not return string or IEnumerable<string>
        // Find it's parameters
        // Check alignment in count and type of parameters and param args to AddValidator, Error if wrong

        // Also allow the user to just get the result, and recognize that.

        // Attach this data to a command

        public ValidatorDef(string methodName)
        {
            MethodName = methodName;
        }
        public string MethodName { get; }
        public List<MemberDef> Parameters { get; } = new List<MemberDef>();


    }
}
