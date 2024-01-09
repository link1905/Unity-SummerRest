using UnityEngine;

namespace SummerRest.Utilities.Attributes
{
    public class DefaultsAttribute : PropertyAttribute
    {
        public string[] Defaults { get; }
        public DefaultsAttribute(params string[] defaults)
        {
            Defaults = defaults;
        }
    }
}