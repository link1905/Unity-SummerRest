using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SummerRest.Configurations
{
    public abstract class ScriptableSingleton<T> : ScriptableObject where T : ScriptableSingleton<T>
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance != null) 
                    return _instance;
                var guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
                switch (guids.Length)
                {
                    case > 1:
                        throw new Exception($"There is more than one {typeof(T)} in the project");
                    case 0:
                        throw new Exception($"There is no {typeof(T)} in the project");
                }
                var guid = guids.First();
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                _instance = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                return _instance;
            }
        }
    }
}