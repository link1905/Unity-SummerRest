using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SummerRest.Editor.Models;
using SummerRest.Editor.Utilities;
using SummerRest.Utilities.DataStructures;
using UnityEditorInternal;
using UnityEngine;

namespace SummerRest.Editor.Configurations
{
    //[CreateAssetMenu(menuName = "Summer/Rest/SummerRestConfigurations", fileName = "SummerRestConfigurations", order = 0)]
    public class SummerRestConfiguration : ScriptableSingleton<SummerRestConfiguration>
    {
        [field: SerializeReference] public List<Domain> Domains { get; private set; } = new();
        [field: SerializeReference][JsonIgnore]
        public AuthenticateConfiguration AuthenticateConfiguration { get; set; }
        [SerializeReference] private AssemblyDefinitionAsset targetAssembly;
        private class AssemblyName
        {
            [JsonProperty("name")] public string Name { get; private set; }
        }
        public string Assembly
        {
            get
            {
                if (targetAssembly is null)
                    return "Assembly-CSharp-firstpass"; //Default assembly name
                try
                {
                    var assembly = JsonConvert.DeserializeObject<AssemblyName>(targetAssembly.text);
                    return assembly.Name;
                }
                catch (Exception)
                {
                    return "Assembly-CSharp-firstpass"; //Default project
                }
            }
        }
    }
}