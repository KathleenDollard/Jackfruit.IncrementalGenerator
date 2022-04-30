// This file is created by a generator.
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using DemoHandlersUpdated;
using Jackfruit;

namespace Jackfruit
{
    public partial class ConsoleApplication
    {
        private FranshiseCommand _rootCommand;
        private ConsoleApplication()
        {
        }

        public int Run(string[] args)
        {
            return RootCommand.SystemCommandLineCommand.Invoke(args);
        }

        public Jackfruit.FranshiseCommand RootCommand
        {
            get
            {
                if (_rootCommand is null)
                {

                    _rootCommand = FranshiseCommand.Create();
                }
                return _rootCommand;
            }
        }


    }

}

