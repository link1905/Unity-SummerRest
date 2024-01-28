using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SummerRest.Editor.Models;
using SummerRest.Editor.TypeReference;
using SummerRest.Editor.Utilities;
using SummerRest.Runtime.Authenticate.TokenRepositories;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.Requests;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace SummerRest.Editor.Configurations
{
    /// <summary>
    /// Singleton class storing the configuration of the plugin <br />
    /// </summary>
    public class SummerRestConfiguration : DataStructures.ScriptableSingleton<SummerRestConfiguration>
    {
        /// <summary>
        /// Domains
        /// </summary>
        [field: SerializeReference] public List<Domain> Domains { get; private set; } = new();

        /// Each <see cref="AuthContainer"/> points to an auth data (userId, token...)/><br /> 
        [SerializeField] private List<AuthContainer> authContainers = new();

        public List<AuthContainer> AuthContainers => authContainers;
        
        /// <summary>
        /// Type ref of auth repository <see cref="IAuthDataRepository"/> <br />
        /// Default is <see cref="PlayerPrefsAuthDataRepository"/>
        /// </summary>
        [SerializeField, ClassTypeConstraint(typeof(IAuthDataRepository))] 
        private ClassTypeReference authDataRepository = new(typeof(PlayerPrefsAuthDataRepository));
        public string AuthDataRepository => authDataRepository.Type is null ? typeof(PlayerPrefsAuthDataRepository).FullName : authDataRepository.Type.FullName;
        /// <summary>
        /// Type ref of data serializer <see cref="IDataSerializer"/> <br />
        /// Default is <see cref="DefaultDataSerializer"/>
        /// </summary>
        [SerializeField, ClassTypeConstraint(typeof(IDataSerializer))] 
        private ClassTypeReference dataSerializer = new(typeof(DefaultDataSerializer));
        public string DataSerializer => dataSerializer.Type is null ? typeof(DefaultDataSerializer).FullName : dataSerializer.Type.FullName;

        /// <summary>
        /// The target assembly for generating <see cref="BaseRequest{TRequest}"/> classes to call apis in runtime
        /// </summary>
        [SerializeReference] private AssemblyDefinitionAsset targetAssembly;
        private class AssemblyName
        {
            [JsonProperty("name")] public string Name { get; private set; }
        }

        /// <summary>
        /// Default assembly of Unity
        /// </summary>
        private const string UnityFirstPass = "Assembly-CSharp-firstpass";
        public string Assembly
        {
            get
            {
                if (targetAssembly is null)
                    return UnityFirstPass;
                try
                {
                    // Unity does not provide any way to access name of an AssemblyDefinitionAsset
                    // We need to deserialize back into a class first to get the name
                    var assembly = JsonConvert.DeserializeObject<AssemblyName>(targetAssembly.text);
                    return assembly.Name;
                }
                catch (Exception)
                {
                    return UnityFirstPass;
                }
            }
        }
        public void RenameAssets()
        {
            for (int i = 0; i < Domains.Count; i++)
                Domains[i].Rename(string.Empty, i);
            AssetDatabase.SaveAssets();
        }
    }
}