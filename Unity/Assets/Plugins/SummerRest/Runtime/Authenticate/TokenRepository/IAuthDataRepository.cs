using SummerRest.Runtime.DataStructures;
using SummerRest.Runtime.Parsers;

namespace SummerRest.Runtime.Authenticate.TokenRepository
{
    public interface IAuthDataRepository<TSelf> : ISingleton<TSelf>, IAuthDataRepository where TSelf : class, IAuthDataRepository<TSelf>, new()
    {
    
    }

    public interface IAuthDataRepository : IDefaultSupport<IAuthDataRepository, PlayerPrefsAuthDataRepository>
    {
        void Save<TData>(string key, TData data);
        void Delete(string key);
        TData Get<TData>(string key);
    }
}