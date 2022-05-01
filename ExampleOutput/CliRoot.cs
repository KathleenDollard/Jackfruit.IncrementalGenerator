// This file is created by a generator.
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using DemoHandlersUpdated;
using Jackfruit;

namespace Jackfruit
{
    public partial class CliRoot<T>
        where T : GeneratedCommandBase<T>
    {
        private CliRoot()
        {
        }

        public int Run(string[] args)
        {
            return RootCommand.Run(args);
        }

        private T? _rootCommand;
        public T RootCommand
        {
            get
            {
                if (_rootCommand is null)
                {

                    _rootCommand = T.Create();
                }
                return _rootCommand ?? throw new Exception();
            }
        }


    }

}

