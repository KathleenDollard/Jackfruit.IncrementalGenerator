// This file is created by a generator.
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;

namespace DemoHandlers
{
    class ConsoleApplication
    {
        private NextGenerationCommand _CliRoot;
        private ConsoleApplication()
        {
        }

        public static ConsoleApplication CreateWithRootCommand(Delegate rootCommandHandler)
        {
            // Changes here!!
            var app = new ConsoleApplication();
            app._CliRoot = NextGenerationCommand.Create();
            return app;
        }

        public NextGenerationCommand CliRoot
        {
            get
            {
                return _CliRoot;
            }
        }
    }

}