using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Diagnostics;
using System.Collections.Immutable;
using Xunit;

namespace Jackfruit.TestSupport
{
    public class IntegrationTestFixture
    {
        public IntegrationTestConfiguration Configuration { get; }

        public IntegrationTestFixture()
            => Configuration = new();

        public IntegrationTestFixture(string testSetName)
        {
            TestSetName = testSetName;
        }

        public string TestSetName
        {
            get => Configuration.TestSetName;
            set => Configuration.TestSetName = value;
        }

        public SyntaxTree TreeFromFileInInputPath(string fileName)
        {
            fileName = fileName.EndsWith(".cs")
                ? Path.Combine(Configuration.TestInputPath, fileName)
                : Path.Combine(Configuration.TestInputPath, fileName + ".cs");

            return CSharpSyntaxTree.ParseText(File.ReadAllText(fileName));
        }

        public string? RunProject(string arguments)
            => TestHelpers.RunGeneratedProject(arguments, Configuration.TestSetName, Configuration.TestBuildPath);

        //public CSharpCompilation CreateCompilation(params SyntaxTree[] syntaxTrees)
        //    => IntegrationHelpers.TestCreatingCompilation(syntaxTrees);

        public static void CheckCompilation(Compilation compilation,
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

        public void OutputGeneratedTrees(Compilation generatedCompilation)
            => TestHelpers.OutputGeneratedTrees(generatedCompilation,
                                                       Configuration.TestGeneratedCodePath,
                                                       new string[] { "Program.cs", "Handlers.cs", "Validators.cs" });

        public Process? CompileOutput()
            => TestHelpers.CompileOutput(Configuration.TestInputPath);

        public static (CSharpCompilation compilation, ImmutableArray<Diagnostic> inputDiagnostics) 
            GetCompilation<T>(params SyntaxTree[] syntaxTrees) 
            where T : IIncrementalGenerator, new()
               {
            return TestHelpers.GetCompilation<T>(syntaxTrees);
        }

        public static (Compilation compilation, ImmutableArray<Diagnostic> inputDiagnostics) 
            RunGenerator<T>(CSharpCompilation inputCompilation)
            where T : IIncrementalGenerator, new()
        {
            var generator = new T();
            return TestHelpers.RunGenerator(inputCompilation, generator);
        }
    }
}