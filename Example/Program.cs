using Jackfruit;
using Example;

Cli.Create(new(Handlers.Franchise, Validators.FranchiseValidate,
    new CliNode(Handlers.StarTrek,
        new CliNode(Handlers.NextGeneration,
            new CliNode(Handlers.DeepSpaceNine),
            new CliNode(Handlers.Voyager)
        ))));

Cli.Franchise.GreetingArgument.SetDefaultValue("Hello");
Cli.Franchise.Run(args);