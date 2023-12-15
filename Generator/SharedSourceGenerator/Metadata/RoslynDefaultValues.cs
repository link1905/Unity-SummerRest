using System.Text;

namespace SharedSourceGenerator.Metadata
{
    public static class RoslynDefaultValues
    {
        public const string PostFixScriptName = "g.cs";
        public const string Attribute = nameof(Attribute);
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;
    }
}