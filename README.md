# Jackfruit.IncrementalGenerator

Please try this demo (temporary directions):

* Clone the repo
* Obtain the `IncrementalGenerator.Runtime` NuGet package. Build and place it package in a local source. 
* Obtain the `IncrementalGenerator Nuget` package. Build and place it package in a local source. 
* Create a new Console project
* Add the package `IncrementalGenerator` (the one you just built)
* Copy the file `DemoHandlers.cs` from the `Example` project. Warning: there are a few floating around, use that one first.
* Place this code in your sub main:

```c#
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
```

Build and run

Feedback welcome.