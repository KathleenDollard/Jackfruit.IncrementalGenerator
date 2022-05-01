// This file is created by a generator.
using DemoHandlersUpdated;

namespace Jackfruit;

public partial class CliRoot
{
    private Commands.Franchise? _rootCommand;
    public Commands.Franchise RootCommand
    {
        get
        {
            _rootCommand ??= Commands.Franchise.Create();
            return _rootCommand;
        }
    }

    public int Run(string[] args)
    {
        return RootCommand.Run(args);
    }
}

