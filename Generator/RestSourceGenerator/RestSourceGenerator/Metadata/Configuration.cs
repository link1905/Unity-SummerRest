using System.Text;
using System.Xml.Serialization;
using RestSourceGenerator.Utilities;

namespace RestSourceGenerator.Metadata
{
    [XmlRoot(nameof(ProjectReflection.SummerRestConfiguration))]
    public struct Configuration
    {
        [XmlAttribute]
        public string Assembly { get; set; }
        [XmlArray, XmlArrayItem(nameof(ProjectReflection.SummerRestConfiguration.Domain))]
        public Request[]? Domains { get; set; }
        [XmlArray]
        public string[] AuthKeys { get; set; }

        public void BuildDomainClasses(StringBuilder builder)
        {
            if (Domains is {Length: >0})
            {
                foreach (var request in Domains)
                    request.BuildClass(builder);
            }
        }
        public string BuildAuthClass()
        {
            return AuthKeys is not { Length: > 0 }
                ? string.Empty
                : AuthKeys.BuildSequentialValues((k, _) => @$"public const string {k.ToClassName()} = {k.ToEmbeddedString()}", 
                    separator: RoslynDefaultValues.SemiColons, end: RoslynDefaultValues.SemiColons);
        }
    }
}