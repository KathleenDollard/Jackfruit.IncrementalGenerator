using System;
using System.Collections.Generic;
using System.Text;

namespace Jackfruit.IncrementalGenerator
{
    internal class Helpers
    {
        public const string ConsoleClass = @"
using System;

namespace Jackfruit
{
    public class ConsoleApplication
    {
        public static ConsoleApplication CreateWithRootCommand(Delegate rootCommandHandler) { }
    }

    public class CliCommand
    {
        public static CliCommand AddCommand(Delegate CommandHandler)
    }
}
";
    }
}
