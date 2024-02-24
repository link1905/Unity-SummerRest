using System.IO;
using SummerRest.Editor.Configurations;

namespace SummerRest.Editor.Utilities
{
    public static class PathsHolder
    {
        /// <summary>
        /// When embed in Assets folder
        /// </summary>
        public const string AssetDir = "Assets/Plugins/SummerRest/Editor/Templates";
        /// <summary>
        /// When used as a package
        /// </summary>
        public const string PackageDir = "Packages/com.summer.summer-rest/Editor/Templates";
        public const string DomainsFolder = "Domains";
        public const string ResponsesFolder = "Responses";
        public const string SourceGeneratedAdditionalFile = "summer-rest-generated.RestSourceGenerator.additionalfile";
        public static string ConfigurePath => SummerRestConfiguration.Instance.GetAssetFolder();
        public static string DomainsPath => Path.Combine(ConfigurePath, DomainsFolder);
        public static string ResponsesPath => Path.Combine(DomainsPath, ResponsesFolder);
        public static string SourceGeneratedAdditionalFilePath => Path.Combine(ConfigurePath, SourceGeneratedAdditionalFile);
    }
}