using Jackfruit.IncrementalGenerator;
using Jackfruit.IntegrationTests;
using Jackfruit.TestSupport;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Reflection.Emit;

namespace Jackfruit.Tests
{

    public class IntegrationTests : IClassFixture<JackfruitIntegrationTestFixture>
    {
        private JackfruitIntegrationTestFixture support;

        public IntegrationTests(JackfruitIntegrationTestFixture support)
        {
            this.support = support;

            var (inputCompilation, inputDiagnostics) = support.GetCompilation<Generator>(
                support.TreeFromFileInInputPath("Handlers.cs"),
                support.TreeFromFileInInputPath("Validators.cs"),
                support.TreeFromFileInInputPath("Program.cs"));
            support.CheckCompilation(inputCompilation, inputDiagnostics, diagnosticFilter: x => x.Id != "CS0103");

            var (outputCompilation, outputDiagnostics) = support.RunGenerator(inputCompilation, new Generator());
            support.CheckCompilation(outputCompilation, outputDiagnostics, syntaxTreeCount: 9);

            support.OutputGeneratedTrees(outputCompilation);
            var exeProcess = support.CompileOutput();
            Assert.NotNull(exeProcess);
            Assert.True(exeProcess.HasExited);

            var output = exeProcess.StandardOutput.ReadToEnd(); // available for debugging - can be a pain to get in VS
            var error = exeProcess.StandardError.ReadToEnd();

            Console.WriteLine(output);

            Assert.Equal(0, exeProcess.ExitCode);
            Assert.Equal("", error);
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
