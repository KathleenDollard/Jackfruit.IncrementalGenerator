using Jackfruit.IncrementalGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using System.Diagnostics;

namespace Jackfruit.Tests
{
    public class IntegrationTests
    {

        private static CSharpCompilation TestCreatingCompilation(params SyntaxTree[] syntaxTrees)
        {
            var (compilation, inputDiagnostics) = TestHelpers.GetCompilation<Generator>(syntaxTrees);
            Assert.NotNull(compilation);
            // TODO: Figure out how to get the text from the span and compare with "Cli"
            var trouble = inputDiagnostics.Where(x => x.Id != "CS0103");
            Assert.Empty(trouble);
            return compilation;
        }


        private static Compilation TestGeneration<T>(CSharpCompilation compilation, T generator)
            where T : IIncrementalGenerator, new()
        {
            var (outputCompilation, outputDiagnostics) = TestHelpers.RunGenerator(compilation, generator);
            Assert.NotNull(outputCompilation);
            Assert.Empty(outputDiagnostics);
            Assert.Equal(6, outputCompilation.SyntaxTrees.Count());
            return outputCompilation;
        }

        private static void TestOutput(Compilation generatedCompilation)
        {
            foreach (var tree in generatedCompilation.SyntaxTrees)
            {
                var className = !string.IsNullOrWhiteSpace(tree.FilePath)
                        ? Path.GetFileName(tree.FilePath)
                        : tree.GetRoot().DescendantNodes()
                            .OfType<ClassDeclarationSyntax>()
                            .First()
                            .Identifier.ToString() + ".cs";
                if (className != "Program.cs" && className != "Handlers.cs" && className != "Validators.cs")
                {
                    var fileName = Path.Combine(testGeneratedCodePath, className);
                    File.WriteAllText(fileName, tree.ToString());
                }
            }
            Assert.Equal(3, Directory.GetFiles(testGeneratedCodePath).Count());
        }

        private const string dotnetVersion = "net6.0";
        private const string testSetName = "TestOutputExample";
        private const string testInputPath = @$"..\..\..\..\{testSetName}";
        private static string testGeneratedCodePath = Path.Combine(testInputPath, "GeneratedViaTest");
        private static string testBuildPath = Path.Combine(testInputPath, "bin", "Debug", dotnetVersion);
        private static string handlerFilePath = Path.Combine(testInputPath, "Handlers.cs");
        private static string validatorFilePath = Path.Combine(testInputPath, "Validators.cs");
        private static string programFilePath = Path.Combine(testInputPath, "Program.cs");

        private static SyntaxTree HandlerSyntaxTree => CSharpSyntaxTree.ParseText(File.ReadAllText(handlerFilePath));
        private static SyntaxTree ValidatorSyntaxTree => CSharpSyntaxTree.ParseText(File.ReadAllText(validatorFilePath));
        private static SyntaxTree ProgramSyntaxTree => CSharpSyntaxTree.ParseText(File.ReadAllText(programFilePath));
        private static Process? TestOutputCompiles()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            ////startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.WorkingDirectory = testInputPath;
            startInfo.FileName = "dotnet";
            startInfo.Arguments = "build";
            Process? exeProcess = Process.Start(startInfo);
            Assert.NotNull(exeProcess);
            if (exeProcess is not null)
            {
                exeProcess.WaitForExit();

                var output = exeProcess.StandardOutput.ReadToEnd();
                var error = exeProcess.StandardError.ReadToEnd();

                Assert.Equal(0, exeProcess.ExitCode);
                Assert.Equal("", error);
            }

            return exeProcess;
        }

        [Fact]
        public void Example_compiles()
        {
            var compilation = TestCreatingCompilation(ProgramSyntaxTree, HandlerSyntaxTree, ValidatorSyntaxTree);
            TestGeneration(compilation, new Generator());
        }

        [Fact]
        public void Example_outputs()
        {
            var compilation = TestCreatingCompilation(ProgramSyntaxTree, HandlerSyntaxTree, ValidatorSyntaxTree);
            var outputCompilation = TestGeneration(compilation, new Generator());
            TestOutput(outputCompilation);
        }

        [Fact]
        public void Example_output_builds()
        {
            var compilation = TestCreatingCompilation(ProgramSyntaxTree, HandlerSyntaxTree, ValidatorSyntaxTree);
            var outputCompilation = TestGeneration(compilation, new Generator());
            TestOutput(outputCompilation);
            TestOutputCompiles();
        }

        [Fact]
        public void Example_output_runs()
        {
            var compilation = TestCreatingCompilation(ProgramSyntaxTree, HandlerSyntaxTree, ValidatorSyntaxTree);
            var outputCompilation = TestGeneration(compilation, new Generator());
            TestOutput(outputCompilation);
            TestOutputCompiles();

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.FileName = $"{Path.Combine(testBuildPath, testSetName)}.exe";
            startInfo.Arguments = "StarTrek --Uhura";

            using Process? exeProcess = Process.Start(startInfo);
            Assert.NotNull(exeProcess);
            if (exeProcess is not null)
            {
                exeProcess.WaitForExit();

                var output = exeProcess.StandardOutput.ReadToEnd();
                var error = exeProcess.StandardError.ReadToEnd();

                Assert.Equal(0, exeProcess.ExitCode);
                Assert.Equal("Hello, Nyota Uhura\r\n", output);
                Assert.Equal("", error);
            }
        }
    }
}
