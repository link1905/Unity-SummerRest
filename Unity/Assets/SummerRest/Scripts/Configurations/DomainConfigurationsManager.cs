using System;
using System.Collections.Generic;
using MemoryPack;
using SummerRest.Models;
using UnityEditor;
using UnityEngine;

namespace SummerRest.Configurations
{
    [CreateAssetMenu(menuName = "Summer/Rest/DomainConfiguration", fileName = "DomainConfigurationsManager", order = 0)]
    [FilePath("Assets/SummerRest/Samples/DomainConfigurationsManager.asset" + nameof(DomainConfigurationsManager), FilePathAttribute.Location.ProjectFolder)]
    public class DomainConfigurationsManager : ScriptableSingleton<DomainConfigurationsManager>, ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector] private byte[] serializedValue;
        [SerializeField] private List<Domain> domains;
        public List<Domain> Domains => domains;
        public void OnBeforeSerialize()
        {
            if (domains == null)
            {
                return;
            }
            serializedValue = MemoryPackSerializer.Serialize(domains);
        }
        public void OnAfterDeserialize()
        {
            if (serializedValue is null || serializedValue.Length == 0)
            {
                domains = new List<Domain>();
            }
            else
            {
                try
                {
                    domains = MemoryPackSerializer.Deserialize<List<Domain>>(serializedValue);
                }
                catch (Exception)
                {
                    domains = new List<Domain>();
                }
            }
        }
    }
}