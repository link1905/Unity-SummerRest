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
        /// <summary>
        /// Save data to a key
        /// </summary>
        /// <param name="key">Please access <see cref="AuthKeys"/> to get the hard key</param>
        /// <param name="data"></param>
        /// <typeparam name="TData"></typeparam>
        void Save<TData>(string key, TData data);
        /// <summary>
        /// Delete data from a key
        /// </summary>
        /// <param name="key">Please access <see cref="AuthKeys"/> to get the hard key</param>
        void Delete(string key);
        /// <summary>
        /// Get data of a key
        /// </summary>
        /// <param name="key">Please access <see cref="AuthKeys"/> to get the hard key</param>
        /// <param name="data"></param>
        /// <returns>Is the key existed</returns>
        bool TryGet<TData>(string key, out TData data);
    }
}