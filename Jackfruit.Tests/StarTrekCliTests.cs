﻿using Xunit;
//using VerifyCS = CSharpSourceGeneratorVerifier<YourGenerator>;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VerifyXunit;
using System.Threading.Tasks;
using Jackfruit.IncrementalGenerator;
using System.Collections.Immutable;
using System.Collections.Generic;
using Jackfruit.TestSupport;

namespace Jackfruit.Tests
{
    [UsesVerify]
    public class StartTrekCliTests
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

        private const string VoyagerRoot =@"
using DemoHandlers;
using Jackfruit;

public class MyClass
{
    public void F() 
    {
        var rootCommand = RootCommand.Create(CommandNode.Create(Handlers.Voyager));
    }

}";

        private const string StarTrekRoot = @"
using DemoHandlers;
using Jackfruit;

public class MyClass
{
    public void F() 
    {
        var rootCommand = RootCommand.Create(CommandNode.Create(Handlers.Franchise, 
            CommandNode.Create(Handlers.StarTrek, 
                CommandNode.Create(Handlers.NextGeneration, 
                    CommandNode.Create(Handlers.DeepSpaceNine),
                    CommandNode.Create(Handlers.Voyager) 
                ))));

        var nextGen = cliRoot.StarTrekCommand.NextGenerationCommand.Create();
        nextGen.PicardOption.AddAlias("" - p"");

        cliRoot.AddValidator(Validators.ValidatePoliteness, cliRoot.GreetingArgument);
        nextGen.AddValidator(NextGenerationResultValidator);
   }

}";
    
        private const string StarTrekRootWithValidator = @"
using DemoHandlers;
using Jackfruit;

public class MyClass
{
    public void F() 
    {
        var rootCommand = RootCommand.Create(CommandNode.Create(Handlers.Franchise, Validators.FranchiseValidate,
            CommandNode.Create(Handlers.StarTrek, 
                CommandNode.Create(Handlers.NextGeneration, 
                    CommandNode.Create(Handlers.DeepSpaceNine),
                    CommandNode.Create(Handlers.Voyager) 
                ))));

        var nextGen = cliRoot.StarTrekCommand.NextGenerationCommand.Create();
        nextGen.PicardOption.AddAlias("" - p"");

        cliRoot.AddValidator(Validators.ValidatePoliteness, cliRoot.GreetingArgument);
        nextGen.AddValidator(NextGenerationResultValidator);
   }

}";


        private const string NextGenerationRoot = @"
using DemoHandlers;
using Jackfruit;

public class MyClass
{
    public void F() 
    {
        var rootCommand = RootCommand.Create(CommandNode.Create(Handlers.NextGeneration));
    }

}";

        [Theory]
        [InlineData("Voyager", VoyagerRoot)]
        [InlineData("NextGeneration", NextGenerationRoot)]
        [InlineData("StarTrek", StarTrekRoot)]
        [InlineData("StarTrekRootWithValidator", StarTrekRootWithValidator)]
        public Task Can_create_commandDef(string fileName, string input)
        {
            var (diagnostics, output) = GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("StarTrekSnapshots").UseTextForParameters(fileName);
        }

        [Theory]
        [InlineData("Voyager", VoyagerRoot)]
        [InlineData("NextGeneration", NextGenerationRoot)]
        [InlineData("StarTrek", StarTrekRoot)]
        [InlineData("StarTrekRootWithValidator", StarTrekRootWithValidator)]
        public Task Can_Generate(string fileName, string input)
        {
            var (diagnostics, output) = GetGeneratedOutput<Generator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("StarTrekSnapshots").UseTextForParameters(fileName);
        }

        [Fact]
        public Task Command_descrption_from_xml_comment()
        {
            const string input = StarTrekRoot;
            var (diagnostics, output) = GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("StarTrekSnapshots");
        }

        [Fact]
        public Task Command_descrption_from_attribute()
        {
            const string input = NextGenerationRoot;
            var (diagnostics, output) = GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("StarTrekSnapshots");
        }

    }
}

