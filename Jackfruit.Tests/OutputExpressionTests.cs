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

        [Fact]
        public Task Instatiation_outputs_correctly()
        {
            MethodModel model = new("A", "string")
            {
                Statements = new()
                {
                    Assign("B",New("C"))                }
            };
            var language = new LanguageCSharp(new StringBuilderWriter(3));
            var output = language.AddMethod(model).Output();
            return Verifier.Verify(output).UseDirectory("OutputExpressionSnaps");
        }

        [Fact]
        public Task Comparison_outputs_correctly()
        {
            MethodModel model = new("A", "string")
            {
                Statements = new()
                {
                    Assign("B",Compare("C",Operator.Equals, "D" ))                }
            };
            var language = new LanguageCSharp(new StringBuilderWriter(3));
            var output = language.AddMethod(model).Output();
            return Verifier.Verify(output).UseDirectory("OutputExpressionSnaps");
        }


        [Fact]
        public Task String_literal_outputs_correctly()
        {
            MethodModel model = new("A", "string")
            {
                Statements = new()
                {
                    Assign("B","C")           
                }
            };
            var language = new LanguageCSharp(new StringBuilderWriter(3));
            var output = language.AddMethod(model).Output();
            return Verifier.Verify(output).UseDirectory("OutputExpressionSnaps");
        }

        [Fact]
        public Task Numeric_literals_outputs_correctly()
        {
            MethodModel model = new("A", "string")
            {
                Statements = new()
                {
                    Assign("B",42),
                    Assign("B",3.14),
                }
            };
            var language = new LanguageCSharp(new StringBuilderWriter(3));
            var output = language.AddMethod(model).Output();
            return Verifier.Verify(output).UseDirectory("OutputExpressionSnaps");
        }

        [Fact]
        public Task Symbol_outputs_correctly()
        {
            MethodModel model = new("A", "string")
            {
                Statements = new()
                {
                    Assign("B",Symbol("C"))
                }
            };
            var language = new LanguageCSharp(new StringBuilderWriter(3));
            var output = language.AddMethod(model).Output();
            return Verifier.Verify(output).UseDirectory("OutputExpressionSnaps");
        }

        [Fact]
        public Task Null_literal_outputs_correctly()
        {
            MethodModel model = new("A", "string")
            {
                Statements = new()
                {
                    Assign("B",Null)
                }
            };
            var language = new LanguageCSharp(new StringBuilderWriter(3));
            var output = language.AddMethod(model).Output();
            return Verifier.Verify(output).UseDirectory("OutputExpressionSnaps");
        }

        [Fact]
        public Task This_literal_outputs_correctly()
        {
            MethodModel model = new("A", "string")
            {
                Statements = new()
                {
                    Assign("B",This)
                }
            };
            var language = new LanguageCSharp(new StringBuilderWriter(3));
            var output = language.AddMethod(model).Output();
            return Verifier.Verify(output).UseDirectory("OutputExpressionSnaps");
        }

        [Fact]
        public Task True_literal_outputs_correctly()
        {
            MethodModel model = new("A", "string")
            {
                Statements = new()
                {
                    Assign("B",true)
                }
            };
            var language = new LanguageCSharp(new StringBuilderWriter(3));
            var output = language.AddMethod(model).Output();
            return Verifier.Verify(output).UseDirectory("OutputExpressionSnaps");
        }

        [Fact]
        public Task false_literal_outputs_correctly()
        {
            MethodModel model = new("A", "string")
            {
                Statements = new()
                {
                    Assign("B",false)
                }
            };
            var language = new LanguageCSharp(new StringBuilderWriter(3));
            var output = language.AddMethod(model).Output();
            return Verifier.Verify(output).UseDirectory("OutputExpressionSnaps");
        }
    }
}
