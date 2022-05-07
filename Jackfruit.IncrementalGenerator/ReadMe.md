There are two problems generation needs to solve. Command existence and CLI tree, and command and symbol details. The second is relatively easy.
I have tried several things for the first.

Things i have tried before (note, validators are not yet tested):

### Static methods on types

This allows only a single CLI in the application. 

Console application has no real meaning.

There are types, delegates and properties with each of the names (such as StarTrek, StarTrekCommand, and Handlers.StarTrek)

```
ConsoleApplication.AddRootCommand(Handlers.StarTrek);
ConsoleApplication.StarTrek.AddSubCommand(Handlers.NextGeneration);
ConsoleApplication.StarTrek.NextGeneration.AddSubCommand(Handlers.DeepSpaceNine);
ConsoleApplication.StarTrek.NextGeneration.AddSubCommand(Handlers.Voyager);

var starTrek ConsoleApplication.StarTrekCommand.Create();
starTrek.KirkOption.AddAlias("" - k"");

ConsoleApplication.Run(args);
```

### Generic approach without console app

This has discoverability issues. 

```
var cliRoot = CliRoot.Create(Handlers.Franchise);
cliRoot.AddCommand(Handlers.StarTrek);
cliRoot.AddCommand<Commands.StarTrek>(Handlers.NextGeneration);
cliRoot.AddCommand<Commands.StarTrek.NextGeneration>(Handlers.DeepSpaceNine);
cliRoot.AddCommand<Commands.StarTrek.NextGeneration>(Handlers.Voyager);

var nextGen = cliRoot.StarTrekCommand.NextGenerationCommand.Create();
nextGen.PicardOption.AddAlias("" - p"");

cliRoot.AddValidator(Validators.ValidatePoliteness, cliRoot.GreetingArgument);
nextGen.AddValidator(NextGenerationResultValidator);

cliRoot.Run(args);

```

### Instance that I never got working

### Direct tree creation with lists

An simple abstract thing holds a list of CLIs and allows simple access in the common case of one CLI.

The part that is just for generation is isolated.

The tree looks like a tree.

This introducses problems of people wanting to create subparts of the tree. That is currently unsupported.

```
Cli.Create(new (Handlers.Franchise, new() {
    new(Handlers.StarTrek, new() {
        new(Handlers.NextGeneration, new() {
            new(Handlers.DeepSpaceNine),
            new(Handlers.Voyager) 
        })
    })
});

var root = Cli.Franchise.Create();

root.StarTrek.NextGeneration.PicardOption.AddAlias("" - p"");
root.Run(args); // Also, `Cli.Run(args0)` when there is only one root
```

### Direct tree creation with params

See direct tree with lists for notes

```
Cli.Create(new (Handlers.Franchise, 
    new(Handlers.StarTrek, 
        new(Handlers.NextGeneration,
            new(Handlers.DeepSpaceNine),
            new(Handlers.Voyager) 
        })
    })
});

var root = Cli.Franchise.Create();

root.StarTrek.NextGeneration.PicardOption.AddAlias("" - p"");
root.Run(args); // Also, `Cli.Run(args0)` when there is only one root
```