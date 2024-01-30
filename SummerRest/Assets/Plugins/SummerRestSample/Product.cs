using System;
using SummerRest.Runtime.RequestComponents;

namespace SummerRestSample
{
    [Serializable]
    public struct Product : IRequestBodyData
    {
        public int id;
        public int title;
        public string description;
        public override string ToString()
        {            
            return $"id: {id}, title: {title}, description: {description}";
        }
    }
}