using Xunit;
using VerifyXunit;
using System.Threading.Tasks;
using Jackfruit;
using System;
using Jackfruit.TestSupport;
using Jackfruit.IncrementalGenerator;

namespace Jackfruit.Tests
{
    [UsesVerify]
    public class CommandDefValidatorTests
    {
        public static string MethodWrapper(string method)
                    => @$"
using Jackfruit;

public partial class RootCommand
{{
    public static RootCommand Create(SubCommand cliRoot)
        => new RootCommand();
}}

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
            var input = MethodWrapper(@"
    public void Test()
    {
       var rootCommand = RootCommand.Create(SubCommand.Create( ToValidate, Validator1));
    }");
            var (inputDiagnostics, diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(TestHelpers.WarningAndErrors(inputDiagnostics));
            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Validator_with_all_parameters_succeeds()
        {
            var input = MethodWrapper(@"
            public void Test()
            {
               var rootCommand = RootCommand.Create(SubCommand.Create( ToValidate, ValidatorAll));
            }");
            var (inputDiagnostics, diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(TestHelpers.WarningAndErrors(inputDiagnostics));
            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Validator_with_no_parameters_is_ignored()
        {
            var input = MethodWrapper(@"
            public void Test()
            {
               var rootCommand = RootCommand.Create(SubCommand.Create( ToValidate, Validator0));
            }");
            var (inputDiagnostics, diagnostics, output) = TestHelpers.GetGeneratedOutput<CommandDefGenerator>(input);

            Assert.Empty(TestHelpers.WarningAndErrors(inputDiagnostics));
            Assert.Empty(diagnostics);
            return Verifier.Verify(output).UseDirectory("Snapshots");
        }

    }
}
