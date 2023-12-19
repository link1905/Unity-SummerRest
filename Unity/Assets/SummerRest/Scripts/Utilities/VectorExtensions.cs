using UnityEngine;

namespace SummerRest.Utilities
{
    public static class VectorExtensions
    {
        public static Vector2 Abs(this Vector2 vec)
        {
            return new Vector2(Mathf.Abs(vec.x), Mathf.Abs(vec.y));
        } 

    }
}