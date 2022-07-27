using Jackfruit.IncrementalGenerator;
using Jackfruit.TestSupport;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace Jackfruit.Tests
{
    [UsesVerify]
    public class ExtractDataTests
    {


        private CommandDetail GetCommandDetail(string testCode)
        {
            var cancellationToken = new CancellationTokenSource().Token; // just for testing

            var input = methodWrapper(testCode);
            var syntaxTree = CSharpSyntaxTree.ParseText(input);
            var (compilation, inputDiagnostics) = TestHelpers.GetCompilation<CommandDefGenerator>(syntaxTree);
            var defineNode = compilation.SyntaxTrees.First().GetRoot()
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Where(x => x.Identifier.Text == "MyClass")
                .Single();

            Assert.Empty(TestHelpers.WarningAndErrors(inputDiagnostics));
            Assert.NotNull(compilation);
            Assert.NotNull(defineNode);

            return ExtractData.GetDetails(defineNode, compilation.GetSemanticModel(syntaxTree), cancellationToken);
        }

        public string methodWrapper(string testCode)
    => @$"
using Jackfruit;
using Jackfruit.Runtime;
using System.ComponentModel;
using System;

public class MyClass : RootCommand
{{
    public override void Define()
    {{
        {testCode}
    }}

    public static void A(int i) {{ }}
    public static void B(int i, string s) {{ }}
    public static void C(int i) {{ }}
    public static void D(int i) {{ }}
    public static void E(int i) {{ }}
    public static void F(int i) {{ }}
    public static void G(int i) {{ }}
    public static void H(int i) {{ }}
}}
";
        [Fact]
        public Task Root_command_w_handler_w_one_param()
        {
            var commandDetails = GetCommandDetail(@"
        SetAction(A);
    ");
            return Verifier.Verify(commandDetails).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Root_command_w_handler_w_two_params()
        {
            var commandDetails = GetCommandDetail(@"
        SetAction(B);
    ");
            return Verifier.Verify(commandDetails).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Root_command_w_one_subCommand()
        {
            var commandDetails = GetCommandDetail(@"
        AddSubCommand(new SubCommand(A));
    ");
            return Verifier.Verify(commandDetails).UseDirectory("Snapshots");
        }

        [Fact]
        public Task Root_command_w_three_subCommands()
        {
            var commandDetails = GetCommandDetail(@"
        AddSubCommand(A);
        AddSubCommand(B);
        AddSubCommand(C);
    ");
            return Verifier.Verify(commandDetails).UseDirectory("Snapshots");
        }
    }
}
