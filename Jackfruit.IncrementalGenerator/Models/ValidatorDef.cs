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

        public ValidatorDef(string methodName, IEnumerable<string> memberNames)
        {
            MethodName = methodName;
            MemberNames = memberNames;
        }
        public string MethodName { get; }
        public IEnumerable<string> MemberNames  { get; set; }

    }
}
