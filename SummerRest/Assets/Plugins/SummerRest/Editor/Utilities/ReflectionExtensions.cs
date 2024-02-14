using System;
using System.Reflection;

namespace SummerRest.Editor.Utilities
{
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Default assembly of Unity
        /// </summary>
        private const string DefaultUnityFirstPassAssembly = "Assembly-CSharp-firstpass";
        private const string DefaultUnityAssembly = "Assembly-CSharp";
        public static Assembly LoadDefaultAssembly()
        {
            try
            {
                return Assembly.Load(DefaultUnityFirstPassAssembly);
            }
            catch (Exception)
            {
                return Assembly.Load(DefaultUnityAssembly);
            }
        }
        public static string LoadDefaultAssemblyName() => LoadDefaultAssembly().GetName().Name;
        public static void CallGenericMethod(this object obj, string methodName, Type[] typeArguments, params object[] parameters)
        {
            Type objectType = obj.GetType();
            MethodInfo method = objectType.GetMethod(methodName);
            MethodInfo genericMethod = method.MakeGenericMethod(typeArguments);
            // Invoke the generic method
            genericMethod.Invoke(obj, parameters);
        }
        public static void CallMethod(this object obj, string methodName, params object[] parameters)
        {
            Type objectType = obj.GetType();
            MethodInfo method = objectType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            method?.Invoke(obj, parameters);
        }
    }
}