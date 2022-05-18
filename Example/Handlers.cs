using System.ComponentModel;

namespace Example
{
    public static class Handlers
    {
        private static void Greet(string greeting, string name)
        {
            //var defaultGreeting = "Hello";
            Console.WriteLine($"{greeting}, {name}");
            return;
        }

        public static void Franchise(string greetingArg)
            { }


            /// <summary>
            /// This is the description for StarTrek
            /// </summary>
            /// <param name="greetingArg">What greeting to use</param>
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
            public static void NextGeneration(
                string greetingArg, [Description("This is the description for Picard")] bool picard)
            {

                if (picard) { Greet(greetingArg, "Jean - Luc Picard"); }
            }

            public static void DeepSpaceNine(string greetingArg, bool sisko, bool odo, bool dax, bool worf, bool oBrien)
            {
                if (sisko) { Greet(greetingArg, "Benjamin Sisko"); }
                if (odo) { Greet(greetingArg, "Constable Odo"); }
                if (dax) { Greet(greetingArg, "Ezri Dax"); }
                if (worf) { Greet(greetingArg, "Worf"); }
                if (oBrien)
                {
                    Greet(greetingArg, "Miles O'Brien");
                }
            }
            public static void Voyager(string greetingArg, bool janeway, bool chakotay, bool torres, bool tuvok, bool sevenOfNine)
            {
                if (janeway) { Greet(greetingArg, "Kathryn Janeway"); }
                if (chakotay) { Greet(greetingArg, "Chakotay"); }
                if (torres)
                {
                    Greet(greetingArg, "B'Elanna Torres");
                }
                if (tuvok) { Greet(greetingArg, "Tuvok"); }
                if (sevenOfNine) { Greet(greetingArg, "Sevan of Nine"); }
            }
        }
    }
