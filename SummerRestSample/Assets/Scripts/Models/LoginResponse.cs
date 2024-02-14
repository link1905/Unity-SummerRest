using System;

namespace Models
{
    [Serializable]
    public struct LoginResponse
    {
        public int id;
        public string token;
    }
}