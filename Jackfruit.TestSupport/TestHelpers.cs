using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Diagnostics;
using Xunit;

namespace Jackfruit.TestSupport;

public class TestHelpers
{
    public static IEnumerable<Diagnostic> WarningAndErrors(IEnumerable<Diagnostic> diagnostics)
        => diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error || d.Severity == DiagnosticSeverity.Warning);

    public static (ImmutableArray<Diagnostic> InputDiagnostics, ImmutableArray<Diagnostic> Diagnostics, string Output)
        GetGeneratedOutput<T>(string source = "")
        where T : IIncrementalGenerator, new()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        return GetGeneratedOutput<T>(new[] { syntaxTree });
    }

    public static (ImmutableArray<Diagnostic> InputDiagnostics, ImmutableArray<Diagnostic> Diagnostics, string Output)
        GetGeneratedOutput<T>(IEnumerable<SyntaxTree> syntaxTrees)
        where T : IIncrementalGenerator, new()
        => GetGeneratedOutput<T>(syntaxTrees.ToArray());

    public static (ImmutableArray<Diagnostic> InputDiagnostics, ImmutableArray<Diagnostic> Diagnostics, string Output)
    GetGeneratedOutput<T>(params SyntaxTree[] syntaxTrees)
         where T : IIncrementalGenerator, new()
         => GetGeneratedOutput<T>(OutputKind.DynamicallyLinkedLibrary, syntaxTrees);

    public static (ImmutableArray<Diagnostic> InputDiagnostics, ImmutableArray<Diagnostic> Diagnostics, string Output)
        GetGeneratedOutput<T>(OutputKind compilationKind, params SyntaxTree[] syntaxTrees)
        where T : IIncrementalGenerator, new()
    {
        var (compilation, inputDiagnostics) = GetCompilation<T>(compilationKind, syntaxTrees);

        var originalTreeCount = compilation.SyntaxTrees.Length;
        var generator = new T();

        var (outputCompilation, diagnostics) = RunGenerator(compilation, generator);

        var trees = outputCompilation.SyntaxTrees.ToList();
        var newTrees = string.Join(
                $"{Environment.NewLine}// *******************************{Environment.NewLine}{Environment.NewLine}",
                trees.Skip(originalTreeCount));

        return (inputDiagnostics, diagnostics, newTrees);
    }

    public static (Compilation outputCompilation, ImmutableArray<Diagnostic> outputDiagnostics)
        RunGenerator<T>(CSharpCompilation compilation, T generator)
        where T : IIncrementalGenerator, new()
    {
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);
        return (outputCompilation, diagnostics);
    }

    public static (CSharpCompilation compilation, ImmutableArray<Diagnostic> inputDiagnostics)
        GetCompilation<T>(params SyntaxTree[] syntaxTrees)
        where T : IIncrementalGenerator, new()
        => GetCompilation<T>(OutputKind.DynamicallyLinkedLibrary, syntaxTrees);

    public static (CSharpCompilation compilation, ImmutableArray<Diagnostic> inputDiagnostics)
        GetCompilation<T>(OutputKind? compilationKind, params SyntaxTree[] syntaxTrees)
        where T : IIncrementalGenerator, new()
    {
        System.Reflection.Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var references = assemblies
            .Where(_ => !_.IsDynamic && !string.IsNullOrWhiteSpace(_.Location))
            .Select(_ => MetadataReference.CreateFromFile(_.Location))
            .Concat(new[]
            {
                MetadataReference.CreateFromFile(typeof(T).Assembly.Location),
                //MetadataReference.CreateFromFile(typeof(EnumExtensionsAttribute).Assembly.Location)
            });
        var temp = references.Where(x => x is not null &&
                Path.GetFileName(x.FilePath ?? "").Contains("Jackfruit", StringComparison.OrdinalIgnoreCase));
        var compilation = CSharpCompilation.Create(
            "generator",
            syntaxTrees,
            references,
            new CSharpCompilationOptions(compilationKind ?? OutputKind.DynamicallyLinkedLibrary));
        var inputDiagnostics = compilation.GetDiagnostics();
        return (compilation, inputDiagnostics);
    }

    public static void OutputGeneratedTrees(Compilation generatedCompilation, string outputDir)
    {
        // the presence of a file path is used to indicate generated. this feels weak.
        foreach (var tree in generatedCompilation.SyntaxTrees)
        {
            if (string.IsNullOrWhiteSpace(tree.FilePath))
                { continue; }
            var fileName = Path.Combine(outputDir, Path.GetFileName(tree.FilePath));
            File.WriteAllText(fileName, tree.ToString());
        }
    }

    public static Process? CompileOutput(string testInputPath)
    {
        ProcessStartInfo startInfo = new()
        {
            ////startInfo.CreateNoWindow = false;
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = testInputPath,
            FileName = "dotnet",
            Arguments = "build"
        };
        Process? exeProcess = Process.Start(startInfo);
        Assert.NotNull(exeProcess);
        if (exeProcess is not null)
        {
            exeProcess.WaitForExit(30000);
        }

        return exeProcess;
    }

    public static string IfOsIsWindows(string windowsString, string unixString)
        => Environment.OSVersion.Platform == PlatformID.Unix
            ? unixString
            : windowsString;

    public static string? RunGeneratedProject(string arguments, string setName, string buildPath)
    {

        ProcessStartInfo startInfo = new()
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        var fieName = Path.Combine(buildPath, setName);

        startInfo.FileName = $"{fieName}{IfOsIsWindows(".exe", "")}";
        startInfo.Arguments = arguments;

        Process? exeProcess = Process.Start(startInfo);
        Assert.NotNull(exeProcess);
        if (exeProcess is not null)
        {
            exeProcess.WaitForExit();

            var output = exeProcess.StandardOutput.ReadToEnd();
            var error = exeProcess.StandardError.ReadToEnd();

            Assert.Equal(0, exeProcess.ExitCode);
            Assert.Equal("", error);
            return output;
        }
        return null;
    }

}