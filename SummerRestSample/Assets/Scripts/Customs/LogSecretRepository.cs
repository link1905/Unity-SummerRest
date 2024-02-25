using SummerRest.Runtime.Authenticate.Repositories;
using UnityEngine;

namespace Customs
{
    public class LogSecretRepository : ISecretRepository
    {
        private readonly ISecretRepository _wrapped = new PlayerPrefsSecretRepository();
        public void Save<TData>(string key, TData data)
        {
            Debug.LogFormat(@"Save ""{0}"" key", key);
            _wrapped.Save(key, data);
        }
        public void Delete(string key)
        {
            Debug.LogFormat(@"Delete ""{0}"" key", key);
            _wrapped.Delete(key);
        }
        public bool TryGet<TData>(string key, out TData data)
        {
            if (!_wrapped.TryGet(key, out data))
            {
                Debug.LogFormat(@"Key ""{0}"" does not exist", key);
                return false;
            }
            Debug.LogFormat(@"Get ""{0}"" key", key);
            return true;
        }
    }
}
