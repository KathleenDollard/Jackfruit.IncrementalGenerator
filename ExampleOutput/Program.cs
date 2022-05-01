using DemoHandlersUpdated;
using Jackfruit;

namespace ExampleOutput;

public class Program
{
    public static void Main(string[] args)
    {
        // Should the root be a command or an abstraction of a CLI? Something else?
        // I have not been able to make this not-static. With it being static, multiple CLIRoots are impossible.
        // The prblem is Create returning the correct, generated, type.
        // I am using CliFactory, instead of CliRoot to be in the right namespace
        var cliRoot =  CliRoot.Create(Handlers.Franchise);
        // I like that this makes simple APIs look simple and cleanly expresses the shape of complex APIs
        cliRoot.AddCommands(Handlers.StarTrek);
        cliRoot.AddCommands<Commands.StarTrek>(Handlers.NextGeneration);
        cliRoot.AddCommands<Commands.StarTrek.NextGeneration>(
                Handlers.DeepSpaceNine,
                Handlers.Voyager);

        // Altenrate syntax, same thing as above
        cliRoot.AddCommand(Handlers.StarTrek);
        cliRoot.AddCommand<Commands.StarTrek>(Handlers.NextGeneration);
        cliRoot.AddCommand<Commands.StarTrek.NextGeneration>(Handlers.DeepSpaceNine);
        cliRoot.AddCommand<Commands.StarTrek.NextGeneration>(Handlers.Voyager);


        // This is a runtime change to the parse tree
        var nextGen = cliRoot.StarTrekCommand.NextGenerationCommand; // cliroot.Franchise.StarTrek... could also be supported
        nextGen.PicardOption.AddAlias("-p");

        cliRoot.AddValidator(Validators.ValidatePoliteness, cliRoot.GreetingArgument);
        nextGen.AddValidator(NextGenerationResultValidator);

        cliRoot.Run(args);

        // exe Hello startrek nextgeneration voyager -- janeway

    }

    private static IEnumerable<string> NextGenerationResultValidator(Commands.StarTrek.NextGeneration.Result result)
    { return Enumerable.Empty<string>(); }


    //Work on an instance design
    // tl;dr; Generation is to a type, not an instance. Type from instance before type is generated is hard/impossible
    // It will be difficult or impossible to implement this version, unless we require that people 
    // follow limited patterns. For example, creating a local variable for NextGeneration and setting
    // it via a method call would be perfectly legal C# and very difficult to parse. At compile time, 
    // we have just the ASCII text for the types we have not yet created.
    // Also, I am removing ConsoleApplication due to Immo's comments in the API review 4/28/22. We can add it back when we integrate host.
    // Also, this syntax heavily implies you can have more than one ConsoleApplication, and with this approach, you can't
    //public static void Main(string[] args)
    //{
    //    var app = ConsoleApplication.Create();
    //    app.SetRootCommand(Handlers.Franchise);
    //    app.RootCommand.AddCommand(Handlers.StarTrek);
    //    app.RootCommand.StarTrekCommand.AddCommand(Handlers.NextGeneration);
    //    app.RootCommand.StarTrekCommand.NextGenerationCommand.AddCommand(Handlers.DeepSpaceNine);
    //    app.RootCommand.StarTrekCommand.NextGenerationCommand.AddCommand(Handlers.Voyager);

    //    app.RootCommand.AddValidator(Validators.ValidatePoliteness, app.RootCommand.GreetingArgument);

    //    var nextGen = app.RootCommand.StarTrekCommand.NextGenerationCommand;
    //    nextGen.AddValidator(NextGenerationResultValidator);
    //    nextGen.PicardOption.AddAlias("-p");

    //    app.Run(args);

    //    // exampleoutput nextgeneration --foo voyager --greeting Hello -- janeway

    //}

    // After a design session with Benson Joeris, I think we may be able to do an instance based approach 
    // with guardrails against reusing the savee variable enforced via return types. I still see dragons here
    // and think it may be an order of magnitude more difficult  than the generic approach. more notes:
    // THere also may be more Intellisense weirdnesses here.
    //So, if AddCommands returns the new type, and AddCommand only works on leaves,
    //to block variance, then this code will not compile
    //var starTrek = cli.AddCommand(Handlers.StarTrek);
    //var nextGeneration = starTrek.AddCommand(Handlers.NextGeneration);
    //var voyager = nextGeneration.AddCommand(Handlers.Voyager);
    //var deepSpaceNine = nextGeneration.AddCommand(Handlers.DeepSpaceNine);

    //cli.Startrek.AddCommand(Handlers.NextGeneration);
    ////cli.Startrek.NextGeneration. // at this point Intellisense is to returning object, after line complete, strong type return
    //cli.Startrek.NextGeneration.AddCommand(Handlers.Voyager);
    //cli.Startrek.NextGeneration.AddCommand(Handlers.DeepSpaceNine);

    //cli.Startrek.AddCommand(Handlers.NextGeneration);
    //nextGeneration.AddCommands(Handlers.DeepSpaceNine, Handlers.Voyager);
}