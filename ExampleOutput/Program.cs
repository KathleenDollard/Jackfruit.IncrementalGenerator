using DemoHandlersUpdated;
using Jackfruit;

namespace ExampleOutput;

public class Program
{
    public static void Main(string[] args)
    {

        var cliRoot = CliRoot.Create(Handlers.Franchise);
        cliRoot.AddCommand(Handlers.StarTrek);
        cliRoot.AddCommand<Commands.StarTrek>(Handlers.NextGeneration);
        cliRoot.AddCommand<Commands.StarTrek.NextGeneration>(Handlers.DeepSpaceNine);
        cliRoot.AddCommand<Commands.StarTrek.NextGeneration>(Handlers.Voyager);

        // An example of a runtime change to the parse tree
        var nextGen = cliRoot.StarTrekCommand.NextGenerationCommand; // cliroot.Franchise.StarTrek... could also be supported
        nextGen.PicardOption.AddAlias("-p");

        cliRoot.AddValidator(Validators.ValidatePoliteness, cliRoot.GreetingArgument);
        nextGen.AddValidator(NextGenerationResultValidator);

        cliRoot.Run(args);
    }

    private static IEnumerable<string> NextGenerationResultValidator(Commands.StarTrek.NextGeneration.Result result)
    { return Enumerable.Empty<string>(); }


    // Example:
    // exe Hello startrek nextgeneration voyager -- janeway

    //// Alternate syntax, same thing as above
    //cliRoot.AddCommands(Handlers.StarTrek);
    //cliRoot.AddCommands<Commands.StarTrek>(Handlers.NextGeneration);
    //cliRoot.AddCommands<Commands.StarTrek.NextGeneration>(
    //        Handlers.DeepSpaceNine,
    //        Handlers.Voyager);
}