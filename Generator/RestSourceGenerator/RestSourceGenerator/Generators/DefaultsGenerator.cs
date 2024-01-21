using Microsoft.CodeAnalysis;
using RestSourceGenerator.Utilities;

namespace RestSourceGenerator.Generators
{
    [Generator]
    public class DefaultsGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }
        public void Execute(GeneratorExecutionContext context)
        {
            if (context.Compilation.AssemblyName != "SummerRest")
                return;
            var conf = ConfigLoader.Load(context);
            if (!conf.HasValue)
                return;
            context.GenerateFormattedCode("IDataSerializer", $@"
using SummerRest.Runtime.DataStructures;
using SummerRest.Runtime.RequestComponents;

namespace SummerRest.Runtime.Parsers
{{
    public interface IDataSerializer : IDefaultSupport<IDataSerializer, {conf.Value.DataSerializer}>
    {{
        T Deserialize<T>(string data, DataFormat dataFormat);
        string Serialize<T>(T data, DataFormat dataFormat);
    }}
}}
");
            context.GenerateFormattedCode("IAuthDataRepository", $@"
using SummerRest.Runtime.DataStructures;
namespace SummerRest.Runtime.Authenticate.TokenRepositories
{{
    public interface IAuthDataRepository : IDefaultSupport<IAuthDataRepository, {conf.Value.AuthDataRepository}>
    {{
        void Save<TData>(string key, TData data);
        void Delete(string key);
        TData Get<TData>(string key);
    }}
}}
");
        }
    }
}