using UnityEngine;
using UnityEngine.UIElements;

namespace SummerRest.Utilities
{
    public static class StylesExtensions
    {
        public static void ReplaceBackgroundColor(this IStyle style, Color color)
        {
            var oldStyle = style.backgroundColor;
            oldStyle.value = color;
            style.backgroundColor = oldStyle;
        }

    }
}