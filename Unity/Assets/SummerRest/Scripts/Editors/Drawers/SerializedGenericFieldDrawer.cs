using System;
using SummerRest.Scripts.Attributes;
using UnityEditor;
using UnityEngine;

namespace SummerRest.Scripts.Editors.Drawers
{
    /*
    [Serializable]
    internal class RequestParam
    {
        [field: SerializeField] public string Key { get; private set; }
        // Generated
        [SerializeField, Inherits(typeof(IRequestParam), typeof(bool), typeof(string), typeof(float))]
        private TypeReference valueTypeRef = new TypeReference(typeof(bool));
        public Type ValueType => valueTypeRef.Type;
        [SerializeField, HideInInspector] private byte[] valueUnderlying;
        //
        
        [field: SerializeReference, SerializedGenericField(typeof(bool), typeof(bool), typeof(string), typeof(float))] 
        public IRequestParam Value { get; private set; }
    }
    */


    [CustomPropertyDrawer(typeof(SerializedGenericField))]
    internal class SerializedGenericFieldDrawer : PropertyDrawer
    {
        private static string UnderlyingValue(string name) => $"{name}Underlying";
        private static string TypeRef(string name) => $"{name}TypeRef";
        private string _underlyingValueName;
        private string _typeRefName;
        
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            
            
            EditorGUILayout.LabelField(label);
            EditorGUI.EndProperty();
        }
    }
}