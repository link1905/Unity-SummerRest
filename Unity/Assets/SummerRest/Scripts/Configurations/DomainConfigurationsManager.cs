using System.Collections.Generic;
using System.Linq;
using SummerRest.Models;
using UnityEditor;
using UnityEngine;

namespace SummerRest.Configurations
{
    [CreateAssetMenu(menuName = "Summer/Rest/DomainConfiguration", fileName = "DomainConfigurationsManager", order = 0)]
    [FilePath("Assets/SummerRest/Samples/DomainConfigurationsManager.asset" + nameof(DomainConfigurationsManager), FilePathAttribute.Location.ProjectFolder)]
    public partial class DomainConfigurationsManager : ScriptableSingleton<DomainConfigurationsManager>
    {
        [field: SerializeReference, HideInInspector]
        public List<Domain> Domains { get; private set; }
    }
#if UNITY_EDITOR
    public partial class DomainConfigurationsManager : ISerializationCallbackReceiver
    {

        public void OnBeforeSerialize()
        {
        }
        public void OnAfterDeserialize()
        {
            //Domains.RemoveAll(e => e is null || e.GetInstanceID() <= 0);
        }
    }
#endif
}