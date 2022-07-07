using Xunit;
using VerifyXunit;
using System.Threading.Tasks;
using Jackfruit;
using System;
using System.Linq;
using Jackfruit.TestSupport;
using Microsoft.CodeAnalysis;

namespace Jackfruit.Tests
{
    [UsesVerify]
    public class CommandDefCliTests
    {
        public  string methodWrapper (string method)
            => @$"
using Jackfruit;
using System.ComponentModel;
using System;

public partial class RootCommand
{{
        public static RootCommand Create(params SubCommand[] subCommands)
            => null;

        public static RootCommand Create(Delegate runHandler, params SubCommand[] subCommands)
            => null;

        public partial class Result
        {{ }}
}}

public class MyClass
{{
    {method}

    public static void A(int i) {{ }}
    public static void B(int i) {{ }}
    public static void C(int i) {{ }}
    public static void D(int i) {{ }}
    public static void E(int i) {{ }}
    public static void F(int i) {{ }}
    public static void G(int i) {{ }}
    public static void H(int i) {{ }}
}}
";
        [Fact]
        public Task Single_root_command_with_single_path_produces_output()
        {
            var input = methodWrapper(@"
    public void Test()
    {
       var rootCommand = RootCommand.Create(A);
    }");
            var (inputDiagnostics,diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(TestHelpers.WarningAndErrors(inputDiagnostics));
            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Two_nested_commands_produce_output()
        {
            var input = methodWrapper(@"
    public void Test()
    {
       var rootCommand = RootCommand.Create(A, 
            SubCommand.Create(B));
    }");
            var (inputDiagnostics, diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(TestHelpers.WarningAndErrors(inputDiagnostics));
            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Three_nested_commands_produce_output()
        {
            var input = methodWrapper(@"
    public void Test() 
    {
       var rootCommand = RootCommand.Create(A, 
            SubCommand.Create(B),
            SubCommand.Create(C),
            SubCommand.Create(D));
    }");
            var (inputDiagnostics, diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(TestHelpers.WarningAndErrors(inputDiagnostics));
            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Multiple_nested_commands_produce_output()
        {
            var input = methodWrapper(@"
    public void Test() 
    {
       var rootCommand = RootCommand.Create(A, 
            SubCommand.Create(B),
            SubCommand.Create(C,
                SubCommand.Create(E),
                SubCommand.Create(F,
                    SubCommand.Create(G),
                    SubCommand.Create(H))),
            SubCommand.Create(D));
    }");
            var (inputDiagnostics, diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(TestHelpers.WarningAndErrors(inputDiagnostics));
            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact(Skip = "Waiting for target type new /param array fix")]
        public Task List_ctor_and_implicit_creation_allowed()
        {
            var input = methodWrapper(@"
    public void Test() 
    {
        var rootCommand = RootCommand.Create(A, 
                SubCommand.Create(B),
                SubCommand.Create(C,  
                    SubCommand.Create(E),
                    SubCommand.Create(F,
                        SubCommand.Create(G), 
                        SubCommand.Create(H)
                    )
                ),
                SubCommand.Create(D)
           );
    }");
            var (inputDiagnostics, diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(TestHelpers.WarningAndErrors(inputDiagnostics));
            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }
        [Fact]
        public Task Single_command_with_null_delegate_has_no_output()
        {
            var input = methodWrapper(@"
        public void Test()
        {
            var rootCommand = RootCommand.Create(null);
        }");
            var (inputDiagnostics, diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(TestHelpers.WarningAndErrors(inputDiagnostics));
            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Method_with_other_names_has_no_output()
        {
            var input = methodWrapper(@"
        public void Test()
        {
            var rootCommand = RootCommand.CreateX(SubCommand.Create(A);
        }");
            var (inputDiagnostics, diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Single(TestHelpers.WarningAndErrors(inputDiagnostics)); // explicitly creating an error here
            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Method_with_zero_parameters_has_no_output()
        {
            var input = methodWrapper(@"
        public void Test()
        {
            var rootCommand = RootCommand.Create();
        }");
            var (inputDiagnostics, diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(TestHelpers.WarningAndErrors(inputDiagnostics));
            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Descriptions_found_in_XmlComment()
        {
        var input = methodWrapper(@"
        public void Test()
        {
            var rootCommand = RootCommand.Create(AA);
        }

        /// <summary>
        /// Command description in XmlComment
        /// </summary>
        /// <param name=""i"">Option description in XmlComment</param>
        public void AA(int i) 
        { }");
            var (inputDiagnostics, diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(TestHelpers.WarningAndErrors(inputDiagnostics));
            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Descriptions_found_in_Attribute()
        {
            var input = methodWrapper(@"
        public void Test()
        {
            var rootCommand = RootCommand.Create(AA);
        }

        [Description(""Command description in Attribute"")]
        public void AA(
                [Description(""Member description in Attribute"")] int i) 
        { }");
            var (inputDiagnostics, diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(TestHelpers.WarningAndErrors(inputDiagnostics));
            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Description_found_in_argument_Attribute()
        {
            var input = methodWrapper(@"
        public void Test()
        {
            var rootCommand = RootCommand.Create(AA);
        }

        public void AA(
                [Description(""Member description in Attribute"")][Argument] int i) 
        { }");
            var (inputDiagnostics, diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(TestHelpers.WarningAndErrors(inputDiagnostics));
            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task IsArgument_found_in_Attribute()
        {
            var input = methodWrapper(@"
        public void Test()
        {
            var rootCommand = RootCommand.Create(AA);
        }

        public void AA(
                [Argument] int i) 
        {
        }");
            var (inputDiagnostics, diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(TestHelpers.WarningAndErrors(inputDiagnostics));
            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Required_found_in_Attribute()
        {
            var input = methodWrapper(@"
        public void Test()
        {
            var rootCommand = RootCommand.Create(AA);
        }

        public void AA(
                [Required] int i) 
        { }");
            var (inputDiagnostics, diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(TestHelpers.WarningAndErrors(inputDiagnostics));
            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Required_found_in_argument_attribute()
        {
            var input = methodWrapper(@"
        public void Test()
        {
            var rootCommand = RootCommand.Create(AA);
        }

        public void AA(
                [Argument][Required] int i) 
        { }");
            var (inputDiagnostics, diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(TestHelpers.WarningAndErrors(inputDiagnostics));
            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }


        [Fact]
        public Task Aliases_found_in_attribute()
        {
            var input = methodWrapper(@"
        public void Test()
        {
            var rootCommand = RootCommand.Create(AA);
        }

        [Aliases(""C1"")]
        public void AA(
                [Aliases(""1"",""2"")] int i) 
        { }");
            var (inputDiagnostics, diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(TestHelpers.WarningAndErrors(inputDiagnostics));
            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task OptionArgumentName_found_in_attribute()
        {
            var input = methodWrapper(@"
        public void Test()
        {
            var rootCommand = RootCommand.Create(AA);
        }

        public void AA(
                [OptionArgumentName(""ArgName"")] int i) 
        { }");
            var (inputDiagnostics, diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(TestHelpers.WarningAndErrors(inputDiagnostics));
            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }
    }
}
