using Xunit;
//using VerifyCS = CSharpSourceGeneratorVerifier<YourGenerator>;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VerifyXunit;
using System.Threading.Tasks;
using Jackfruit.IncrementalGenerator;
using System.Collections.Immutable;
using System.Collections.Generic;
using Jackfruit.TestSupport;
using Jackfruit.Internal;
using System;

namespace Jackfruit.Tests
{
    [UsesVerify]
    public class StartTrekCliTests
    {
        private static (ImmutableArray<Diagnostic> InputDiagnostics, ImmutableArray<Diagnostic> Diagnostics, string Output) 
            GetGeneratedOutput<T>(string source)
            where T : IIncrementalGenerator, new()
        {
            var syntaxTrees = new List<SyntaxTree>
            {
                CSharpSyntaxTree.ParseText(source),
                //CSharpSyntaxTree.ParseText(common),
                CSharpSyntaxTree.ParseText(StarTrekTestData.StarTrek)
            };
            return TestHelpers.GetGeneratedOutput<T>(syntaxTrees);
        }

        private const string common = @"
using Jackfruit.Internal;
using System;

namespace Jackfruit
    {
        /// <summary>
        /// This is the main class for the Jackfruit generator. After you call the 
        /// Create command, the returned RootCommand will contain your CLI. If you 
        /// need multiple root commands in your application differentiate them with &gt;T&lt;
        /// </summary>
        public partial class RootCommand 
        {
            public static RootCommand Create(params SubCommand[] subCommands)
                => new RootCommand();

            public static RootCommand Create(Delegate runHandler, params SubCommand[] subCommands)
                => new RootCommand();

            public static RootCommand Create(Delegate runHandler, Delegate validator, params SubCommand[] subCommands)
                => new RootCommand();

            public partial class Result
            { }
        }
    }
";

    private const string VoyagerRoot =@"
using DemoHandlers;
using Jackfruit;

public class MyClass
{
    public void F() 
    {
        var rootCommand = RootCommand.Create(Handlers.Voyager);
    }

}";

        private const string VoyagerRootSingleSubCommand = @"
using DemoHandlers;
using Jackfruit;

public class MyClass
{
    public void F() 
    {
        var rootCommand = RootCommand.Create(SubCommand.Create(Handlers.Voyager));
    }

}";


        private const string NextGenerationRoot = @"
using DemoHandlers;
using Jackfruit;

public class MyClass
{
    public void F() 
    {
        var rootCommand = RootCommand.Create(Handlers.NextGeneration,SubCommand.Create(Handlers.Voyager), SubCommand.Create(Handlers.DeepSpaceNine));
    }
}";


        private const string StarTrekRoot = @"
using DemoHandlers;
using Jackfruit;

public class MyClass
{
    public void F() 
    {
        var rootCommand = RootCommand.Create(Handlers.Franchise, 
            SubCommand.Create(Handlers.StarTrek, 
                SubCommand.Create(Handlers.NextGeneration, 
                    SubCommand.Create(Handlers.DeepSpaceNine),
                    SubCommand.Create(Handlers.Voyager) 
                )));
   }

}";
    
        private const string StarTrekRootWithValidator = @"
using DemoHandlers;
using Jackfruit;

public class MyClass
{
    public void F() 
    {
        var rootCommand = RootCommand.Create(Handlers.Franchise, Validators.FranchiseValidate,
            SubCommand.Create(Handlers.StarTrek, 
                SubCommand.Create(Handlers.NextGeneration, 
                    SubCommand.Create(Handlers.DeepSpaceNine),
                    SubCommand.Create(Handlers.Voyager) 
                )));
   }

}";

        [Theory]
        [InlineData("Voyager", VoyagerRoot)]
        [InlineData("VoyagerRootSingleSubCommand", VoyagerRootSingleSubCommand)]
        [InlineData("NextGeneration", NextGenerationRoot)]
        [InlineData("StarTrek", StarTrekRoot)]
        [InlineData("StarTrekRootWithValidator", StarTrekRootWithValidator)]
        public Task Can_create_commandDef(string fileName, string input)
        {
            var (inputDiagnostics, diagnostics, output) = GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Single(TestHelpers.WarningAndErrors(inputDiagnostics));
            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("StarTrekSnapshots").UseTextForParameters(fileName);
        }

        [Theory]
        [InlineData("Voyager", VoyagerRoot)]
        [InlineData("VoyagerRootSingleSubCommand", VoyagerRootSingleSubCommand)]
        [InlineData("NextGeneration", NextGenerationRoot)]
        [InlineData("StarTrek", StarTrekRoot)]
        [InlineData("StarTrekRootWithValidator", StarTrekRootWithValidator)]
        public Task Can_Generate(string fileName, string input)
        {
            var (inputDiagnostics, diagnostics, output) = GetGeneratedOutput<Generator>(input);

            Assert.Single(TestHelpers.WarningAndErrors(inputDiagnostics));
            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("StarTrekSnapshots").UseTextForParameters(fileName);
        }

        [Fact]
        public Task Command_descrption_from_xml_comment()
        {
            const string input = StarTrekRoot;
            var (inputDiagnostics, diagnostics, output) = GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Single(TestHelpers.WarningAndErrors(inputDiagnostics));
            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("StarTrekSnapshots");
        }

        [Fact]
        public Task Command_descrption_from_attribute()
        {
            const string input = NextGenerationRoot;
            var (inputDiagnostics, diagnostics, output) = GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Single(TestHelpers.WarningAndErrors(inputDiagnostics));
            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("StarTrekSnapshots");
        }

    }
}

