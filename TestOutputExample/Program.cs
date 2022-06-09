using Jackfruit;
using DemoHandlers;
using System.Runtime.CompilerServices;

internal class Program
{
    private static void Main(string[] args)
    {
        // TODO: Add conditional to generated output
        Cli.Create(new CliNode(Handlers.Franchise, Validators.FranchiseValidate,
            new CliNode(Handlers.StarTrek,
                new CliNode(Handlers.NextGeneration,
                    new CliNode(Handlers.DeepSpaceNine),
                    new CliNode(Handlers.Voyager)
                ))));

        Cli.Franchise.GreetingArgument.SetDefaultValue("Hello");
        Cli.Franchise.Run(args);


        #region 1
        //In the version below, the return value of Create changes due to generation - creepy!!
        //var franchise = Cli.Create(
        //       DefineCommand(Handlers.Franchise, Validators.FranchiseValidate,
        //          DefineCommand(Handlers.StarTrek,
        //             DefineCommand(Handlers.NextGeneration,
        //                 DefineCommand(Handlers.DeepSpaceNine),
        //                 DefineCommand(Handlers.Voyager)
        //             ))));

        #region 2
        //We may be able to do this. Cli.Create would return the base class of the user's Jackfruit command class
        //var root = Cli.Create(Handlers.Franchise, Validators.FranchiseValidate)
        //                    .Add(SubCommand(Handlers.StarTrek)
        //                        .Add(SubCommand(Handlers.NextGeneration)
        //                            .Add(SubCommand(Handlers.DeepSpaceNine),
        //                                 SubCommand(Handlers.Voyager))));
        //var franchise = root as Franchise;
        //franchise.GreetingArgument.SetDefaultValue("Hello");
        //root.Run(args);

        #region Other questions and comments
        // This is the successor to Dragonfruit. 
        // * The goal is to avoid forcing an understanding of SCL
        //
        // What do we call the wrapper for SCL command.
        // * Currently "Command" in docs and just the name in the code.
        // * It's options and args are the real ones
        // * It's handler is different and it has extra behavior
        //
        // What do we call the Result. 
        // * Currently "<name>.Result"
        // * We have a lot of things named result, but how to prefix this?
        //
        // Is there a better name for the entry point than "Cli"

    }

}
  namespace DemoHandlers
{
    public partial class Franchise
    {
        public Delegate Handler2 => Handlers.Franchise;
        public 
    }
}

