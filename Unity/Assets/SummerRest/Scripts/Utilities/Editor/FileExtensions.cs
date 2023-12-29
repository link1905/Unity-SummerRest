using System.IO;

namespace SummerRest.Scripts.Utilities.Editor
{
    public static class FileExtensions
    {
        public static string GetParentPath(string path) => Path.GetDirectoryName(path);
        public static string Combine(params string[] paths) => Path.Combine(paths);
        public static void EnsurePathExist(string filePath)
        {
            var parent = GetParentPath(filePath);
            if (Directory.Exists(parent))
                Directory.CreateDirectory(parent);
        }
        public static void OverwriteFile(string path, string content)
        {
            EnsurePathExist(path);
            File.Delete(path);
            File.WriteAllText(path, content);
        }
    }
}