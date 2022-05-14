using Xunit;
using VerifyXunit;
using System.Threading.Tasks;
using Jackfruit;
using System;

namespace Jackfruit.Tests
{
    [UsesVerify]
    public class CommandDefValidatorTests
    {
        public  string methodWrapper (string method)
            => @$"
using Jackfruit;
public class MyClass
{{
    {method}

    public static void ToValidate(int i, int j, int k){{ }}
    public static void Validator0() {{ }}
    public static void Validator1(int j) {{ }}
    public static void ValidatorAll(int i, int j, int k) {{ }}
}}
";
        [Fact]
        public Task Validator_with_one_parameter_succeeds()
        {
            var input = methodWrapper(@"
    public void Test()
    {
       Cli.Create(new( ToValidate, Validator1))
    }");
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Validator_with_all_parameters_succeeds()
        {
            var input = methodWrapper(@"
            public void Test()
            {
               Cli.Create(new( ToValidate, ValidatorAll))
            }");
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Validator_with_no_parameters_is_ignored()
        {
            var input = methodWrapper(@"
            public void Test()
            {
               Cli.Create(new( ToValidate, Validator0))
            }");
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

    }
}
