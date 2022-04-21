# Jackfruit.IncrementalGenerator

Please try this demo:

* (Temporary) Obtain the NuGet package. Either I sent it to you or you can build and place the package in a local source. 
* Create a new package
* Add the package `Jackfruit.IncrementalGenerator`
* Replace `program.cs` with this code:

```c#
using DemoHandlers;

ConsoleApplication.AddRootCommand(Handlers.StarTrek);
ConsoleApplication.StarTrek.AddSubCommand(Handlers.NextGeneration);
ConsoleApplication.StarTrek.NextGeneration.AddSubCommand(Handlers.DeepSpaceNine);
ConsoleApplication.StarTrek.NextGeneration.AddSubCommand(Handlers.Voyager);

// if you do not want to customize anything
ConsoleApplication.Run(args);
```

* Add a new file `Handlers.cs` and add this code:

```c#
using System.ComponentModel;
namespace DemoHandlers
{
    public static class Handlers
    {
        private static void Greet(string greeting, string name)
        {
            var defaultGreeting = "Hello";
            Console.WriteLine($"{greeting ?? defaultGreeting},  {name} ");
            return;
        }
        /// <summary>
        /// This is the description for StarTrek
        /// </summary>
        /// <param name="greeting">What greeting to use</param>
        /// <param name="kirk">Whether to greek Captain Kirk</param>
        /// <param name="spock">Whether to greek Spock</param>
        /// <param name="uhura">Whether to greek Lieutenant Uhura</param>
        public static void StarTrek(string greetingArg, bool kirk, bool spock, bool uhura)
        {
            if (kirk) { Greet(greetingArg, "James T.Kirk"); }
            if (spock) { Greet(greetingArg, "Spock"); }
            if (uhura) { Greet(greetingArg, "Nyota Uhura"); }

        }
        [Description("This is the description for Next Generation")]
        public static void NextGeneration(string greetingArg, [Description("This is the description for Picard")] bool picard)
        {
            if (picard) { Greet(greetingArg, "Jean - Luc Picard"); }
        }
        public static void DeepSpaceNine(string greeting, bool sisko, bool odo, bool dax, bool worf, bool oBrien)
        {
            if (sisko) { Greet(greeting, "Benjamin Sisko"); }
            if (odo) { Greet(greeting, "Constable Odo"); }
            if (dax) { Greet(greeting, "Ezri Dax"); }
            if (worf) { Greet(greeting, "Worf"); }
            if (oBrien)
            {
                Greet(greeting, "Miles O'Brien");
            }
        }
        public static void Voyager(string greeting, bool janeway, bool chakotay, bool torres, bool tuvok, bool sevenOfNine)
        {
            if (janeway) { Greet(greeting, "Kathryn Janeway"); }
            if (chakotay) { Greet(greeting, "Chakotay"); }
            if (torres)
            {
                Greet(greeting, "B'Elanna Torres");
            }
            if (tuvok) { Greet(greeting, "Tuvok"); }
            if (sevenOfNine) { Greet(greeting, "Sevan of Nine"); }
        }
    }
}
```

* Build
* Run. Use Help to figure out what you want to do and ensure it all works