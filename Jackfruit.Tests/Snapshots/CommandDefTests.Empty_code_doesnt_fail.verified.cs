﻿//HintName: ConsoleApplication.g.cs

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