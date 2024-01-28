using SummerRest.Runtime.Attributes;
using UnityEngine;

namespace SummerRest.Runtime.Authenticate.Repositories
{
    /// <summary>
    /// Class for resolving auth value queries <br/>
    /// It may be wise to store your secret values in another place (disk,vault,database...) instead of current running application <br/>
    /// Default support is <see cref="PlayerPrefsSecretRepository"/> based on <see cref="PlayerPrefs"/>, you can change it to your own repository in the plugin window
    /// </summary>
    [GeneratedDefault("SecretRepository", typeof(PlayerPrefsSecretRepository))]
    public partial interface ISecretRepository
    {
        void Save<TData>(string key, TData data);
        void Delete(string key);
        bool TryGet<TData>(string key, out TData data);
    }
}