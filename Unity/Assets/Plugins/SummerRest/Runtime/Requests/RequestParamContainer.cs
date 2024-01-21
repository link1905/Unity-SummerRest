using System;
using System.Collections.Generic;

namespace SummerRest.Runtime.Requests
{
    /// <summary>
    /// Contains parameters of a request
    /// </summary>
    public class RequestParamContainer
    {
        /// <summary>
        /// A param is potent to be a list of values, so we use Collection instead of a single value
        /// </summary>
        private readonly Dictionary<string, ICollection<string>> _paramMapper = new();
        public IDictionary<string, ICollection<string>> ParamMapper => _paramMapper;
        internal event Action OnChangedParams;
        /// <summary>
        /// Add or set a value to a key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
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
        /// <summary>
        /// Remove a param from the request
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Is the key existed</returns>
        public bool RemoveParam(string key)
        {
            if (!_paramMapper.Remove(key)) 
                return false;
            OnChangedParams?.Invoke();
            return true;
        }

        /// <summary>
        /// Remove a value (parameter as a list) from a key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>Are the key and the value existed</returns>
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
        /// <summary>
        /// Add multiple values to a key <br/>
        /// Since a param modification triggers the process of rebuilding URL, you may use this method instead of <see cref="AddParam"/> for better performance
        /// </summary>
        /// <param name="key"></param>
        /// <param name="addValues"></param>
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