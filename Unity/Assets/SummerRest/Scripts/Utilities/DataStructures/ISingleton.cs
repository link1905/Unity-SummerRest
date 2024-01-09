namespace SummerRest.Utilities.DataStructures
{
    public interface ISingleton<TType> where TType : class, new()
    {
        private static TType _singleton;
        public static TType GetSingleton()
        {
            return _singleton ??= new TType();
        }
    }
}