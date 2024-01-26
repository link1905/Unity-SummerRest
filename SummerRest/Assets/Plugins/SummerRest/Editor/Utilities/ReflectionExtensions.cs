using System;
using System.Reflection;

namespace SummerRest.Editor.Utilities
{
    public static class ReflectionExtensions
    {
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