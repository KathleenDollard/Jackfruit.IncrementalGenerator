using Microsoft.CodeAnalysis;

namespace Jackfruit.TestSupport
{
    public class IntegrationTestConfiguration
    {

        internal static string currentPath = Environment.CurrentDirectory;

        private string? testInputPath;
        private string? testGeneratedCodePath;
        private string? testBuildPath;
        private string? handlerFilePath;
        private string? validatorFilePath;
        private string? programFilePath;

        public IntegrationTestConfiguration(string testSetName)
        {
            TestSetName = testSetName;
        }

        public string TestSetName { get; }
        public string TestInputPath
        {
            get => testInputPath ?? Path.Combine(currentPath, @$"../../../../{TestSetName}");
            set => testInputPath = value;
        }
        public string TestGeneratedCodePath
        {
            get => testGeneratedCodePath ?? Path.Combine(TestInputPath, "GeneratedViaTest");
            set => testGeneratedCodePath = value;
        }
        public string TestBuildPath
        {
            get => testBuildPath ?? Path.Combine(TestInputPath, "bin", "Debug", DotnetVersion);
            set => testBuildPath = value;
        }
        public string HandlerFilePath
        {
            get => handlerFilePath ?? Path.Combine(TestInputPath, "Handlers.cs");
            set => handlerFilePath = value;
        }
        public string ValidatorFilePath
        {
            get => validatorFilePath ?? Path.Combine(TestInputPath, "Validators.cs");
            set => validatorFilePath = value;
        }
        public string ProgramFilePath
        {
            get => programFilePath ?? Path.Combine(TestInputPath, "Program.cs");
            set => programFilePath = value;
        }
        public string DotnetVersion { get; set; } = "net6.0";

        public OutputKind? OutputKind { get; set; } = 
            Microsoft.CodeAnalysis.OutputKind.DynamicallyLinkedLibrary;

        public int? SyntaxTreeCount { get; set; } = null;

        public string[] SourceFiles { get; set; } = Array.Empty<string>();
    }
}
