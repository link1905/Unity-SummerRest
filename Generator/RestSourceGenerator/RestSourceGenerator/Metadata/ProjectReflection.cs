using SharedSourceGenerator.Metadata;

namespace RestSourceGenerator.Metadata
{
    public static class DefaultPropertyNames
    {
        public const string Parent = nameof(Parent);
    }

    public static class UnityEngine
    {
        public const string SerializeField = "UnityEngine.SerializeField";
    }
    public static class ProjectReflection
    {
        public const string ProjectAssemblyName = "SummerRest";
        
        public static class Attributes
        {
            public const string AttributesRoot = ProjectAssemblyName + CSharpReservedNames.Dot + nameof(Attributes);
            public static class InheritOrCustom
            {
                public const string Name = nameof(InheritOrCustom) + RoslynDefaultValues.Attribute;
                public const string FullName =  AttributesRoot + CSharpReservedNames.Dot + Name;
            }
            public static class SerializedGenericField
            {
                public const string Name = nameof(SerializedGenericField) + RoslynDefaultValues.Attribute;
                public const string FullName =  AttributesRoot + CSharpReservedNames.Dot + Name;
                public const string Container = nameof(Container);
                public const string Value = nameof(Value);
                public const string Type = nameof(Type);

            }
        }
    }
}