using Xunit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VerifyXunit;
using System.Threading.Tasks;
using Jackfruit.IncrementalGenerator;

namespace Jackfruit.Tests
{
    [UsesVerify]
    public class CommandDefTests
    {

        [Fact]
        public Task Empty_code_doesnt_fail()
        {
            var compilation = CSharpCompilation.Create("name");
            Generator generator = new();

            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

            driver = driver.RunGenerators(compilation);

            return Verifier.Verify(driver).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Single_command_with_single_path_produces_output()
        {
            const string input = @"
using Jackfruit;
public class MyClass
{
    public void F() 
    {
        ConsoleApplication.CreateWithRootCommand(A);
    }

    public void A(int i) 
    {
    }

}";
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        /// <summary>
        /// This test is important for the AddCommand where the classes in the complex path are part of generation
        /// </summary>
        /// <returns></returns>
        [Fact]
        public Task Single_command_with_compound_path_produces_output()
        {
            const string input = @"
using Jackfruit;
public class MyClass
{
    public void F() 
    {
        MyClass.YesMine.Really.ConsoleApplication.CreateWithRootCommand(A);
    }
    public void A(int i) 
    {
    }
}";
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Single_command_with_null_delegate_has_no_output()
        {
            const string input = @"
using Jackfruit;
public class MyClass
{
    public void F() 
    {
        MyClass.YesMine.Really.ConsoleApplication.CreateWithRootCommand(null);
    }
}";
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }


        [Fact]
        public Task Method_with_other_names_has_no_output()
        {
            const string input = @"
using Jackfruit;
public class MyClass
{
    public void F() 
    {
        ConsoleApplication.CreateWithRootCommandX(A);
    }

}";
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Method_with_zero_parameters_has_no_output()
        {
            const string input = @"
using Jackfruit;
public class MyClass
{
    public void F() 
    {
        ConsoleApplication.CreateWithRootCommand();
    }

}";
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Method_with_more_than_one_parameter_has_no_output()
        {
            const string input = @"
using Jackfruit;
public class MyClass
{
    public void F() 
    {
        ConsoleApplication.CreateWithRootCommand(A, B, C);
    }
    public void A(int i) 
    {
    }
}";
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Multiple_CommandDefs_are_output()
        {
            const string input = @"
using Jackfruit;
public class MyClass
{
    public void F() 
    {
        ConsoleApplication.B.AddSubCommand(A);
        ConsoleApplication.C.AddSubCommand(A);
        ConsoleApplication.D.AddSubCommand(A);
    }
    public void A(int i) 
    {
    }
}";
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Command_found_in_top_level_are_output()
        {
            const string input = @"
ConsoleApplication.CreateWithRootCommand(A);
static void A(int i) 
{
}

";
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }


        [Fact]
        public Task Nested_subcommands_are_output()
        {
            const string input = @"
using Jackfruit;
namespace Fred
{
    public class MyClass
    {
        public void F() 
        {
            ConsoleApplication.CreateWithRootCommand(A);
            ConsoleApplication.A.B.AddSubCommand(A);
            ConsoleApplication.A.C.AddSubCommand(A);
            ConsoleApplication.A.B.D.AddSubCommand(A);
        }
        public void A(int i) 
        {
        }
    }
}";
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

    }
}
