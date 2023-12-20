using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using RestSourceGenerator.Generators;
using RestSourceGenerator.Metadata;
using RestSourceGenerator.Tests.Utilities;
using SharedSourceGenerator.Metadata;
using SharedSourceGenerator.Utilities;
using Xunit;

namespace RestSourceGenerator.Tests.Tests;

public class SerializedGenericFieldSourceGeneratorTests
{
    [Fact]
    public async Task GenerateSerializedGenericField()
    {
        var code = """
                   namespace SummerRest.Attributes {
                       using System;
                       [AttributeUsage(AttributeTargets.Field)]
                       public class SerializedGenericFieldAttribute : Attribute
                        {
                            public Type DefaultType { get; }
                            public Type[] BaseTypes { get; }
                            public SerializedGenericField(Type defaultType, params Type[] baseTypes)
                            {
                                DefaultType = defaultType;
                                BaseTypes = baseTypes;
                            }
                        }
                   }
                   namespace RestSourceGenerator.Tests.Samples
                   {
                       using SummerRest.Attributes;
                       public interface IRequestParamValue
                       { }
                       public class RequestParamValue : IRequestParamValue {}
                       [Serializable]
                       public partial class RequestParam
                       {
                           [SerializedGenericField(typeof(RequestParamValue))]
                           private IRequestParamValue _value;
                       }
                   }
                   """;
        var generated = """
                        // Auto-generated
                        using System;
                        using SummerRest.Attributes;
                        using SummerRest.DataStructures;
                        using TypeReferences;
                        using UnityEngine;
                        namespace RestSourceGenerator.Tests.Samples
                        {
                            public partial class RequestParam
                            {
                                [SerializeField, HideInInspector] private ValueContainer valueContainer;
                                public RestSourceGenerator.Tests.Samples.IRequestParamValue Value => valueContainer.Value;
                                public Type ValueType => valueContainer.Type;
                                [Serializable]
                                public class ValueContainer : InterfaceContainer<RestSourceGenerator.Tests.Samples.IRequestParamValue>
                                {
                                    [SerializeField, HideInInspector, Inherits(typeof(RestSourceGenerator.Tests.Samples.IRequestParamValue))]
                                    private TypeReference typeRef = new(typeof(RestSourceGenerator.Tests.Samples.RequestParamValue));
                                    public override Type Type
                                    {
                                        get
                                        {
                                            if (typeRef.Type is null)
                                                typeRef.Type = typeof(RestSourceGenerator.Tests.Samples.RequestParamValue);
                                            return System.Type.GetType(typeRef.TypeNameAndAssembly);      
                                        }
                                    } 
                                }
                            }
                        }
                        """;

        await TestsUtilities.SimpleTest<SerializedGenericFieldSourceGenerator>(code, 
            (typeof(SerializedGenericFieldSourceGenerator),
            $"RequestParam.{ProjectReflection.Attributes.SerializedGenericField.Name}.{RoslynDefaultValues.PostFixScriptName}",
            generated.FormatSource()));
    }
}