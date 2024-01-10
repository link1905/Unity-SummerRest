using PropertyAttribute = UnityEngine.PropertyAttribute;

namespace SummerRest.Editor.Attributes
{
    public class TextMultilineAttribute : PropertyAttribute
    {
        public int MinHeight { get; }
        public TextMultilineAttribute(int minHeight = 3)
        {
            MinHeight = minHeight;
        }
    }
}