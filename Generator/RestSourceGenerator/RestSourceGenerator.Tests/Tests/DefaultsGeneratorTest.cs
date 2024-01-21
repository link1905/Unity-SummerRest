using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using RestSourceGenerator.Generators;
using RestSourceGenerator.Utilities;
using Xunit;

namespace RestSourceGenerator.Tests.Tests;

public class DefaultsGeneratorTest : CSharpSourceGeneratorTest<DefaultsGenerator, XUnitVerifier>
{
    protected override string DefaultTestProjectName => "SummerRest";
    [Fact]
    public Task Test_Generate_Data_Serializer_And_Auth_Repos()
    {
        var jsonContent = """
                    {
                        "DataSerializer": "TestNamespace.TestDataSerializer",
                        "AuthDataRepository": "TestNamespace.TestAuthDataRepository"
                    }    
                    """;

        var dataSerializerExpect = """
using SummerRest.Runtime.DataStructures;
using SummerRest.Runtime.RequestComponents;

namespace SummerRest.Runtime.Parsers
{
    public interface IDataSerializer : IDefaultSupport<IDataSerializer, TestNamespace.TestDataSerializer>
    {
        T Deserialize<T>(string data, DataFormat dataFormat);
        string Serialize<T>(T data, DataFormat dataFormat);
    }
}
""";
        var authReposExpect = """
using SummerRest.Runtime.DataStructures;
namespace SummerRest.Runtime.Authenticate.TokenRepositories
{
    public interface IAuthDataRepository : IDefaultSupport<IAuthDataRepository, TestNamespace.TestAuthDataRepository>
    {
        void Save<TData>(string key, TData data);
        void Delete(string key);
        TData Get<TData>(string key);
    }
}
""";
        
        var test = new DefaultsGeneratorTest
        {
            TestState =
            {
                AdditionalFiles = { ("summer-rest-generated.RestSourceGenerator.additionalfile", jsonContent) },
                GeneratedSources =
                {
                    (typeof(DefaultsGenerator), "IDataSerializer.g.cs", dataSerializerExpect.FormatCode()),
                    (typeof(DefaultsGenerator), "IAuthDataRepository.g.cs", authReposExpect.FormatCode()) 
                }
            },
            CompilerDiagnostics = CompilerDiagnostics.None
        };
        return test.RunAsync();
    } 
}