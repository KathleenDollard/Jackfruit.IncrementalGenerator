using DemoHandlers;
using Jackfruit.IncrementalGenerator;

namespace ExampleOutput;

public class Program
{
    public static void Main(string[] args)
    {
        ConsoleApp.AddRootCommand(Handlers.NextGeneration);

        // if you do not want to customize anything
        ConsoleAppBase.Run(args);



    }
}