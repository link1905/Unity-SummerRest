using System.Collections.Generic;
using Newtonsoft.Json;
using SummerRest.Models;
using UnityEngine;

namespace SummerRest.Configurations
{
    [CreateAssetMenu(menuName = "Summer/Rest/DomainConfiguration", fileName = "DomainConfigurationsManager", order = 0)]
    public partial class DomainConfigurationsManager : ScriptableSingleton<DomainConfigurationsManager>
    {
        [field: SerializeReference] private List<Domain> domains = new();
        public List<Domain> Domains => domains;
    }
#if UNITY_EDITOR
    public partial class DomainConfigurationsManager : ISerializationCallbackReceiver
    {
        public void OnBeforeSerialize()
        {
        }
        public void OnAfterDeserialize()
        {
        }
    }
#endif
}