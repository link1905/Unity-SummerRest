using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Microsoft.CodeAnalysis.Text;
using RestSourceGenerator.Generators;

namespace RestSourceGenerator.Tests.Utilities;

public static class TestsUtilities
{
    public static Task SimpleTest<TSourceGen>(string source, (Type sourceGeneratorType, string filename, SourceText content) file) where TSourceGen : ISourceGenerator, new()
    {
        var test = new CSharpSourceGeneratorTest<TSourceGen, XUnitVerifier>
        {
            TestState =
            {
                Sources = { source },
                GeneratedSources = { file }
            },
            CompilerDiagnostics = CompilerDiagnostics.None
        };
        return test.RunAsync();
    }
}