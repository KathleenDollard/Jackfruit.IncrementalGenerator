using Jackfruit;
using Example;

Cli.Create(new(Handlers.Franchise,
    new CliNode(Handlers.StarTrek,
        new CliNode(Handlers.NextGeneration,
            new CliNode(Handlers.DeepSpaceNine),
            new CliNode(Handlers.Voyager)
        ))));

Cli.Franchise.GreetingArgument.SetDefaultValue("Hello");
Cli.Franchise.Run(args);