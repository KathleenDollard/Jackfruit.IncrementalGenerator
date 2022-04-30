using System.ComponentModel;

namespace DemoHandlersUpdated
{
    public static class Handlers
    {
        private static void Greet(string greeting, string name)
        {
            var defaultGreeting = "Hello";
            Console.WriteLine($"{greeting ?? defaultGreeting}, {name}");
            return;
        }
        /// <summary>
        /// This is the description for StarTrek
        /// </summary>
        /// <param name="greeting">What greeting to use</param>
        /// <param name="kirk">Whether to greet Captain Kirk</param>
        /// <param name="spock">Whether to greet Spock</param>
        /// <param name="uhura">Whether to greet Lieutenant Uhura</param>
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
                Greet(greeting, "Miles O'Brien"); }
            }
            public static void Voyager(string greeting, bool janeway, bool chakotay, bool torres, bool tuvok, bool sevenOfNine)
            {
                if (janeway) { Greet(greeting, "Kathryn Janeway"); }
                if (chakotay) { Greet(greeting, "Chakotay"); }
                if (torres)
                {
                    Greet(greeting, "B'Elanna Torres"); }
                if (tuvok) { Greet(greeting, "Tuvok"); }
                    if (sevenOfNine) { Greet(greeting, "Sevan of Nine"); }
                }
            }
        }
