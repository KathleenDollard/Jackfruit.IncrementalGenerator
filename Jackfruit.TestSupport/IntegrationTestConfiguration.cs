namespace Jackfruit.TestSupport
{
    public class IntegrationTestConfiguration
    {

        internal static string currentPath = Environment.CurrentDirectory;

        public string? testInputPath;
        public string? testGeneratedCodePath;
        public string? testBuildPath;
        public string? handlerFilePath;
        public string? validatorFilePath;
        public string? programFilePath;
        public string dotnetVersion = "net6.0"; // this is the first place incremental generators appeared
        private string? testSetName;

        public IntegrationTestConfiguration()
        { }

        public string TestSetName
        {
            get => testSetName ?? "TestOutputExample"; 
            protected internal set => testSetName = value;
        }
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
            get => testBuildPath ?? Path.Combine(TestInputPath, "bin", "Debug", dotnetVersion);
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
        public string DotnetVersion
        {
            get => dotnetVersion;
            set => dotnetVersion = value;
        }
    }
}
