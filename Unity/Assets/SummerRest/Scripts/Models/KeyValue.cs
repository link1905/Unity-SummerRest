using System;
using MemoryPack;
using UnityEngine;

namespace SummerRest.Models
{
    [Serializable]
    [MemoryPackable]
    public partial class KeyValue
    {
        [SerializeField, MemoryPackInclude] private string key;
        [MemoryPackIgnore] public string Key => key;
        [SerializeField, MemoryPackInclude] private string value;
        [MemoryPackIgnore] public string Value => value;
    }
}