using SummerRest.Runtime.Attributes;
using UnityEngine;

namespace SummerRest.Runtime.Authenticate.TokenRepositories
{
    /// <summary>
    /// Class for resolving auth value queries <br/>
    /// It may be wise to store your secret values in another place (disk,vault,database...) instead of current running application <br/>
    /// Default support is <see cref="PlayerPrefsAuthDataRepository"/> based on <see cref="PlayerPrefs"/>, you can change it to your own repository in the plugin window
    /// </summary>
    [GeneratedDefault("AuthDataRepository", typeof(PlayerPrefsAuthDataRepository))]
    public partial interface IAuthDataRepository
    {
        void Save<TData>(string key, TData data);
        void Delete(string key);
        bool TryGet<TData>(string key, out TData data);
    }
}