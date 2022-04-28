using DemoHandlers;
using Jackfruit;


namespace ExampleOutput;

public class Program
{
    public static void Main2(string[] args)
    {
        ConsoleApplication.AddRootCommand(Handlers.StarTrek);
        ConsoleApplication.StarTrek.AddSubCommand(Handlers.NextGeneration);
        ConsoleApplication.StarTrek.NextGeneration.AddSubCommand(Handlers.DeepSpaceNine);
        ConsoleApplication.StarTrek.NextGeneration.AddSubCommand(Handlers.Voyager);

        // if you do not want to customize anything
        ConsoleApplication.Run(args);



    }
}