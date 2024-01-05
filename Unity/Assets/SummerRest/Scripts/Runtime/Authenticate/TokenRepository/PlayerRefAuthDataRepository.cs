using System;
using Newtonsoft.Json;
using UnityEngine;

namespace SummerRest.Runtime.Authenticate.TokenRepository
{
    public class PlayerRefAuthDataRepository : IAuthDataRepository
    {
        public void Save<TData>(string key, TData data)
        {
            var json = JsonConvert.SerializeObject(data);
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
            var json = PlayerPrefs.GetString(key, null);
            if (json is null)
                return default;
            try
            {
                var obj = JsonConvert.DeserializeObject<TData>(json);
                return obj;
            }
            catch (Exception)
            {
                Debug.LogErrorFormat("Can not deserialize value {0} of key {1} to object of type {2}", json, key, typeof(TData));
            }
            return default;
        }
    }
}