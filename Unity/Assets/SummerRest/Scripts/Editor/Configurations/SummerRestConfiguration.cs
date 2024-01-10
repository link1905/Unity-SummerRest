using System.Collections.Generic;
using SummerRest.Editor.Models;
using UnityEngine;

namespace SummerRest.Editor.Configurations
{
    //[CreateAssetMenu(menuName = "Summer/Rest/SummerRestConfigurations", fileName = "SummerRestConfigurations", order = 0)]
    public class SummerRestConfiguration : ScriptableSingleton<SummerRestConfiguration>
    {
        [field: SerializeReference] public List<Domain> Domains { get; private set; } = new();
        [field: SerializeReference]
        public AuthenticateConfiguration AuthenticateConfiguration { get; set; }
    }
}