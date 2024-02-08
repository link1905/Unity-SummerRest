using System;
using SummerRest.Runtime.RequestComponents;

namespace SummerRestSample.Models
{
    [Serializable]
    public struct Product : IRequestBodyData
    {
        public int id;
        public string title;
        public string description;
        public override string ToString()
        {            
            return $"id: {id}, title: {title}, description: {description}";
        }
    }
}