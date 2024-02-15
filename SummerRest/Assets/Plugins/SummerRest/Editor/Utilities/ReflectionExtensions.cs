using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Compilation;
using Assembly = System.Reflection.Assembly;

namespace SummerRest.Editor.Utilities
{
    public static class ReflectionExtensions
    {
        public static IEnumerable<Assembly> GetAllAssemblies()
        {
            return CompilationPipeline.GetAssemblies().Select(e => Assembly.Load(e.name));
        } 
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
        public static string LoadDefaultAssemblyName()
        {
            try
            {
                return LoadDefaultAssembly().GetName().Name;
            }
            catch (Exception)
            {
                throw new Exception(
                    "There is no script in default assembly of your project! Please select a target assembly or create at least 1 script");
            }
        }

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