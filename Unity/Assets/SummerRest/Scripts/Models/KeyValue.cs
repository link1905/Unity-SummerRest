using System;
using UnityEngine;

namespace SummerRest.Models
{
    [Serializable]
    public class KeyValue
    {
        [SerializeField] private string key;
        public string Key => key;
        [SerializeField] private string value;
        public string Value => value;
    }
}