// This file is created by a generator.
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;

namespace DemoHandlers
{
    class ConsoleApplication
    {
        private StarTrekCommand _CliRoot;
        private ConsoleApplication()
        {
        }

        public static void AddRootCommand(Delegate rootCommandHandler)
        {
        }

        public static ConsoleApplication Create()
        {
            var app = new ConsoleApplication();
            app._CliRoot = StarTrekCommand.Create();
            return app;
        }

        public static int Run(string[] args)
        {
            var app = ConsoleApplication.Create();
            return app.CliRoot.Invoke(args);
        }

        public StarTrekCommand CliRoot
        {
            get
            {
                return _CliRoot;
            }
        }
    }

}