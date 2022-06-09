using Jackfruit.IncrementalGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace Jackfruit.Tests
{
    public class FranchiseFixture
    {
        internal const string dotnetVersion = "net6.0";
        internal const string testSetName = "TestOutputExample";
        internal static string currentPath = Environment.CurrentDirectory;
        internal static string testInputPath = Path.Combine(currentPath, @$"../../../../{testSetName}");
        internal static string testGeneratedCodePath = Path.Combine(testInputPath, "GeneratedViaTest");
        internal static string testBuildPath = Path.Combine(testInputPath, "bin", "Debug", dotnetVersion);
        internal static string handlerFilePath = Path.Combine(testInputPath, "Handlers.cs");
        internal static string validatorFilePath = Path.Combine(testInputPath, "Validators.cs");
        internal static string programFilePath = Path.Combine(testInputPath, "Program.cs");

        private static SyntaxTree HandlerSyntaxTree => CSharpSyntaxTree.ParseText(File.ReadAllText(handlerFilePath));
        private static SyntaxTree ValidatorSyntaxTree => CSharpSyntaxTree.ParseText(File.ReadAllText(validatorFilePath));
        private static SyntaxTree ProgramSyntaxTree => CSharpSyntaxTree.ParseText(File.ReadAllText(programFilePath));

        private static CSharpCompilation TestCreatingCompilation(params SyntaxTree[] syntaxTrees)
        {
            var (compilation, inputDiagnostics) = TestHelpers.GetCompilation<Generator>(syntaxTrees);
            Assert.NotNull(compilation);
            // TODO: Figure out how to get the text from the span and compare with "Cli"
            var trouble = inputDiagnostics.Where(x => (x.Severity == DiagnosticSeverity.Warning || x.Severity == DiagnosticSeverity.Error) && x.Id != "CS0103");
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
                exeProcess.WaitForExit(10000);

                var output = exeProcess.StandardOutput.ReadToEnd();
                var error = exeProcess.StandardError.ReadToEnd();

                Assert.Equal(0, exeProcess.ExitCode);
                Assert.Equal("", error);
            }

            return exeProcess;
        }

        public FranchiseFixture()
        {
            var inputCompilation = TestCreatingCompilation(ProgramSyntaxTree, HandlerSyntaxTree, ValidatorSyntaxTree);
            var outputCompilation = TestGeneration(inputCompilation, new Generator());
            TestOutput(outputCompilation);
            TestOutputCompiles();
        }

    }

    public class IntegrationTests : IClassFixture<FranchiseFixture>
    {

        private static string IfOsIsWindows(string windowsString, string unixString)
            => Environment.OSVersion.Platform == PlatformID.Unix
                ? unixString
                : windowsString;

        private static string? RunGeneratedProject(string arguments)
        {

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            var fieName = Path.Combine(FranchiseFixture.testBuildPath, FranchiseFixture.testSetName);

            startInfo.FileName = $"{fieName}{IfOsIsWindows(".exe", "")}";
            startInfo.Arguments = arguments;

            Process? exeProcess = Process.Start(startInfo);
            Assert.NotNull(exeProcess);
            if (exeProcess is not null)
            {
                exeProcess.WaitForExit();

                var output = exeProcess.StandardOutput.ReadToEnd();
                var error = exeProcess.StandardError.ReadToEnd();

                Assert.Equal(0, exeProcess.ExitCode);
                Assert.Equal("", error);
                return output;
            }
            return null;
        }


        //[Fact]
        //public void Example_compiles()
        //{
        //    //var compilation = TestCreatingCompilation(ProgramSyntaxTree, HandlerSyntaxTree, ValidatorSyntaxTree);
        //    TestGeneration(inputCompilation, new Generator());
        //}

        //[Fact]
        //public void Example_outputs()
        //{
        //    var compilation = TestCreatingCompilation(ProgramSyntaxTree, HandlerSyntaxTree, ValidatorSyntaxTree);
        //    var outputCompilation = TestGeneration(compilation, new Generator());
        //    TestOutput(outputCompilation);
        //}

        //[Fact]
        //public void Example_output_builds()
        //{
        //    var compilation = TestCreatingCompilation(ProgramSyntaxTree, HandlerSyntaxTree, ValidatorSyntaxTree);
        //    var outputCompilation = TestGeneration(compilation, new Generator());
        //    TestOutput(outputCompilation);
        //    TestOutputCompiles();
        //}

        [Fact]
        public void Simple_uhura()
        {
            var output = RunGeneratedProject("StarTrek --Uhura");
            Assert.Equal($"Hello, Nyota Uhura{Environment.NewLine}", output);
        }


        [Fact]
        public void Nested_janeway()
        {
            var output = RunGeneratedProject("StarTrek NextGeneration Voyager --Janeway");
            Assert.Equal($"Hello, Kathryn Janeway{Environment.NewLine}", output);
        }

        [Fact]
        public void Alias_picard()
        {
            var output = RunGeneratedProject("StarTrek NextGeneration -p");
            Assert.Equal($"Hello, Jean-Luc Picard{Environment.NewLine}", output);
        }
    }
}
