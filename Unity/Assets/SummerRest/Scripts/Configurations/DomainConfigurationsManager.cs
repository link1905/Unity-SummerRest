using System.Collections.Generic;
using MemoryPack;
using SummerRest.Models;
using UnityEditor;
using UnityEngine;

namespace SummerRest.Configurations
{
    [CreateAssetMenu(menuName = "Summer/Rest/DomainConfiguration", fileName = "DomainConfigurationsManager", order = 0)]
    [FilePath("Configurations/SummerRest/" + nameof(DomainConfigurationsManager), FilePathAttribute.Location.ProjectFolder)]
    public class DomainConfigurationsManager : ScriptableSingleton<DomainConfigurationsManager>, ISerializationCallbackReceiver
    {
        [SerializeField] private byte[] serializedValue;
        [SerializeField, HideInInspector] public List<Domain> domains;
        public List<Domain> Domains => domains;
        public void OnBeforeSerialize()
        {
            serializedValue = MemoryPackSerializer.Serialize(domains);
        }
        public void OnAfterDeserialize()
        {
            if (serializedValue is null || serializedValue.Length == 0)
                domains = new List<Domain>();
            else
                domains = MemoryPackSerializer.Deserialize<List<Domain>>(serializedValue);
        }
    }
}