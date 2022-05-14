using Xunit;
using Jackfruit;
using DemoHandlers;

namespace Jackfruit.Tests
{
    public class IntegrationTests
    {


        [Fact]
        public void Invocation_outputs_correctly()
        {
            var franchise = Cli.Franchise;
            Assert.NotNull(franchise);
            Assert.NotNull(franchise.StarTrek);
            Assert.NotNull(franchise.StarTrek.NextGeneration);
            Assert.NotNull(franchise.StarTrek.NextGeneration.Voyager);
            Assert.NotNull(franchise.StarTrek.NextGeneration.DeepSpaceNine);
        }
    }
}
