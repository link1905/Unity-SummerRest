using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.RequestComponents;
using UnityEngine;

namespace SummerRest.Runtime.Authenticate.TokenRepositories
{
    /// <summary>
    /// Default <see cref="IAuthDataRepository"/> that leverages <see cref="PlayerPrefs"/> to store and retrieve data
    /// </summary>
    public class PlayerPrefsAuthDataRepository : IAuthDataRepository
    {
        public void Save<TData>(string key, TData data)
        {
            var json = IDataSerializer.Current.Serialize(data, DataFormat.Json);
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();
        }
        public void Delete(string key)
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }
        public TData Get<TData>(string key)
        {
            return IDataSerializer.Current.Deserialize<TData>(PlayerPrefs.GetString(key, null), DataFormat.Json);
        }
    }
}