namespace SummerRest.Runtime.DataStructures
{
    /// <summary>
    /// Inherit this class to make a simple singleton C# class
    /// </summary>
    /// <typeparam name="TType"></typeparam>
    public interface ISingleton<TType> where TType : class, ISingleton<TType>, new()
    {
        private static TType _singleton;
        public static TType GetSingleton()
        {
            return _singleton ??= new TType();
        }
    }
}