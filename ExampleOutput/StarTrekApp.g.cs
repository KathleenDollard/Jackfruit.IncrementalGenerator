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


        // Several changes in this file to generate!!
        // also when i run --picard shows false because the -- is not included

        public static void AddRootCommand(Delegate rootCommandHandler)
        { }

        public static ConsoleApplication Create()
        { 
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

        public static int Run (string[] args)
        {
            var app =ConsoleApplication.Create();
            return app.CliRoot.Invoke(args);
        }
    }

}


