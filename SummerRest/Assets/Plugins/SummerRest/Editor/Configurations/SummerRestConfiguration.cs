using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SummerRest.Editor.Models;
using SummerRest.Editor.TypeReference;
using SummerRest.Runtime.Authenticate.Repositories;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.Requests;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Serialization;
using ISecretRepository = SummerRest.Runtime.Authenticate.Repositories.ISecretRepository;

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
        /// Type ref of auth repository <see cref="ISecretRepository"/> <br />
        /// Default is <see cref="PlayerPrefsSecretRepository"/>
        /// </summary>
        [FormerlySerializedAs("authDataRepository")] [SerializeField, ClassTypeConstraint(typeof(ISecretRepository))] 
        private ClassTypeReference secretRepository = new(typeof(PlayerPrefsSecretRepository));
        public string SecretRepository => secretRepository.Type is null ? typeof(PlayerPrefsSecretRepository).FullName : secretRepository.Type.FullName;
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