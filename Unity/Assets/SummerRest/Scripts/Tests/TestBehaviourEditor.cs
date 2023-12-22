using UnityEditor;
using UnityEngine;

namespace SummerRest.Tests
{
    [CustomEditor(typeof(TestBehaviour))]
    public class TestBehaviourEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (!GUILayout.Button("Click"))
                return;
            if (target is not TestBehaviour b)
                return;
            b.request.Add(new A());
        }
    }
}