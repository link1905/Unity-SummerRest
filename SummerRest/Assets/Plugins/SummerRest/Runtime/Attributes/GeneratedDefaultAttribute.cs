using System;

namespace SummerRest.Runtime.Attributes
{
    [AttributeUsage(AttributeTargets.Interface)]
    internal class GeneratedDefaultAttribute : System.Attribute
    {
        public GeneratedDefaultAttribute(string propNameInAdditionalFile, Type @default)
        {
        }
    }
}