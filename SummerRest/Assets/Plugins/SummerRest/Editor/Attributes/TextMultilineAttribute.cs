using PropertyAttribute = UnityEngine.PropertyAttribute;

namespace SummerRest.Editor.Attributes
{
    /// <summary>
    /// Show a <see cref="string"/> field on multiline
    /// </summary>
    public class TextMultilineAttribute : PropertyAttribute
    {
        public int MinHeight { get; }
        public TextMultilineAttribute(int minHeight = 3)
        {
            MinHeight = minHeight;
        }
    }
}