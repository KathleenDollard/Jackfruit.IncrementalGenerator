using Xunit;
using VerifyXunit;
using System.Threading.Tasks;
using Jackfruit.IncrementalGenerator;
using Jackfruit.IncrementalGenerator.CodeModels;
using Jackfruit.IncrementalGenerator.Output;
using static Jackfruit.IncrementalGenerator.CodeModels.ExpressionHelpers;
using static Jackfruit.IncrementalGenerator.CodeModels.StatementHelpers;
using System.Collections.Generic;

namespace Jackfruit.Tests
{
    [UsesVerify]
    public class OutputStatementsTests
    {


        [Fact]
        public Task If_then_outputs_correctly()
        {
            MethodModel model = new("A", "string")
            {
                Statements = new()
                {
                    If(ifCondition: true,
                        ifStatements: new List<IStatement>{ Assign("B",false) },
                        elseStatements: new List<IStatement>{Assign("B", true) })
                }
            };
            var language = new LanguageCSharp(new StringBuilderWriter(3));
            var output = language.AddMethod(model).Output();
            return Verifier.Verify(output).UseDirectory("OutputStatementSnaps");
        }

        [Fact]
        public Task If_then_else_outputs_correctly()
        {
            MethodModel model = new("A", "string")
            {
                Statements = new()
                {
                    If(ifCondition: true,
                        ifStatements: new List<IStatement>{ Assign("B",false) },
                        elseStatements: new List<IStatement>{Assign("B", false) })
                }
            };
            var language = new LanguageCSharp(new StringBuilderWriter(3));
            var output = language.AddMethod(model).Output();
            return Verifier.Verify(output).UseDirectory("OutputStatementSnaps");
        }

        [Fact]
        public Task If_then_elseif_outputs_correctly()
        {
            MethodModel model = new("A", "string")
            {
                Statements = new()
                {
                    If(ifCondition: true,
                        ifStatements: new List<IStatement>{ Assign("B",false) },
                        (false, new List<IStatement>{Assign("B", false) }))
                }
            };
            var language = new LanguageCSharp(new StringBuilderWriter(3));
            var output = language.AddMethod(model).Output();
            return Verifier.Verify(output).UseDirectory("OutputStatementSnaps");
        }


        [Fact]
        public Task If_then_elseif_with_else_outputs_correctly()
        {
            MethodModel model = new("A", "string")
            {
                Statements = new()
                {
                    If(ifCondition: true,
                        ifStatements: new List<IStatement>{ Assign("B",false) },
                        elseStatements: new List<IStatement>{Assign("B", false) },
                        (false, new List<IStatement>{Assign("B", false) }))
                }
            };
            var language = new LanguageCSharp(new StringBuilderWriter(3));
            var output = language.AddMethod(model).Output();
            return Verifier.Verify(output).UseDirectory("OutputStatementSnaps");
        }

        [Fact]
        public Task If_then_3_else_outputs_correctly()
        {
            MethodModel model = new("A", "string")
            {
                Statements = new()
                {
                    If(ifCondition: Compare(Symbol("B"),Operator.Equals, 1),
                        ifStatements: new List<IStatement>{ Assign("B",false) },
                        elseStatements: new List<IStatement>{Assign("B", false) },
                        (Compare(Symbol("B"),Operator.Equals, 2), new List<IStatement>{Assign("B", false) }),
                        (Compare(Symbol("B"),Operator.Equals, 3), new List<IStatement>{Assign("B", false) }),
                        (Compare(Symbol("B"),Operator.Equals, 4), new List<IStatement>{Assign("B", false) }))
                }
            };
            var language = new LanguageCSharp(new StringBuilderWriter(3));
            var output = language.AddMethod(model).Output();
            return Verifier.Verify(output).UseDirectory("OutputStatementSnaps");
        }

        [Fact]
        public Task Foreach_outputs_correctly()
        {
            MethodModel model = new("A", "string")
            {
                Statements = new()
                {
                    ForEach("x",Symbol("xList"),
                        new List<IStatement>{ Assign("B",false) })
                }
            };
            var language = new LanguageCSharp(new StringBuilderWriter(3));
            var output = language.AddMethod(model).Output();
            return Verifier.Verify(output).UseDirectory("OutputStatementSnaps");
        }

        [Fact]
        public Task Assignment_outputs_correctly()
        {
            MethodModel model = new("A", "string")
            {
                Statements = new()
                {
                    Assign("B", true)
                }
            };
            var language = new LanguageCSharp(new StringBuilderWriter(3));
            var output = language.AddMethod(model).Output();
            return Verifier.Verify(output).UseDirectory("OutputStatementSnaps");
        }

        [Fact]
        public Task Assignment_with_declare_outputs_correctly()
        {
            MethodModel model = new("A", "string")
            {
                Statements = new()
                {
                    AssignWithDeclare("B", true),
                    AssignWithDeclare("string","B", true)

                }
            };
            var language = new LanguageCSharp(new StringBuilderWriter(3));
            var output = language.AddMethod(model).Output();
            return Verifier.Verify(output).UseDirectory("OutputStatementSnaps");
        }

        [Fact]
        public Task Return_outputs_correctly()
        {
            MethodModel model = new("A", "string")
            {
                Statements = new()
                {
                   Return(true)
                }
            };
            var language = new LanguageCSharp(new StringBuilderWriter(3));
            var output = language.AddMethod(model).Output();
            return Verifier.Verify(output).UseDirectory("OutputStatementSnaps");
        }

        [Fact]
        public Task SimpleCall_outputs_correctly()
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
            return Verifier.Verify(output).UseDirectory("OutputStatementSnaps");
        }

        [Fact]
        public Task Comment_outputs_correctly()
        {
            MethodModel model = new("A", "string")
            {
                Statements = new()
                {
                    Comment("This is a comment")
                }
            };
            var language = new LanguageCSharp(new StringBuilderWriter(3));
            var output = language.AddMethod(model).Output();
            return Verifier.Verify(output).UseDirectory("OutputStatementSnaps");
        }


    }
}
