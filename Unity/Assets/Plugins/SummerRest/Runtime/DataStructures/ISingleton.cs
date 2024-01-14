namespace SummerRest.Runtime.DataStructures
{
    public interface ISingleton<TType> where TType : class, ISingleton<TType>, new()
    {
        private static TType _singleton;
        public static TType GetSingleton()
        {
            return _singleton ??= new TType();
        }
    }
}