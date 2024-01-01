using System.Collections.Generic;
using SummerRest.Editor.Models;
using UnityEngine;

namespace SummerRest.Editor.Configurations
{
    [CreateAssetMenu(menuName = "Summer/Rest/SummerRestConfigurations", fileName = "SummerRestConfigurations", order = 0)]
    public partial class SummerRestConfigurations : ScriptableSingleton<SummerRestConfigurations>
    {
        [field: SerializeReference] private List<Domain> domains = new();
        public List<Domain> Domains => domains;
    }
#if UNITY_EDITOR
    public partial class SummerRestConfigurations : ISerializationCallbackReceiver
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