using Xunit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VerifyXunit;
using System.Threading.Tasks;
using Jackfruit.IncrementalGenerator;
using Jackfruit.IncrementalGenerator.CodeModels;
using Jackfruit.IncrementalGenerator.Output;
using static Jackfruit.IncrementalGenerator.CodeModels.ExpressionHelpers;

namespace Jackfruit.Tests
{
    [UsesVerify]
    public class OutputStatementsTests
    {


        //[Fact]
        //public Task InvocationModel_outputs_correctly()
        //{
        //    var model = Invoke("A", "B");

        //    var language = new LanguageCSharp(new StringBuilderWriter(3));
        //    string output = language.AddCodeFile(model).Output();
        //    return Verifier.Verify(output).UseDirectory("OutputExpressionSnaps");
        //}

    }
}
