using Jackfruit.IncrementalGenerator;
using Jackfruit.IntegrationTests;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace Jackfruit.Tests
{

    public class IntegrationTests : IClassFixture<JackfruitIntegrationTestFixture>
    {
        private JackfruitIntegrationTestFixture support;

        public IntegrationTests(JackfruitIntegrationTestFixture support)
        {
            this.support = support;
            var inputCompilation = support.CreateCompilation(
                support.TreeFromFileInInputPath("Handlers.cs"),
                support.TreeFromFileInInputPath("Validators.cs"),
                support.TreeFromFileInInputPath("Program.cs"));
            var outputCompilation = support.TestGeneration(inputCompilation, new Generator());
            support.TestOutput(outputCompilation);
            support.TestOutputCompiles();
        }
        [Fact]
        public void Simple_uhura()
        {
            var output = support.RunProject("StarTrek --Uhura");
            Assert.Equal($"Hello, Nyota Uhura{Environment.NewLine}", output);
        }

        [Fact]
        public void Nested_janeway()
        {
            var output = support.RunProject("StarTrek NextGeneration Voyager --Janeway");
            Assert.Equal($"Hello, Kathryn Janeway{Environment.NewLine}", output);
        }

        [Fact]
        public void Alias_picard()
        {
            var output = support.RunProject("StarTrek NextGeneration -p");
            Assert.Equal($"Hello, Jean-Luc Picard{Environment.NewLine}", output);
        }
    }
}
