namespace SummerRest.Runtime.DataStructures
{
    public interface IDefaultSupport<TInterface, TDefault> where TDefault : TInterface, new()
    {
        public static TInterface Current { get; set; } = new TDefault();
    }
}