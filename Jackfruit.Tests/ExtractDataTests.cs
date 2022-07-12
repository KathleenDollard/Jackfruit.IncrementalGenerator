using Jackfruit.IncrementalGenerator;
using Jackfruit.TestSupport;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
    public class ExtractDataTests
    {

        private CancellationToken cancellationToken = new CancellationTokenSource().Token; // just for testing

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
        public void Single_root_command_with_a_handler_succeeds()
        {
            var input = methodWrapper(@"
        SetAction(A);
    ");
            var syntaxTree = CSharpSyntaxTree.ParseText(input);
            var (compilation,inputDiagnostics)= TestHelpers.GetCompilation<CommandDefGenerator>(syntaxTree);
            var defineNode = syntaxTree.GetRoot()
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .Where(x=>x.Identifier.Text == "Define")
                .Single();

            Assert.Empty(TestHelpers.WarningAndErrors(inputDiagnostics));
            Assert.NotNull(compilation);
            Assert.NotNull(defineNode);

            var commandDetails = ExtractData.GetDetails(defineNode, compilation.GetSemanticModel(syntaxTree), cancellationToken);
            //return Verifier.Verify(output).UseDirectory("Snapshots");
        }
    }
}
