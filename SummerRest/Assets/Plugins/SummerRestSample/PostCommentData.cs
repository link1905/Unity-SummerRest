using System;
using SummerRest.Runtime.RequestComponents;

namespace SummerRestSample
{
    [Serializable]
    public struct PostCommentData : IRequestBodyData
    {
        public int id;
        public int postId;
        public string name;
        public string email;
        public string body;
        public override string ToString()
        {            
            return $"id: {id}, postId: {postId}, name: {name}, email: {email}, body: {body}";
        }
    }
}