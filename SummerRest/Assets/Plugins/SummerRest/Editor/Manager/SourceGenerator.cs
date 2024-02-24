using System.IO;
using SummerRest.Editor.Configurations;
using SummerRest.Editor.Utilities;
using SummerRest.Runtime.Parsers;
using SummerRest.Runtime.RequestComponents;
using SummerRest.Runtime.Requests;
using UnityEditor;
using UnityEngine;

namespace SummerRest.Editor.Manager
{

    /// <summary>
    /// Generate a additional JSON file for automatically generating <see cref="BaseRequest{TRequest}"/> <br/>
    /// The generating process happens inside Roslyn source generator<br/>
    /// </summary>
    public static class SourceGenerator
    {
        private const string FileName = "summer-rest-generated.RestSourceGenerator.additionalfile";
        public static void GenerateAdditionalFile()
        {
            var conf = SummerRestConfiguration.Instance;
            // Will throw exceptions at this step if the conf is not satisfied
            conf.ValidateToGenerate();
            
            var path = conf.GetAssetFolder() + "/" + FileName;
            var configureXml = IDataSerializer.Current.Serialize(conf, DataFormat.Xml, true);
            var jsonAsset = EditorAssetUtilities.LoadOrCreate(path, () => new TextAsset());
            if (jsonAsset is null)
            {
                Debug.LogErrorFormat("Can not create new asset file {0} at {1}", FileName, path);
                return;
            }
            EditorAssetUtilities.WriteTextContent(path, configureXml);
            AssetDatabase.ImportAsset(path);
            // Ensure the Editor to reload to run Roslyn processes 
            EditorUtility.RequestScriptReload();
        }
    }
}