using Jackfruit;
using DemoHandlers;

internal class Program
{
    private static void Main(string[] args)
    {
        Cli.Create(new CliNode(Handlers.Franchise, Validators.FranchiseValidate,
            new CliNode(Handlers.StarTrek,
                new CliNode(Handlers.NextGeneration,
                    new CliNode(Handlers.DeepSpaceNine),
                    new CliNode(Handlers.Voyager)
                ))));

        Cli.Franchise.GreetingArgument.SetDefaultValue("Hello");
        Cli.Franchise.Run(args);
    }
}

