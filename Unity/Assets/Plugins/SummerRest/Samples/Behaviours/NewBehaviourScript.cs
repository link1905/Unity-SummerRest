using SummerRest.Editor.TypeReference;
using SummerRest.Runtime.RequestComponents;
using UnityEngine;

namespace SummerRest.Samples.Behaviours
{
    internal class MyRequestBody : IRequestBodyData {} 
    public class MyRequestBody1 : IRequestBodyData {} 
    public class NewBehaviourScript : MonoBehaviour
    {
        private class MyRequestBody2 : IRequestBodyData {} 
        [SerializeField][ClassTypeConstraint(typeof(IRequestBodyData))] 
        private ClassTypeReference classTypeReference;
    }
}
