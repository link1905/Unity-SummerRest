using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Editor
{
    public static class ExportPackage
    {
        private static readonly string Eol = Environment.NewLine;
        private static readonly string[] Needs = {"packagePath", "outputPath"};
        public static void Export () {
            var validatedOptions = ParseCommandLineArguments();
            var packagePath = validatedOptions["packagePath"];
            var outputPath = validatedOptions["outputPath"];
            // Ensure export path.
            var dir = new FileInfo(outputPath).Directory;
            if (dir is { Exists: false }) {
                Console.WriteLine($"The directory of {outputPath} does not exist => automatically create a new one");
                dir.Create();
            }
            // Export
            AssetDatabase.ExportPackage(
                packagePath,
                outputPath,
                ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies
            );
        }
        private static Dictionary<string, string> ParseCommandLineArguments()
        {
            var providedArguments = new Dictionary<string, string>();
            var args = Environment.GetCommandLineArgs();
            Console.WriteLine(
                $"{Eol}" +
                $"###########################{Eol}" +
                $"#    Parsing settings     #{Eol}" +
                $"###########################{Eol}" +
                $"{Eol}"
            );
            // Extract flags with optional values
            for (int current = 0, next = 1; current < args.Length; current++, next++)
            {
                // Parse flag
                bool isFlag = args[current].StartsWith("-");
                if (!isFlag) 
                    continue;
                var flag = args[current].TrimStart('-');
                // Parse optional value
                var flagHasValue = next < args.Length && !args[next].StartsWith("-");
                var value = flagHasValue ? args[next].TrimStart('-') : "";
                if (!Needs.Contains(flag))
                    continue;
                // Assign
                Console.WriteLine($"Found flag \"{flag}\" with value \"{value}\".");
                providedArguments.Add(flag, value);
            }

            foreach (var need in Needs)
            {
                if (!providedArguments.TryGetValue(need, out var val) || string.IsNullOrEmpty(val))
                {
                    Console.WriteLine($"Missing argument -{need}");
                    EditorApplication.Exit(110);
                }
            }
            return providedArguments;
        }
    }
}
