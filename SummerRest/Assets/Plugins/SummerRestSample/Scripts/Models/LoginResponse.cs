using System;

namespace SummerRestSample.Models
{
    [Serializable]
    public struct LoginResponse
    {
        public int id;
        public string token;
    }
}