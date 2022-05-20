using Jackfruit;
using DemoHandlers;

Cli.Create(new(Handlers.Franchise, Validators.FranchiseValidate,
    new CliNode(Handlers.StarTrek,
        new CliNode(Handlers.NextGeneration,
            new CliNode(Handlers.DeepSpaceNine),
            new CliNode(Handlers.Voyager)
        ))));

Cli.Franchise.Run(args);



