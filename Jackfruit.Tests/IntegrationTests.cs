using Jackfruit.IncrementalGenerator;
using Jackfruit.TestSupport;
using System;
using System.Linq;
using Xunit;

namespace Jackfruit.Tests
{

    public class IntegrationTests : IClassFixture<IntegrationTestFixture>
    {
        private IntegrationTestFixture support;

        public IntegrationTests(IntegrationTestFixture support)
        {
            this.support = support;
        }

        //public string? RunCommand(string testSetName, string arguments, params string[] fileNames)
        //{
        //    if (string.IsNullOrWhiteSpace(testSetName))
        //    {  support.}
        //    var (inputCompilation, inputDiagnostics) = IntegrationTestFixture.GetCompilation<Generator>(
        //        fileNames.Select(fileName => support.TreeFromFileInInputPath(fileName)).ToArray());
        //    IntegrationTestFixture.CheckCompilation(inputCompilation, inputDiagnostics, diagnosticFilter: x => x.Id != "CS0103");

        //    var (outputCompilation, outputDiagnostics) = IntegrationTestFixture.RunGenerator<Generator>(inputCompilation);
        //    IntegrationTestFixture.CheckCompilation(outputCompilation, outputDiagnostics, syntaxTreeCount: 9);

        //    support.OutputGeneratedTrees(outputCompilation);
        //    var exeProcess = support.CompileOutput();
        //    Assert.NotNull(exeProcess);
        //    Assert.True(exeProcess.HasExited);

        //    var output = exeProcess.StandardOutput.ReadToEnd(); // available for debugging - can be a pain to get in VS
        //    var error = exeProcess.StandardError.ReadToEnd();

        //    Console.WriteLine(output);

        //    Assert.Equal(0, exeProcess.ExitCode);
        //    Assert.Equal("", error);

        //    support.RunProject(arguments);
        //}
        private string TestOutputExample="TestOutputExample";
        private string[] filesTestOutputExample = new string[] { "Handlers.cs", "Validators.cs", "Program.cs" };
        private string TestOutputEmpty = "TestOutputEmpty";
        private string[] filesTestOutputEmpty = new string[] { "Program.cs" };

        [Fact]
        public void Simple_uhura()
        {
            var output = IntegrationHelpers.RunCommand<Generator>(TestOutputExample,
                                                                  "StarTrek --Uhura",
                                                                  filesTestOutputExample);
            Assert.Equal($"Hello, Nyota Uhura{Environment.NewLine}", output);
        }

        [Fact]
        public void Nested_janeway()
        {
            var output = IntegrationHelpers.RunCommand<Generator>(TestOutputExample,
                                                                 "StarTrek NextGeneration Voyager --Janeway",
                                                                 filesTestOutputExample);
            Assert.Equal($"Hello, Kathryn Janeway{Environment.NewLine}", output);
        }

        [Fact]
        public void Alias_picard()
        {
            var output = IntegrationHelpers.RunCommand<Generator>(TestOutputExample,
                                                                  "StarTrek NextGeneration -p",
                                                                  filesTestOutputExample);
            Assert.Equal($"Hello, Jean-Luc Picard{Environment.NewLine}", output);
        }


        [Fact]
        public void EmptyProject()
        {
            var output = IntegrationHelpers.RunCommand<Generator>(TestOutputEmpty,
                                                                  "StarTrek NextGeneration -p",
                                                                  filesTestOutputExample);
            Assert.Equal($"Hello, Jean-Luc Picard{Environment.NewLine}", output);
        }
    }
}
