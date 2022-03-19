using Xunit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VerifyXunit;
using System.Threading.Tasks;
using Jackfruit.IncrementalGenerator;
using Jackfruit.IncrementalGenerator.CodeModels;
using Jackfruit.IncrementalGenerator.Output;
using static Jackfruit.IncrementalGenerator.CodeModels.ExpressionHelpers;
using static Jackfruit.IncrementalGenerator.CodeModels.StatementHelpers;

namespace Jackfruit.Tests
{
    [UsesVerify]
    public class OutputExpressionTests
    {


        [Fact]
        public Task Invocation_outputs_correctly()
        {
            MethodModel model = new("A", "string")
            {
                Statements = new()
                {
                    SimpleCall(Invoke("B", "C"))
                }
            };
            var language = new LanguageCSharp(new StringBuilderWriter(3));
            var output = language.AddMethod(model).Output();
            return Verifier.Verify(output).UseDirectory("OutputExpressionSnaps");
        }

    }
}
