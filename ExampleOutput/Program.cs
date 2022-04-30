using DemoHandlersUpdated;
using Jackfruit;

namespace ExampleOutput;

public class Program
{
    public static void Main(string[] args)
    {
        var app = ConsoleApplication.Create();
        app.SetRootCommand(Handlers.Franchise);
        app.RootCommand.AddCommand(Handlers.StarTrek);
        app.RootCommand.StarTrekCommand.AddCommand(Handlers.NextGeneration);
        app.RootCommand.StarTrekCommand.NextGenerationCommand.AddCommand(Handlers.DeepSpaceNine);
        app.RootCommand.StarTrekCommand.NextGenerationCommand.AddCommand(Handlers.Voyager); 
        
        var nextGen = app.RootCommand.StarTrekCommand.NextGenerationCommand;
        nextGen.AddValidator(Validators.ValidatePoliteness, nextGen.PicardOption);
        nextGen.PicardOption.AddAlias("-p");

        app.Run(args);

        // exampleoutput nextgeneration --foo voyager --greeting Hello -- janeway

    }
}