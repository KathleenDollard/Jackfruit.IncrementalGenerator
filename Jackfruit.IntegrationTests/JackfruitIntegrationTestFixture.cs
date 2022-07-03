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

namespace Jackfruit.IntegrationTests
{
    public abstract class JackfruitIntegrationTestFixture
    {
        public E2EConfiguration Configuration { get; }

        public JackfruitIntegrationTestFixture()
            => Configuration = new();

        public string TestSetName
        {
            get => Configuration.TestSetName;
            set => Configuration.TestSetName = value;
        }

        public string? RunProject(string arguments)
            => IntegrationHelpers.RunGeneratedProject(arguments, Configuration.TestSetName, Configuration.TestBuildPath);

        public CSharpCompilation TestCreatingCompilation(params SyntaxTree[] syntaxTrees)
            => IntegrationHelpers.TestCreatingCompilation(syntaxTrees);

        public Compilation TestGeneration<T>(CSharpCompilation compilation, T generator)
            where T : IIncrementalGenerator, new()
            => IntegrationHelpers.TestGeneration<T>(compilation, generator);

        public void OutputGeneratedTrees(Compilation generatedCompilation)
            => IntegrationHelpers.OutputGeneratedTrees(generatedCompilation,
                                                       Configuration.TestBuildPath,
                                                       new string[] { "Program.cs", "Handlers.cs", "Validators.cs" });

        public Process? TestOutputCompiles()
            => IntegrationHelpers.TestOutputCompiles(Configuration.TestBuildPath);
    }
}