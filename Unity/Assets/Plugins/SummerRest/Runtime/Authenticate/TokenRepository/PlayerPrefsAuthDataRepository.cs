using System.Collections.Concurrent;
using System.Collections.Generic;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.RequestComponents;
using UnityEngine;

namespace SummerRest.Runtime.Authenticate.TokenRepository
{
    public class PlayerPrefsAuthDataRepository : IAuthDataRepository<PlayerPrefsAuthDataRepository>
    {
        private readonly ConcurrentDictionary<string, string> _cache = new();
        private readonly IDataSerializer _dataSerializer = new DefaultDataSerializer();
        public void Save<TData>(string key, TData data)
        {
            var json = _dataSerializer.Serialize(data, DataFormat.Json);
            _cache[key] = json;
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();
        }
        public void Delete(string key)
        {
            _cache.Remove(key, out _);
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }
        public TData Get<TData>(string key)
        {
            if (!_cache.TryGetValue(key, out var json))
                json = PlayerPrefs.GetString(key, null);
            return _dataSerializer.Deserialize<TData>(json, DataFormat.Json);
        }
    }
}