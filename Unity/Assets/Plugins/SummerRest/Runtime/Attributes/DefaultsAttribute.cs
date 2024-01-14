using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("SummerRest.Editor")]
namespace SummerRest.Runtime.Attributes
{
    internal class DefaultsAttribute : PropertyAttribute
    {
        public string[] Defaults { get; }
        public DefaultsAttribute(params string[] defaults)
        {
            Defaults = defaults;
        }
    }
}