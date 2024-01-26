using System;
using SummerRest.Runtime.RequestComponents;

namespace SummerRestSample
{
    [Serializable]
    public struct PostData : IRequestBodyData
    {
        public int id;
        public string title;
        public string body;
        public int userId;
    }
}