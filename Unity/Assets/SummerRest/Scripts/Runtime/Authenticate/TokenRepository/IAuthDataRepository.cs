using SummerRest.Runtime.Parsers;
using SummerRest.Utilities.DataStructures;

namespace SummerRest.Runtime.Authenticate.TokenRepository
{
    public interface IAuthDataRepository : IDefaultSupport<IAuthDataRepository, PlayerRefAuthDataRepository>
    {
        void Save<TData>(string key, TData data);
        void Delete(string key);
        TData Get<TData>(string key);
    }
}