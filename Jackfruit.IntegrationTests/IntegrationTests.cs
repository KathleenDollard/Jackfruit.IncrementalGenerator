using Xunit;
using Jackfruit;
using Jackfruit.IntegrationTests;
using DemoHandlers;

namespace Jackfruit.Tests
{
    public class IntegrationTests
    {


        [Fact]
        public void Invocation_outputs_correctly()
        {
            var cliTree = new CliTree();
            var franchise = Cli.Franchise;
            Assert.NotNull(franchise);
            Assert.NotNull(franchise.StarTrekCommand);
            Assert.NotNull(franchise.StarTrekCommand.NextGenerationCommand);
            Assert.NotNull(franchise.StarTrekCommand.NextGenerationCommand.VoyagerCommand);
            Assert.NotNull(franchise.StarTrekCommand.NextGenerationCommand.DeepSpaceNineCommand);
        }
    }
}
