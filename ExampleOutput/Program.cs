using DemoHandlers;
using Jackfruit;

namespace ExampleOutput;

public class Program
{
    public static void Main(string[] args)
    {
        var app = ConsoleApplication.Create();
        app.SetRootCommand(Handlers.StarTrek);
        app.RootCommand.AddCommand(Handlers.NextGeneration);
        app.RootCommand.NextGenerationCommand.AddCommand(Handlers.DeepSpaceNine);
        app.RootCommand.NextGenerationCommand.AddCommand(Handlers.Voyager);


        var x = app.RootCommand.NextGenerationCommand;
        x.PicardOption.AddAlias("-p");

        app.Run(args);

        // exampleoutput nextgeneration --foo voyager --greeting Hello -- janeway

    }
}