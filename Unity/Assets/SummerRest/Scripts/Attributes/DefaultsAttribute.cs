using UnityEngine;

namespace SummerRest.Scripts.Attributes
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