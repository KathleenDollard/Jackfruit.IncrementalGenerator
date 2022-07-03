using Jackfruit.IncrementalGenerator;
using Jackfruit.TestSupport;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace Jackfruit.Tests
{
    public static class IntegrationHelpers
    {
        internal const string dotnetVersion = "net6.0";
        internal static string currentPath = Environment.CurrentDirectory;

        public static CSharpCompilation TestCreatingCompilation(params SyntaxTree[] syntaxTrees)
        {
            var (compilation, inputDiagnostics) = TestHelpers.GetCompilation<Generator>(syntaxTrees);
            Assert.NotNull(compilation);
            // TODO: Figure out how to get the text from the span and compare with "Cli"
            var trouble = inputDiagnostics.Where(x => x.Id != "CS0103");
            Assert.Empty(trouble);
            return compilation;
        }

        public static Compilation TestGeneration<T>(CSharpCompilation compilation, T generator)
            where T : IIncrementalGenerator, new()
        {
            var (outputCompilation, outputDiagnostics) = TestHelpers.RunGenerator(compilation, generator);
            Assert.NotNull(outputCompilation);
            Assert.Empty(outputDiagnostics);
            Assert.Equal(9, outputCompilation.SyntaxTrees.Count());
            return outputCompilation;
        }

        public static void OutputGeneratedTrees(Compilation generatedCompilation, string outputDir, params string[] skipFiles)
        {
            foreach (var tree in generatedCompilation.SyntaxTrees)
            {
                var className = !string.IsNullOrWhiteSpace(tree.FilePath)
                        ? Path.GetFileName(tree.FilePath)
                        : tree.GetRoot().DescendantNodes()
                            .OfType<ClassDeclarationSyntax>()
                            .First()
                            .Identifier.ToString() + ".cs";
                if (skipFiles.Contains(className) || skipFiles.Contains(Path.GetFileNameWithoutExtension(className)))
                {
                    var fileName = Path.Combine(outputDir, className);
                    File.WriteAllText(fileName, tree.ToString());
                }
            }
            Assert.Equal(6, Directory.GetFiles(outputDir).Count());
        }

        public static Process? TestOutputCompiles(string testInputPath)
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
                exeProcess.WaitForExit(30000);

                var output = exeProcess.StandardOutput.ReadToEnd();
                var error = exeProcess.StandardError.ReadToEnd();

                Assert.Equal(0, exeProcess.ExitCode);
                Assert.Equal("", error);
            }

            return exeProcess;
        }

        public static string IfOsIsWindows(string windowsString, string unixString)
    => Environment.OSVersion.Platform == PlatformID.Unix
        ? unixString
        : windowsString;

        public static string? RunGeneratedProject(string arguments, string setName, string buildPath)
        {

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            var fieName = Path.Combine(buildPath, setName);

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
    }

    //public class IntegrationTests : IClassFixture<FranchiseFixture>
    //{



    //    [Fact]
    //    public void Simple_uhura()
    //    {
    //        var output = RunGeneratedProject("StarTrek --Uhura");
    //        Assert.Equal($"Hello, Nyota Uhura{Environment.NewLine}", output);
    //    }


    //    [Fact]
    //    public void Nested_janeway()
    //    {
    //        var output = RunGeneratedProject("StarTrek NextGeneration Voyager --Janeway");
    //        Assert.Equal($"Hello, Kathryn Janeway{Environment.NewLine}", output);
    //    }

    //    [Fact]
    //    public void Alias_picard()
    //    {
    //        var output = RunGeneratedProject("StarTrek NextGeneration -p");
    //        Assert.Equal($"Hello, Jean-Luc Picard{Environment.NewLine}", output);
    //    }
    //}
}
