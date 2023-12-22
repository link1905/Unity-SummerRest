using SummerRest.Models;
using UnityEngine;

namespace SummerRest.Tests
{
    public class TestBehaviour : MonoBehaviour
    {
        [SerializeField] private Request request;
        public void OnBeforeSerialize()
        {
        }
        public void OnAfterDeserialize()
        {
        }
    }
}