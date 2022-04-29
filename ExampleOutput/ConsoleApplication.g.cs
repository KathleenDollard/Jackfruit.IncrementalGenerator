// This file is created by a generator.
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using DemoHandlers;
using Jackfruit;

namespace Jackfruit
{
    public partial class ConsoleApplication
    {
        private StarTrekCommand _rootCommand;
        private ConsoleApplication()
        {
        }

        public int Run(string[] args)
        {
            return RootCommand.Command.Invoke(args);
        }

        public Jackfruit.StarTrekCommand RootCommand
        {
            get
            {
                if (_rootCommand is null)
                {

                    _rootCommand = StarTrekCommand.Create();
                }
                return _rootCommand;
            }
        }


    }

}

