using System.Linq;
using Microsoft.CodeAnalysis;

namespace RestSourceGenerator.Generators
{
    [Generator]
    public class TestAdditionalFileSourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }
        public void Execute(GeneratorExecutionContext context)
        {
            if (context.Compilation.AssemblyName != "SummerRest")
                return;
            var name = $"{context.Compilation.AssemblyName}.ExampleTest";
            context.AddSource($"{name}.g.cs", $@"
public class ExampleTest {{
    public int A {{ get; set; }} = {context.AdditionalFiles.Length};
}}
");
        }
    }
}