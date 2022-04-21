using DemoHandlers;
using Jackfruit.IncrementalGenerator;

namespace ExampleOutput;

public class Program
{
    public static void Main(string[] args)
    {
        ConsoleApplication.AddRootCommand(Handlers.NextGeneration);

        // if you do not want to customize anything
        ConsoleApplication.Run(args);



    }
}