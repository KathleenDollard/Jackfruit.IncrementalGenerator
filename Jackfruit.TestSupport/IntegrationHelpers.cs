using Microsoft.CodeAnalysis;
using Xunit;

namespace Jackfruit.TestSupport
{
    public static class IntegrationHelpers
    {
        public static string? RunCommand<T>(string testSetName, string arguments, params string[] fileNames)
            where T : IIncrementalGenerator, new()
        {
            var support = new IntegrationTestFixture(testSetName);

            var (inputCompilation, inputDiagnostics) = IntegrationTestFixture.GetCompilation<T>(
                fileNames.Select(fileName => support.TreeFromFileInInputPath(fileName)).ToArray());
            IntegrationTestFixture.CheckCompilation(inputCompilation, inputDiagnostics, diagnosticFilter: x => x.Id != "CS0103");

            var (outputCompilation, outputDiagnostics) = IntegrationTestFixture.RunGenerator<T>(inputCompilation);
            IntegrationTestFixture.CheckCompilation(outputCompilation, outputDiagnostics, syntaxTreeCount: 9);

            support.OutputGeneratedTrees(outputCompilation);
            var exeProcess = support.CompileOutput();
            Assert.NotNull(exeProcess);
            Assert.True(exeProcess.HasExited);

            var output = exeProcess.StandardOutput.ReadToEnd(); // available for debugging - can be a pain to get in VS
            var error = exeProcess.StandardError.ReadToEnd();
            Console.WriteLine(output);
            Assert.Equal(0, exeProcess.ExitCode);
            Assert.Equal("", error);

            return support.RunProject(arguments);
        }
    }
}
