using System.Text;

namespace RestSourceGenerator.Utilities
{
    public static class RoslynDefaultValues
    {
        public const string PostFixScriptName = "g.cs";
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;
        public const string Commas = ",";
        public const string SemiColons = ";";
        public const string Null = "null";
        public const string String = "string";
        public const string Int = "int";
        public static string Array(string type) => $"{type}[]";
        public static string EmptyArray(string type) => $"System.Array.Empty<{type}>()";
    }
}