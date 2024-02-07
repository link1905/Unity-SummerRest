using System;
using System.Collections.Generic;

namespace SummerRest.Runtime.Requests
{
    /// <summary>
    /// Contains parameters of a request <br/>
    /// Since a parameter is potential to be single or multiple (list) <br/>
    /// Use <see cref="SetSingleParam"/> if you intend to work with a single param
    /// Use <see cref="AddParamToList(string,string)"/> <see cref="RemoveValueFromList"/> if you intend to work with a multi-valued param
    /// </summary>
    public class RequestParamContainer
    {
        /// <summary>
        /// A param is potent to be a list of values, so we use Collection instead of a single value
        /// </summary>
        private readonly Dictionary<string, ICollection<string>> _paramMapper = new();
        public IDictionary<string, ICollection<string>> ParamMapper => _paramMapper;
        internal event Action OnChangedParams;
        public ICollection<string> this[in string key] =>
            _paramMapper.TryGetValue(key, out var @params) ? @params : null;

        /// <summary>
        /// Please use this method instead of <see cref="AddParamToList(string,string)"/> if you intend to work with a single param instead of a list <br/>
        /// Technically, this method clear the param list of the key then add the new value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public void SetSingleParam(string key, string value)
        {
            if (_paramMapper.TryGetValue(key, out var @params))
            {
                @params.Clear();
                @params.Add(value);
                OnChangedParams?.Invoke();
            }
            else
                AddParamToList(key, value);
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
        public bool RemoveValueFromList(string key, string value)
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
        /// Add or set a value to a key (param list)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddParamToList(string key, string value)
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
        /// Add multiple values to a key <br/>
        /// Since a param modification triggers the process of rebuilding URL, you may use this method for better performance
        /// </summary>
        /// <param name="key"></param>
        /// <param name="addValues"></param>
        public void AddParamToList(string key, params string[] addValues)
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

        public void Clear()
        {
            _paramMapper.Clear();
        }
    }
}