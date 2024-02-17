using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SummerRest.Editor.DataStructures
{
    public class SingletonException : Exception
    {
        public int Count { get; }
        public SingletonException(int count, string msg) : base(msg)
        {
            Count = count;
        }
    }
    public abstract class ScriptableSingleton<T> : ScriptableObject where T : ScriptableSingleton<T>
    {
        private static T _instance;
        public static bool Initialized => _instance is not null;
        public static T Instance
        {
            get
            {
                LoadInstance();
                return _instance;
            }
        }

        protected static void LoadInstance()
        {
            if (Initialized)
                return;
            var guids = AssetDatabase.FindAssets($"t:{typeof(T)} a:assets");
            switch (guids.Length)
            {
                case > 1:
                    throw new SingletonException(guids.Length, $"There is more than one {typeof(T)} in the project");
                case 0:
                    throw new SingletonException(0, $"There is no {typeof(T)} in the project");
            }
            var guid = guids.First();
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            _instance = AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }
    }
}