using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using RestSourceGenerator.Generators;
using RestSourceGenerator.Metadata;
using SharedSourceGenerator.Metadata;
using SharedSourceGenerator.Utilities;
using Xunit;

namespace RestSourceGenerator.Tests;

public class InheritOrCustomSourceGeneratorTests
{
    [Fact]
    public async Task GenerateInheritOrCustomFieldsClasses()
    {
        var code = """
                   namespace SummerRest.Attributes {
                       using System;
                       [AttributeUsage(AttributeTargets.Field)]
                       public class InheritOrCustomAttribute : Attribute
                       {
                           
                       }
                   }
                   namespace RestSourceGenerator.Tests.Samples
                   {
                       using SummerRest.Attributes;
                       public partial class InheritOrCustomA
                       {
                           public InheritOrCustomA Parent { get; set; }
                           [InheritOrCustom] private int _limit;
                           [InheritOrCustom] private int redirect;
                           [InheritOrCustom] private string _origin;
                           public string DoNotGen { get; }
                       }
                   }
                   """;
        var generated = $$$"""
                           // Auto-generated
                           namespace RestSourceGenerator.Tests.Samples
                           {
                               public partial class InheritOrCustomA
                               {
                                   private bool _limitInheritCheck = true;
                                   public int Limit {
                                       get {
                                           if (_limitInheritCheck && {{{DefaultPropertyNames.Parent}}} is not null)
                                                return {{{DefaultPropertyNames.Parent}}}.Limit;
                                            return _limit;
                                       }
                                   }
                                   private bool redirectInheritCheck = true;
                                   public int Redirect {
                                       get {
                                           if (redirectInheritCheck && {{{DefaultPropertyNames.Parent}}} is not null)
                                                return {{{DefaultPropertyNames.Parent}}}.Redirect;
                                            return redirect;
                                       }
                                   }
                                   private bool _originInheritCheck = true;
                                   public string Origin {
                                       get {
                                           if (_originInheritCheck && {{{DefaultPropertyNames.Parent}}} is not null)
                                                return {{{DefaultPropertyNames.Parent}}}.Origin;
                                            return _origin;
                                       }
                                   }
                               }
                           }
                           """;
        await new CSharpSourceGeneratorTest<InheritOrCustomFieldSourceGenerator, XUnitVerifier>
        {
            TestState =
            {
                Sources = { code },
                GeneratedSources =
                {
                    (typeof(InheritOrCustomFieldSourceGenerator),
                        $"InheritOrCustomA.{nameof(ProjectReflection.Attributes.InheritOrCustom)}.{RoslynDefaultValues.PostFixScriptName}",
                        generated.FormatSource()),
                }
            }
        }.RunAsync();

        // // Create an instance of the source generator.
        // var generator = new InheritOrCustomFieldSourceGenerator();
        // // Source generators should be tested using 'GeneratorDriver'.
        // var driver = CSharpGeneratorDriver.Create(generator);
        // // To run generators, we can use an empty compilation.
        // var compilation = CSharpCompilation.Create("RestSourceGenerator.Tests");
        // var type = compilation.GetTypeByMetadataName("RestSourceGenerator.Tests.Samples.InheritOrCustomA");
        // // Run generators. Don't forget to use the new compilation rather than the previous one.
        // driver.RunGeneratorsAndUpdateCompilation(compilation, out var newCompilation, out _);
        //
        // var results = new Dictionary<string, string>
        // {
        //     {
        //         $"{nameof(InheritOrCustomA)}.{nameof(ProjectReflection.Attributes.InheritOrCustom)}.{RoslynDefaultValues.PostFixScriptName}",
        //         
        //         }
        //     };
        //
        // // Retrieve all files in the compilation.
        // foreach (var tree in newCompilation.SyntaxTrees)
        // {
        //     if (!results.TryGetValue(tree.FilePath, out var expected))
        //         Assert.Fail($"The matched result of {tree.FilePath} was not found");
        //     Assert.True(tree.GetText().ContentEquals(expected.FormatSource()));
        // }
    }
}