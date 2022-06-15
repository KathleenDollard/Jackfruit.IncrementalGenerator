# Jackfruit.IncrementalGenerator

Please try this demo (temporary directions) to check out this approach to try this System.CommandLine *flavor*:

1. Obtain a copy of the source and build (VS or CLI)
1. Play with the 3 user files that are in TestExample - Program, Handlers and Validators
1. If you make any changes, run the integration tests to try out (you might break them, which is ok)

## If you would like to try creating your own flavor (temp directions):

1. Fork the repo.
1. Copy and rename  the `Jackfruit.IncrementalGenerator` project
1. Create you flavor along with tests

As you find issues in the supporting projects, please file issues or PRs. Thanks for your patience. This process will get easier, for now I just wanted to make it possible to build flavors without starting from scratch.

## The current flavor in the rep:

```c#
using Jackfruit;
using DemoHandlers;

Cli.Create(new(Handlers.Franchise, 
    new CliNode(Handlers.StarTrek, 
        new CliNode(Handlers.NextGeneration, 
            new CliNode(Handlers.DeepSpaceNine),
            new CliNode(Handlers.Voyager)
        ))));


Cli.Franchise.Run(args);
```

Build and run

Feedback welcome.
