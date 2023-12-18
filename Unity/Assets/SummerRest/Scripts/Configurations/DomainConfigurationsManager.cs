using SummerRest.Scripts.Models;
using UnityEngine;

namespace SummerRest.Scripts.Configurations
{
    internal class DomainConfigurationsManager : ScriptableObject
    {
        [field: SerializeField] internal Domain[] Domains { get; private set; }
    }
}