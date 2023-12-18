using System.Threading.Tasks;
using RestSourceGenerator.Generators;
using RestSourceGenerator.Metadata;
using RestSourceGenerator.Tests.Utilities;
using SharedSourceGenerator.Metadata;
using SharedSourceGenerator.Utilities;
using Xunit;

namespace RestSourceGenerator.Tests.Tests;

public class InheritOrCustomSourceGeneratorTests
{
    [Fact]
    public async Task GenerateInheritOrCustomFieldsClasses()
    {
        var code = """
                   namespace UnityEngine {
                       using System;
                       public class SerializeField : Attribute { }
                   }
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
                           [InheritOrCustom, SerializeField] private int _limit;
                           [InheritOrCustom, SerializeField] private int redirect;
                           [InheritOrCustom, SerializeField] private string _origin;
                           public string DoNotGen { get; }
                       }
                   }
                   """;
        var generated = $$$"""
                           // Auto-generated
                           using UnityEngine;
                           namespace RestSourceGenerator.Tests.Samples
                           {
                               public partial class InheritOrCustomA
                               {
                                   [SerializeField, HideInInspector] private bool _limitInheritCheck = true;
                                   public int Limit {
                                       get {
                                           if (_limitInheritCheck && {{{DefaultPropertyNames.Parent}}} is not null)
                                                return {{{DefaultPropertyNames.Parent}}}.Limit;
                                            return _limit;
                                       }
                                   }
                                   [SerializeField, HideInInspector] private bool redirectInheritCheck = true;
                                   public int Redirect {
                                       get {
                                           if (redirectInheritCheck && {{{DefaultPropertyNames.Parent}}} is not null)
                                                return {{{DefaultPropertyNames.Parent}}}.Redirect;
                                            return redirect;
                                       }
                                   }
                                   [SerializeField, HideInInspector] private bool _originInheritCheck = true;
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
        await TestsUtilities.SimpleTest<InheritOrCustomFieldSourceGenerator>(code, 
            (typeof(InheritOrCustomFieldSourceGenerator), 
                $"InheritOrCustomA.{ProjectReflection.Attributes.InheritOrCustom.Name}.{RoslynDefaultValues.PostFixScriptName}", 
                generated.FormatSource()));
    }
}