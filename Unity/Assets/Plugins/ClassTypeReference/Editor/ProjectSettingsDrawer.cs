﻿namespace TypeReferences.Editor
{
    using System.Collections.Generic;
    using SolidUtilities.Editor;
    using UnityEditor;
    using UnityEngine;

    internal static class ProjectSettingsDrawer
    {
        private const string SearchbarLabel = "Search bar minimum items count";
        private const string SearchbarTooltip = "The minimum number of items in the drop-down for the search bar to appear.";

        private const string BuiltInLabel = "Use built-in names";
        private const string BuiltInTooltip = "Whether to make dropdown show built-in types by their keyword name (e.g. int) instead of the full name";

        private const string ShowAllTypesLabel = "Show all types";
        private const string ShowAllTypesTooltip =
            "Whether to show all the types existing in the project in the dropdown. When disabled, only the types the declaring class can reference directly are shown in the dropdown.";

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new SettingsProvider("Project/Packages/Type References", SettingsScope.Project)
            {
                guiHandler = OnGUI,
                keywords = GetKeywords()
            };
        }

        private static void OnGUI(string searchContext)
        {
            using var _ = EditorGUIUtilityHelper.LabelWidthBlock(220f);
            ProjectSettings.SearchbarMinItemsCount = EditorGUILayout.IntField(new GUIContent(SearchbarLabel, SearchbarTooltip), ProjectSettings.SearchbarMinItemsCount, GUILayout.MaxWidth(300f));
            ProjectSettings.UseBuiltInNames = EditorGUILayout.Toggle(new GUIContent(BuiltInLabel, BuiltInTooltip), ProjectSettings.UseBuiltInNames);
            ProjectSettings.ShowAllTypes = EditorGUILayout.Toggle(new GUIContent(ShowAllTypesLabel, ShowAllTypesTooltip), ProjectSettings.ShowAllTypes);
        }

        private static HashSet<string> GetKeywords()
        {
            var keywords = new HashSet<string>();
            keywords.AddWords(SearchbarLabel);
            keywords.AddWords(SearchbarTooltip);
            keywords.AddWords(BuiltInLabel);
            keywords.AddWords(BuiltInTooltip);
            keywords.AddWords(ShowAllTypesLabel);
            keywords.AddWords(ShowAllTypesTooltip);
            return keywords;
        }

        private static readonly char[] _separators = { ' ' };

        private static void AddWords(this HashSet<string> set, string phrase)
        {
            foreach (string word in phrase.Split(_separators))
            {
                set.Add(word);
            }
        }
    }
}