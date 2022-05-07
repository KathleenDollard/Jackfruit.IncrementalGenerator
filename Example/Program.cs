using Jackfruit;
using DemoHandlers;

Cli.Create(new(Handlers.Franchise, new() {
    new(Handlers.StarTrek, new()
    {
        new(Handlers.NextGeneration, new()
        {
            new (Handlers.DeepSpaceNine),
            new(Handlers.Voyager)
        })
    })
}));


Cli.Franchise.Run(args);