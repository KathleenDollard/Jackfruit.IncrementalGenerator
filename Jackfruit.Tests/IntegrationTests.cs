using Jackfruit.IncrementalGenerator;
using Jackfruit.TestSupport;
using System;
using System.Linq;
using Xunit;
using Microsoft.CodeAnalysis;

namespace Jackfruit.Tests
{

    public class IntegrationTests
    {
        private readonly IntegrationTestConfiguration testOutputExampleConfiguration =
            new("TestOutputExample")
            {
                OutputKind = OutputKind.ConsoleApplication 
            };

        private readonly IntegrationTestConfiguration testOutputEmptyConfiguration =
            new("TestOutputEmpty")
            {
                OutputKind = OutputKind.ConsoleApplication
            };

        private readonly IntegrationTestConfiguration testOutputSimpleConfiguration =
            new("TestOutputSimple")
            {
                OutputKind = OutputKind.ConsoleApplication
            };


        [Fact]
        public void Simple_uhura()
        {
            IntegrationHelpers.GenerateIntoProject<Generator>(testOutputExampleConfiguration);
            var output = IntegrationHelpers.RunCommand<Generator>(testOutputExampleConfiguration,
                                                                  "StarTrek --Uhura");
            Assert.Equal($"Hello, Nyota Uhura{Environment.NewLine}", output);
        }

        [Fact]
        public void Nested_janeway()
        {
            IntegrationHelpers.GenerateIntoProject<Generator>(testOutputExampleConfiguration);
            var output = IntegrationHelpers.RunCommand<Generator>(testOutputExampleConfiguration,
                                                                 "StarTrek NextGeneration Voyager --Janeway");
            Assert.Equal($"Hello, Kathryn Janeway{Environment.NewLine}", output);
        }

        [Fact]
        public void Alias_picard()
        {
            IntegrationHelpers.GenerateIntoProject<Generator>(testOutputExampleConfiguration);
            var output = IntegrationHelpers.RunCommand<Generator>(testOutputExampleConfiguration,
                                                                  "StarTrek NextGeneration -p");
            Assert.Equal($"Hello, Jean-Luc Picard{Environment.NewLine}", output);
        }

        [Fact]
        public void EmptyProject()
        {
            IntegrationHelpers.GenerateIntoProject<Generator>(testOutputEmptyConfiguration);
            var output = IntegrationHelpers.RunCommand<Generator>(testOutputEmptyConfiguration,
                                                                  "StarTrek NextGeneration -p");
            Assert.Equal($"Hello, World!{Environment.NewLine}", output);
        }

        [Fact]
        public void SimpleProject()
        {
            IntegrationHelpers.GenerateIntoProject<Generator>(testOutputSimpleConfiguration);
            var output = IntegrationHelpers.RunCommand<Generator>(testOutputSimpleConfiguration,
                                                                  "Hello --To George");
            Assert.Equal($"Hello, George!{Environment.NewLine}", output);
        }
    }
}
