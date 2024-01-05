using System.Collections.Generic;
using SummerRest.Editor.Models;
using UnityEngine;

namespace SummerRest.Editor.Configurations
{
    [CreateAssetMenu(menuName = "Summer/Rest/SummerRestConfigurations", fileName = "SummerRestConfigurations", order = 0)]
    public partial class SummerRestConfigurations : ScriptableSingleton<SummerRestConfigurations>
    {
        [field: SerializeReference] public List<Domain> Domains { get; private set; } = new();
        [field: SerializeField] public AuthenticateConfigurations AuthenticateConfigurations { get; private set; }
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