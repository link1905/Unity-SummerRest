using System.IO;
using Newtonsoft.Json;
using SummerRest.Editor.Configurations;
using SummerRest.Editor.Models;
using SummerRest.Editor.Utilities;
using UnityEngine;

namespace SummerRest.Editor.Manager
{

    public static class SourceGenerator
    {
        private static string TempSourceIndicatorPath => 
        Path.Combine(Path.GetDirectoryName(Application.dataPath)!, "Temp", 
            "SummerRestSourceGenIndicator.json");
        public static void GenerateAdditionalFile()
        {
            var domains = SummerRestConfigurations.Instance.Domains;
            var configureJson = JsonConvert.SerializeObject(domains);
            FileExtensions.OverwriteFile(TempSourceIndicatorPath, configureJson);
        }
    }
}