using SummerRest.Runtime.Parsers;

namespace SummerRest.Runtime.Authenticate.TokenRepository
{
    public interface IAuthDataRepository : IDefaultSupport<IAuthDataRepository, PlayerRefAuthDataRepository>
    {
        void Save<TData>(string key, TData data);
        void Delete(string key);
        TData Get<TData>(string key);
    }
}