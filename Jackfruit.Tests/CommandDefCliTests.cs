using Xunit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VerifyXunit;
using System.Threading.Tasks;
using Jackfruit.IncrementalGenerator;
using Jackfruit;

namespace Jackfruit.Tests
{
    [UsesVerify]
    public class CommandDefCliTests
    {
        public  string methodWrapper (string method)
            => @$"
using Jackfruit;
public class MyClass
{{
    {method}

    public void A(int i) {{ }}
    public void B(int i) {{ }}
    public void C(int i) {{ }}
    public void D(int i) {{ }}
    public void E(int i) {{ }}
    public void F(int i) {{ }}
    public void G(int i) {{ }}
    public void H(int i) {{ }}
}}
";
        [Fact]
        public Task Single_root_command_with_single_path_produces_output()
        {
            var input = methodWrapper(@"
    public void Test()
    {
       Cli.Create(new(A))
    }");
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Two_nested_commands_produce_output()
        {
            var input = methodWrapper(@"
    public void Test()
    {
        Cli.Create(new(A, new() { 
              new(B)}));
    }");
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Three_nested_commands_produce_output()
        {
            var input = methodWrapper(@"
    public void Test() 
    {
        Cli.Create(new(A, new() { 
                new(B),
                new(C),
                new(D)
            })) ;
    }");
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Multiple_nested_commands_produce_output()
        {
            var input = methodWrapper(@"
    public void Test() 
    {
        Cli.Create(new(A, new() { 
                new(B),
                new(C, new() {
                    new(E),
                    new(F, new() {
                        new(G), 
                        new(H)
                    })
                }),
                new(D)
            })) ;
    }");
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Single_command_with_null_delegate_has_no_output()
        {
            var input = methodWrapper(@"
        public void Test()
        {
            Cli.Create(new (null));
        }");
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Method_with_other_names_has_no_output()
        {
            var input = methodWrapper(@"
        public void Test()
        {
            Cli.CreateX(new (A));
        }");
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Method_with_zero_parameters_has_no_output()
        {
            var input = methodWrapper(@"
        public void Test()
        {
            Cli.Create();
        }");
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Method_with_more_than_one_parameter_has_no_output()
        {
            var input = methodWrapper(@"
        public void Test()
        {
            Cli.Create(new (A),new (B),new (C));
        }");
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Descriptions_found_in_XmlComment()
        {
        var input = methodWrapper(@"
        public void Test()
        {
            Cli.Create(new (AA));
        }

        /// <summary>
        /// Command description in XmlComment
        /// </summary>
        /// <param name=""i"">Option description in XmlComment</param>
        public void AA(int i) 
        { }");
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Descriptions_found_in_Attribute()
        {
            var input = methodWrapper(@"
        public void Test()
        {
            Cli.Create(new (AA));
        }

        [Description(""Command description in Attribute"")]
        public void AA(
                [Description(""Member description in Attribute"")] int i) 
        { }");
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Description_found_in_argument_Attribute()
        {
            var input = methodWrapper(@"
        public void Test()
        {
            Cli.Create(new (AA));
        }

        public void AA(
                [Description(""Member description in Attribute"")][Argument] int i) 
        { }");
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task IsArgument_found_in_Attribute()
        {
            var input = methodWrapper(@"
        public void Test()
        {
            Cli.Create(new (AA));
        }

        public void AA(
                [Argument] int i) 
        {
        }");
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Required_found_in_Attribute()
        {
            var input = methodWrapper(@"
        public void Test()
        {
            Cli.Create(new (AA));
        }

        public void AA(
                [Required] int i) 
        { }");
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Required_found_in_argument_attribute()
        {
            var input = methodWrapper(@"
        public void Test()
        {
            Cli.Create(new (AA));
        }

        public void AA(
                [Argument][Required] int i) 
        { }");
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }


        [Fact]
        public Task Aliases_found_in_attribute()
        {
            var input = methodWrapper(@"
        public void Test()
        {
            Cli.Create(new (AA));
        }

        [Aliases(""C1"")]
        public void AA(
                [Aliases(""1"",""2"")] int i) 
        { }");
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task OptionArgumentName_found_in_attribute()
        {
            var input = methodWrapper(@"
        public void Test()
        {
            Cli.Create(new (AA));
        }

        public void AA(
                [OptionArgumentName(""ArgName"")] int i) 
        { }");
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }
    }
}
