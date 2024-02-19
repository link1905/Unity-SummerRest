using System.IO;
using UnityEditor;

namespace Editor
{
    public static class ExportPackage
    {
        public static void Export (string packagePath, string outputPath) {
            // Ensure export path.
            var dir = new FileInfo(outputPath).Directory;
            if (dir is { Exists: false }) {
                dir.Create();
            }
            // Export
            AssetDatabase.ExportPackage(
                packagePath,
                outputPath,
                ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies
            );
        }
    }
}
