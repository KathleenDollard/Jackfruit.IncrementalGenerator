using Jackfruit.IncrementalGenerator;
using Jackfruit.Tests;
using Jackfruit.TestSupport;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CommandLine;
using System.Collections.Immutable;

namespace Jackfruit.IntegrationTests
{
    public class JackfruitIntegrationTestFixture
    {
        public E2EConfiguration Configuration { get; }

        public JackfruitIntegrationTestFixture()
            => Configuration = new();

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
            => IntegrationHelpers.RunGeneratedProject(arguments, Configuration.TestSetName, Configuration.TestBuildPath);

        //public CSharpCompilation CreateCompilation(params SyntaxTree[] syntaxTrees)
        //    => IntegrationHelpers.TestCreatingCompilation(syntaxTrees);

        public void CheckCompilation(Compilation compilation,
                                     IEnumerable<Diagnostic> diagnostics,
                                     Func<Diagnostic, bool>? diagnosticFilter = null,
                                     int? syntaxTreeCount = null)
        {
            Assert.NotNull(compilation);
            var filteredDiagnostics = diagnosticFilter is null
                ? diagnostics
                : diagnostics.Where(diagnosticFilter);
            Assert.Empty(filteredDiagnostics);
            if (syntaxTreeCount.HasValue)
            { Assert.Equal(syntaxTreeCount.Value, compilation.SyntaxTrees.Count()); }
        }

        public void OutputGeneratedTrees(Compilation generatedCompilation)
            => IntegrationHelpers.OutputGeneratedTrees(generatedCompilation,
                                                       Configuration.TestBuildPath,
                                                       new string[] { "Program.cs", "Handlers.cs", "Validators.cs" });

        public Process? CompileOutput()
            => IntegrationHelpers.CompileOutput(Configuration.TestBuildPath);

        public (CSharpCompilation compilation, ImmutableArray<Diagnostic> inputDiagnostics) 
            GetCompilation<T>(params SyntaxTree[] syntaxTrees) 
            where T : IIncrementalGenerator, new()
               {
            return TestHelpers.GetCompilation<T>(syntaxTrees);
        }

        internal (Compilation compilation, ImmutableArray<Diagnostic> inputDiagnostics) 
            RunGenerator(CSharpCompilation inputCompilation, Generator generator)
        {
            return TestHelpers.RunGenerator(inputCompilation, generator);
        }
    }
}