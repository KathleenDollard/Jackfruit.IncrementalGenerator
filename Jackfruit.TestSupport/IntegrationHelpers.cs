using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;
using System.Diagnostics;
using Xunit;

namespace Jackfruit.TestSupport
{
    public static partial class IntegrationHelpers
    {
        public static void GenerateIntoProject<T>(IntegrationTestConfiguration configuration)
            where T : IIncrementalGenerator, new()
        {
            SyntaxTree[] syntaxTrees = configuration.SourceFiles
                .Select(fileName => TreeFromFileInInputPath(configuration, fileName)).ToArray();
            var (inputCompilation, inputDiagnostics) = GetCompilation<T>(configuration, syntaxTrees);
            CheckCompilation(configuration, inputCompilation, inputDiagnostics, diagnosticFilter: x => x.Id != "CS0103");

            var (outputCompilation, outputDiagnostics) = RunGenerator<T>(inputCompilation);
            CheckCompilation(configuration, outputCompilation, outputDiagnostics, syntaxTreeCount: configuration.SyntaxTreeCount);

            OutputGeneratedTrees(configuration, outputCompilation);
            var exeProcess = CompileOutput(configuration);
            Assert.NotNull(exeProcess);
            Assert.True(exeProcess.HasExited);

            var output = exeProcess.StandardOutput.ReadToEnd(); // available for debugging - can be a pain to get in VS
            var error = exeProcess.StandardError.ReadToEnd();
            Console.WriteLine(output);
            Assert.Equal(0, exeProcess.ExitCode);
            Assert.Equal("", error);
        }

        public static string? RunCommand<T>(IntegrationTestConfiguration configuration, string arguments)
            where T : IIncrementalGenerator, new()
                => RunProject(configuration, arguments);


        public static SyntaxTree TreeFromFileInInputPath(IntegrationTestConfiguration configuration, string fileName)
        {
            fileName = fileName.EndsWith(".cs")
                ? Path.Combine(configuration.TestInputPath, fileName)
                : Path.Combine(configuration.TestInputPath, fileName + ".cs");

            return CSharpSyntaxTree.ParseText(File.ReadAllText(fileName));
        }

        public static string? RunProject(IntegrationTestConfiguration configuration, string arguments)
            => TestHelpers.RunGeneratedProject(arguments, configuration.TestSetName, configuration.TestBuildPath);

        //public CSharpCompilation CreateCompilation(params SyntaxTree[] syntaxTrees)
        //    => IntegrationHelpers.TestCreatingCompilation(syntaxTrees);

        public static void CheckCompilation(IntegrationTestConfiguration configuration,
                                            Compilation compilation,
                                            IEnumerable<Diagnostic> diagnostics,
                                            Func<Diagnostic, bool>? diagnosticFilter = null,
                                            int? syntaxTreeCount = null)
        {
            Assert.NotNull(compilation);
            var filteredDiagnostics = diagnosticFilter is null
                ? TestHelpers.WarningAndErrors(diagnostics)
                : TestHelpers.WarningAndErrors(diagnostics).Where(diagnosticFilter);
            Assert.Empty(filteredDiagnostics);
            if (syntaxTreeCount.HasValue)
            { Assert.Equal(syntaxTreeCount.Value, compilation.SyntaxTrees.Count()); }
        }

        public static void OutputGeneratedTrees(IntegrationTestConfiguration configuration, Compilation generatedCompilation)
            => TestHelpers.OutputGeneratedTrees(generatedCompilation,
                                                       configuration.TestGeneratedCodePath,
                                                       configuration.SourceFiles);

        public static Process? CompileOutput(IntegrationTestConfiguration configuration)
            => TestHelpers.CompileOutput(configuration.TestInputPath);

        private static (CSharpCompilation compilation, ImmutableArray<Diagnostic> inputDiagnostics)
            GetCompilation<T>(IntegrationTestConfiguration configuration, params SyntaxTree[] syntaxTrees)
            where T : IIncrementalGenerator, new()
            => TestHelpers.GetCompilation<T>(configuration.OutputKind, syntaxTrees);

        public static (Compilation compilation, ImmutableArray<Diagnostic> inputDiagnostics)
            RunGenerator<T>(CSharpCompilation inputCompilation)
            where T : IIncrementalGenerator, new()
        {
            var generator = new T();
            return TestHelpers.RunGenerator(inputCompilation, generator);
        }
    }
}
