using SummerRest.Models;
using UnityEngine;

namespace SummerRest.Configurations
{
    [CreateAssetMenu(menuName = "Summer/Rest/DomainConfiguration", fileName = "DomainConfigurationsManager", order = 0)]
    internal class DomainConfigurationsManager : ScriptableObject
    {
        [field: SerializeField] internal Domain[] Domains { get; private set; }
    }
}