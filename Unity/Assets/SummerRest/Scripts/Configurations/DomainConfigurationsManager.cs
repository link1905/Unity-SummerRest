using SummerRest.Models;
using UnityEditor;
using UnityEngine;

namespace SummerRest.Configurations
{
    [CreateAssetMenu(menuName = "Summer/Rest/DomainConfiguration", fileName = "DomainConfigurationsManager", order = 0)]
    [FilePath("Configurations/SummerRest/" + nameof(DomainConfigurationsManager), FilePathAttribute.Location.ProjectFolder)]
    public class DomainConfigurationsManager : ScriptableSingleton<DomainConfigurationsManager>
    {
        [field: SerializeField] internal Domain[] Domains { get; private set; }
    }
}