using Jackfruit.IncrementalGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.Diagnostics;

namespace Jackfruit.Tests
{
    public class IntegrationTests
    {

        private static CSharpCompilation TestCreatingCompilation(params SyntaxTree[] syntaxTrees)
        {
            var (compilation, inputDiagnostics) = TestHelpers.GetCompilation<Generator>(syntaxTrees);
            Assert.NotNull(compilation);
            // TODO: Figure out how to get the text from the span and compare with "Cli"
            var trouble = inputDiagnostics.Where(x => x.Id != "CS0103");
            Assert.Empty(trouble);
            return compilation;
        }


        private static Compilation TestGeneration<T>(CSharpCompilation compilation, T generator)
            where T : IIncrementalGenerator, new()
        {
            var (outputCompilation, outputDiagnostics) = TestHelpers.RunGenerator(compilation, generator);
            Assert.NotNull(outputCompilation);
            Assert.Empty(outputDiagnostics);
            Assert.Equal(6, outputCompilation.SyntaxTrees.Count());
            return outputCompilation;
        }

        private static EmitResult TestGenerationEmit(Compilation outputCompilation, string outputPath)
        {
            var emitResult = outputCompilation.Emit(outputPath);
            Assert.Empty(emitResult.Diagnostics.Where(x => x.Severity is DiagnosticSeverity.Error or DiagnosticSeverity.Warning));
            Assert.True(emitResult.Success);
            return emitResult;
        }

        [Fact]
        public void Example_compiles_without_error()
        {
            var compilation = TestCreatingCompilation(Code.ProgramSyntaxTree, Code.HandlerSyntaxTree, Code.ValidatorSyntaxTree);
            TestGeneration(compilation, new Generator());
        }



        //[Fact]
        //public void Example_emits_without_error()
        //{
        //    var compilation = TestCreatingCompilation(Code.ProgramSyntaxTree, Code.HandlerSyntaxTree, Code.ValidatorSyntaxTree);
        //    var outputCompilation = TestGeneration(compilation, new Generator());
        //    TestGenerationEmit(outputCompilation, @".\Test.dll");
        //}

        //[Fact]
        //public void Example_runs_without_error()
        //{
        //    var compilation = TestCreatingCompilation(Code.ProgramSyntaxTree, Code.HandlerSyntaxTree, Code.ValidatorSyntaxTree);
        //    var outputCompilation = TestGeneration(compilation, new Generator());
        //    var file = $@".\{Path.GetTempFileName()}.dll";
        //    TestGenerationEmit(outputCompilation,file);

        //    // Output test.runtimeconfig.json

        //    ProcessStartInfo startInfo = new ProcessStartInfo();
        //    ////startInfo.CreateNoWindow = false;
        //    startInfo.UseShellExecute = false;
        //    startInfo.RedirectStandardOutput = true;
        //    startInfo.RedirectStandardError = true;
        //    startInfo.FileName = file;
        //    startInfo.Arguments = "StarTrek --Uhura";

        //    using Process? exeProcess = Process.Start(startInfo);
        //    if (exeProcess is not null)
        //    { exeProcess.WaitForExit(); }

        //    var output = exeProcess.StandardOutput.ReadToEnd();
        //    var error = exeProcess.StandardError.ReadToEnd();

        //    Assert.Equal(0, exeProcess.ExitCode);
        //    Assert.Equal("Hello, Nyota Uhura", output);
        //    Assert.Equal("", error);
        //}
    }
}

//class Program
//{
//    static void Main()
//    {
//        LaunchCommandLineApp();
//    }

//    /// <summary>
//    /// Launch the application with some options set.
//    /// </summary>
//    static void LaunchCommandLineApp()
//    {
//        // For the example
//        const string ex1 = "C:\\";
//        const string ex2 = "C:\\Dir";

//        // Use ProcessStartInfo class
//        ProcessStartInfo startInfo = new ProcessStartInfo();
//        startInfo.CreateNoWindow = false;
//        startInfo.UseShellExecute = false;
//        startInfo.FileName = "dcm2jpg.exe";
//        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
//        startInfo.Arguments = "-f j -o \"" + ex1 + "\" -z 1.0 -s y " + ex2;

//        try
//        {
//            // Start the process with the info we specified.
//            // Call WaitForExit and then the using statement will close.
//            using (Process exeProcess = Process.Start(startInfo))
//            {
//                exeProcess.WaitForExit();
//            }
//        }
//        catch
//        {
//            // Log error.
//        }
//    }
//}

//namespace CompilationTest
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            HashSet<Assembly> referencedAssemblies = new HashSet<Assembly>()
//            {
//                typeof(object).Assembly,
//                Assembly.Load(new AssemblyName("Microsoft.CSharp")),
//                Assembly.Load(new AssemblyName("netstandard")),
//                Assembly.Load(new AssemblyName("System.Runtime")),
//                Assembly.Load(new AssemblyName("System.Linq")),
//                Assembly.Load(new AssemblyName("System.Linq.Expressions"))
//            };

//            string greetingClass = @"
//namespace TestLibraryAssembly
//{
//    public static class Greeting
//    {
//        public static string GetGreeting(string name)
//        {
//            return ""Hello, "" + name + ""!"";
//        }
//    }
//}
//";

//            CSharpCompilation compilation1 = CSharpCompilation.Create(
//                "TestLibraryAssembly",
//                new[]
//                {
//                    CSharpSyntaxTree.ParseText(greetingClass)
//                },
//                referencedAssemblies.Select(assembly => MetadataReference.CreateFromFile(assembly.Location)).ToList(),
//                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
//            );
//            MemoryStream memoryStream1 = new MemoryStream();
//            EmitResult emitResult1 = compilation1.Emit(memoryStream1);
//            memoryStream1.Position = 0;
//            MetadataReference testLibraryReference = MetadataReference.CreateFromStream(memoryStream1);

//            string programCode = @"
//using TestLibraryAssembly;

//namespace TestProgram
//{
//    public class Program
//    {
//        public void Main()
//        {
//            string greeting = Greeting.GetGreeting(""Name"");
//        }
//    }
//}
//";

//            CSharpCompilation compilation2 = CSharpCompilation.Create(
//                "TestProgram",
//                new[]
//                {
//                    CSharpSyntaxTree.ParseText(programCode)
//                },
//                referencedAssemblies
//                    .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
//                    .Concat(new List<MetadataReference> { testLibraryReference }).ToList(),
//                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
//            );
//            MemoryStream memoryStream2 = new MemoryStream();
//            EmitResult emitResult2 = compilation2.Emit(memoryStream2);
//            memoryStream2.Position = 0;

//            Assembly programAssembly = Assembly.Load(memoryStream2.ToArray());
//            Type programType = programAssembly.GetType("TestProgram.Program");
//            MethodInfo method = programType.GetMethod("Main");
//            object instance = Activator.CreateInstance(programType);
//            method.Invoke(instance, null);
//        }
//    }
//}