using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("SummerRest.Editor")]
namespace SummerRest.Runtime.Attributes
{
    /// <summary>
    /// For drawing a field as a dropdown with default values
    /// </summary>
    internal class DefaultsAttribute : PropertyAttribute
    {
        public string[] Defaults { get; }
        public DefaultsAttribute(params string[] defaults)
        {
            Defaults = defaults;
        }
    }
}