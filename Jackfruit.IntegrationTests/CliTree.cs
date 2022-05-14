using DemoHandlers;

namespace Jackfruit.IntegrationTests
{
    internal class CliTree
    {
        public CliTree()
        {
            Cli.Create(new CliNode(Handlers.Franchise,
                new CliNode(Handlers.StarTrek,
                    new CliNode(Handlers.NextGeneration,
                        new(Handlers.DeepSpaceNine),
                        new(Handlers.Voyager)
                    ))));
        }
    }
}
