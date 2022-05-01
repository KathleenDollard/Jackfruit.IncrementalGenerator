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
        ConsoleApplication.AddRootCommand(A);
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
        MyClass.YesMine.Really.ConsoleApplication.AddRootCommand(A);
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
        MyClass.YesMine.Really.ConsoleApplication.AddRootCommand(null);
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
        ConsoleApplication.AddRootCommandX(A);
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
        ConsoleApplication.AddRootCommand();
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
        ConsoleApplication.AddRootCommand(A, B, C);
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
ConsoleApplication.AddRootCommand(A);
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
            ConsoleApplication.AddRootCommand(A);
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

        [Fact]
        public Task Descriptions_found_in_XmlComment()
        {
            const string input = @"
using Jackfruit;
public class MyClass
{
    public void F() 
    {
        ConsoleApplication.AddRootCommand(A);
    }

    /// <summary>
    /// Command description in XmlComment
    /// </summary>
    /// <param name=""i"">Option description in XmlComment</param>
    public void A(int i) 
    {
    }

}";
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Descriptions_found_in_Attribute()
        {
            const string input = @"
using Jackfruit;
public class MyClass
{
    public void F() 
    {
        var app = ConsoleApplication.Create();
        app.SetRootCommand(A);
    }

    [Description(""Command description in Attribute"")]
    public void A(
            [Description(""Member description in Attribute"")] int i) 
    {
    }

}";
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Description_found_in_argument_Attribute()
        {
            const string input = @"
using Jackfruit;
public class MyClass
{
    public void F() 
    {
        var app = ConsoleApplication.Create();
        app.SetRootCommand(A);
    }

    public void A(
            [Description(""Member description in Attribute"")][Argument] int i) 
    {
    }

}";
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task IsArgument_found_in_Attribute()
        {
            const string input = @"
using Jackfruit;
public class MyClass
{
    public void F() 
    {
        var app = ConsoleApplication.Create();
        app.SetRootCommand(A);
    }

    public void A(
            [Argument] int i) 
    {
    }

}";
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Required_found_in_Attribute()
        {
            const string input = @"
using Jackfruit;
public class MyClass
{
    public void F() 
    {
        var app = ConsoleApplication.Create();
        app.SetRootCommand(A);
    }

    public void A(
            [Required] int i) 
    {
    }

}";
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }


        [Fact]
        public Task Required_found_in_argument_attribute()
        {
            const string input = @"
using Jackfruit;
public class MyClass
{
    public void F() 
    {
        var app = ConsoleApplication.Create();
        app.SetRootCommand(A);
    }

    public void A(
            [Argument][Required] int i) 
    {
    }

}";
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }


        [Fact]
        public Task Aliases_found_in_attribute()
        {
            const string input = @"
using Jackfruit;
public class MyClass
{
    public void F() 
    {
        var app = ConsoleApplication.Create();
        app.SetRootCommand(A);
    }

    [Aliases(""C1"")]
    public void A(
            [Aliases(""1"",""2"")] int i) 
    {
    }

}";
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }



        [Fact]
        public Task OptionArgumentName_found_in_attribute()
        {
            const string input = @"
using Jackfruit;
public class MyClass
{
    public void F() 
    {
        var app = ConsoleApplication.Create();
        app.SetRootCommand(A);
    }

    public void A(
            [OptionArgumentName(""ArgName"")] int i) 
    {
    }

}";
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }
    }

}
