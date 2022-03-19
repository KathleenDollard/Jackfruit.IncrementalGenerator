using Xunit;
//using VerifyCS = CSharpSourceGeneratorVerifier<YourGenerator>;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VerifyXunit;

using System.Threading;
using System.Threading.Tasks;
using Jackfruit.IncrementalGenerator;
using System.Collections.Immutable;
using System.Collections.Generic;

namespace Jackfruit.Tests
{
    [UsesVerify]
    public class StartTrekTests
    {
        private (ImmutableArray<Diagnostic> Diagnostics, string Output) GetGeneratedOutput<T>(string source)
            where T : IIncrementalGenerator, new()
        {
            var syntaxTrees = new List<SyntaxTree>
            {
                CSharpSyntaxTree.ParseText(source),
                CSharpSyntaxTree.ParseText(StarTrekTestData.StarTrek)
            };
            return TestHelpers.GetGeneratedOutput<T>(syntaxTrees);
        }

        [Fact]
        public Task Can_create_root()
        {
            const string input = @"
using DemoHandlers;
using Jackfruit;

public class MyClass
{
    public void F() 
    {
        ConsoleApplication.CreateWithRootCommand(Handlers.Voyager);
    }

}";
            var (diagnostics, output) = GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Can_find_xmlComment_command_descrption()
        {
            const string input = @"
using DemoHandlers;
using Jackfruit;

public class MyClass
{
    public void F() 
    {
        ConsoleApplication.CreateWithRootCommand(Handlers.StarTrek);
    }

}";
            var (diagnostics, output) = GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Can_find_attribute_command_descrption()
        {
            const string input = @"
using DemoHandlers;
using Jackfruit;

public class MyClass
{
    public void F() 
    {
        var app = ConsoleApplication.CreateWithRootCommand(Handlers.NextGeneration);
    }

}";
            var (diagnostics, output) = GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }
    }
}

