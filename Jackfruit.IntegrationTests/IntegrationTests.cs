using Xunit;
using Jackfruit;

namespace Jackfruit.Tests
{
    public class IntegrationTests
    {


        [Fact]
        public void Invocation_outputs_correctly()
        {
            var starTrekApp = ConsoleApplication.Create();
            Assert.NotNull(starTrekApp);
            Assert.NotNull(starTrekApp.CliRoot);
            Assert.NotNull(starTrekApp.CliRoot.NextGeneration);
            Assert.NotNull(starTrekApp.CliRoot.NextGeneration.DeepSpaceNine);
            //Assert.NotNull(starTrekApp.CliRoot.NextGeneration.Voyager);
        }
    }
}
