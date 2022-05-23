using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.IO;

namespace Jackfruit.Tests;

public class TestHelpers
{
    public static (ImmutableArray<Diagnostic> Diagnostics, string Output) GetGeneratedOutput<T>(string source)
        where T : IIncrementalGenerator, new()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        return GetGeneratedOutput<T>(new[] { syntaxTree });
    }

    public static (ImmutableArray<Diagnostic> Diagnostics, string Output)
        GetGeneratedOutput<T>(IEnumerable<SyntaxTree> syntaxTrees)
        where T : IIncrementalGenerator, new()
        => GetGeneratedOutput<T>(syntaxTrees.ToArray());

    public static (ImmutableArray<Diagnostic> Diagnostics, string Output)
        GetGeneratedOutput<T>(params SyntaxTree[] syntaxTrees)
        where T : IIncrementalGenerator, new()
    {
        var (compilation, inputDiagnostics) = GetCompilation<T>(syntaxTrees);

        var originalTreeCount = compilation.SyntaxTrees.Length;
        var generator = new T();

        var (outputCompilation, diagnostics) = RunGenerator(compilation, generator);

        var trees = outputCompilation.SyntaxTrees.ToList();
        var newTrees = string.Join(
                $"{Environment.NewLine}// *******************************{Environment.NewLine}{Environment.NewLine}",
                trees.Skip(originalTreeCount));

        return (diagnostics, newTrees);
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
        GetCompilation<T>(IEnumerable<SyntaxTree> syntaxTrees)
        where T : IIncrementalGenerator, new()
        => GetCompilation<T>(syntaxTrees.ToArray());

    public static (CSharpCompilation compilation, ImmutableArray<Diagnostic> inputDiagnostics)
        GetCompilation<T>(params SyntaxTree[] syntaxTrees)
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
                Path.GetFileName( x.FilePath).Contains("Jackfruit", StringComparison.OrdinalIgnoreCase));
        var compilation = CSharpCompilation.Create(
            "generator",
            syntaxTrees,
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var inputDiagnostics = compilation.GetDiagnostics();
        return (compilation, inputDiagnostics);
    }
}