using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Jackfruit.Tests;

public class Code
{
    private const string validator = @"
using System;
using System.Collections.Generic;

namespace DemoHandlers
{
    internal class Validators
    {
        public static IEnumerable<string> FranchiseValidate(string greeting)
        {
            var errors = new List<string>();
            if (greeting.Contains(""Poo"", StringComparison.OrdinalIgnoreCase))
            {
                errors.Add(""We do not say 'Poo' on this ship!"");
            }
            return errors;
        }
    }
}";

    private const string handler = @"
using System.ComponentModel;
using System;

namespace DemoHandlers
{
    public static class Handlers
    {
        private static void Greet(string greeting, string name)
        {
            var defaultGreeting = ""Hello"";
            Console.WriteLine($""{greeting ?? defaultGreeting}, {name}"");
            return;
        }

        public static void Franchise(string greetingArg)
        { }


        /// <summary>
        /// This is the description for StarTrek
        /// </summary>
        /// <param name=""greetingArg"">What greeting to use</param>
        /// <param name=""kirk"">Whether to greet Captain Kirk</param>
        /// <param name=""spock"">Whether to greet Spock</param>
        /// <param name=""uhura"">Whether to greet Lieutenant Uhura</param>
        public static void StarTrek(string greetingArg, bool kirk, bool spock, bool uhura)
        {
            if (kirk) { Greet(greetingArg, ""James T.Kirk""); }
            if (spock) { Greet(greetingArg, ""Spock""); }
            if (uhura) { Greet(greetingArg, ""Nyota Uhura""); }

        }

        [Description(""This is the description for Next Generation"")]
        public static void NextGeneration(
            string greetingArg, [Description(""This is the description for Picard"")] bool picard)
        {

            if (picard) { Greet(greetingArg, ""Jean - Luc Picard""); }
        }

        public static void DeepSpaceNine(string greetingArg, bool sisko, bool odo, bool dax, bool worf, bool oBrien)
        {
            if (sisko) { Greet(greetingArg, ""Benjamin Sisko""); }
            if (odo) { Greet(greetingArg, ""Constable Odo""); }
            if (dax) { Greet(greetingArg, ""Ezri Dax""); }
            if (worf) { Greet(greetingArg, ""Worf""); }
            if (oBrien)
            {
                Greet(greetingArg, ""Miles O'Brien"");
            }
        }
        public static void Voyager(string greetingArg, bool janeway, bool chakotay, bool torres, bool tuvok, bool sevenOfNine)
        {
            if (janeway) { Greet(greetingArg, ""Kathryn Janeway""); }
            if (chakotay) { Greet(greetingArg, ""Chakotay""); }
            if (torres)
            {
                Greet(greetingArg, ""B'Elanna Torres"");
            }
            if (tuvok) { Greet(greetingArg, ""Tuvok""); }
            if (sevenOfNine) { Greet(greetingArg, ""Sevan of Nine""); }
        }
    }
}";

    private const string program = @"
using Jackfruit;
using DemoHandlers;

internal class Program
{
    private static void Main(string[] args)
    {
        Cli.Create(new(Handlers.Franchise, Validators.FranchiseValidate,
    new CliNode(Handlers.StarTrek,
        new CliNode(Handlers.NextGeneration,
            new CliNode(Handlers.DeepSpaceNine),
            new CliNode(Handlers.Voyager)
        ))));

        Cli.Franchise.GreetingArgument.SetDefaultValue(""Hello"");
        Cli.Franchise.Run(args);
    }
}";

    internal static SyntaxTree ValidatorSyntaxTree =>
        CSharpSyntaxTree.ParseText(validator);
    internal static SyntaxTree HandlerSyntaxTree =>
    CSharpSyntaxTree.ParseText(handler);
    internal static SyntaxTree ProgramSyntaxTree =>
    CSharpSyntaxTree.ParseText(program);
}
