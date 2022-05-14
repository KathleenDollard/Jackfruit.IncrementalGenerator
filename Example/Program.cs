using Jackfruit;
using DemoHandlers;

Cli.Create(new(Handlers.Franchise, 
        new CliNode(Handlers.StarTrek, 
            new CliNode(Handlers.NextGeneration, 
                new CliNode(Handlers.DeepSpaceNine),
                new CliNode(Handlers.Voyager)
        )
    )
));

Cli.Franchise.Run(args);