# Jackfruit.IncrementalGenerator

Please try this demo (temporary directions):

* Clone the repo (at this time "Obtain" means clone, build, pack, push local)
* Working in Visual Studio, ignore NU1202
* Obtain the `IncrementalGenerator.Runtime` NuGet package. Build and place it package in a local source. 
* Obtain the `IncrementalGenerator Nuget` package. Build and place it package in a local source. 
* Create a new Console project
* Add the package `IncrementalGenerator` (the one you just built-you will need to add the local feed)
* Copy the file `DemoHandlers.cs` from the `Example` project. Warning: there are a few floating around, use that one first.
* Place this code in your sub main:

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
