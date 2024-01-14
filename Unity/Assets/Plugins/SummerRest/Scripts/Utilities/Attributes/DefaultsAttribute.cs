﻿using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("SummerRest.Editor")]
namespace SummerRest.Utilities.Attributes
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