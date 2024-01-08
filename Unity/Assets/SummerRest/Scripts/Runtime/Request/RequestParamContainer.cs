using System;
using System.Collections.Generic;

namespace SummerRest.Runtime.Request
{
    public class RequestParamContainer
    {
        private readonly Dictionary<string, ICollection<string>> _paramMapper = new();
        public IDictionary<string, ICollection<string>> ParamMapper => _paramMapper;
        internal event Action OnChangedParams;
        public void AddParam(string key, string value)
        {
            if (!_paramMapper.TryGetValue(key, out var values))
            {
                values = new HashSet<string>();
                _paramMapper.Add(key, values);
            }
            values.Add(value);
            OnChangedParams?.Invoke();
        }
        public bool RemoveParam(string key)
        {
            if (!_paramMapper.Remove(key)) 
                return false;
            OnChangedParams?.Invoke();
            return true;
        }
        public bool RemoveValueFromParam(string key, string value)
        {
            if (!_paramMapper.TryGetValue(key, out var values))
                return false;
            var rev = values.Remove(value);
            if (values.Count == 0)
                _paramMapper.Remove(key);
            if (rev)
                OnChangedParams?.Invoke();
            return rev;
        }
        public void AddParams(string key, params string[] addValues)
        {
            if (!_paramMapper.TryGetValue(key, out var values))
            {
                values = new HashSet<string>(addValues);
                _paramMapper.Add(key, values);
            }
            else
            {
                foreach (var value in addValues)
                    values.Add(value);
            }
            OnChangedParams?.Invoke();
        }
    }
}