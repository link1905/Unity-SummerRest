using System;
using System.Collections.Generic;
using System.Reflection;
using SummerRest.Editor.TypeReference;
using SummerRest.Editor.Utilities;
using UnityEditor;
using UnityEngine;

namespace SummerRest.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(ClassTypeConstraintAttribute), true)]
    internal sealed class ClassTypeReferencePropertyDrawer : PropertyDrawer
    {
        private readonly List<Type> _cacheSelections = new();

        private List<Type> GetFilteredTypes(ClassTypeConstraintAttribute filter)
        {
            if (_cacheSelections.Count > 0)
                return _cacheSelections;

            // var assembly = ReflectionExtensions.LoadDefaultAssembly();
            // FilterTypes(assembly, filter, _cacheSelections);
            foreach (var refAssembly in ReflectionExtensions.GetAllAssemblies())
                FilterTypes(refAssembly, filter, _cacheSelections);
            _cacheSelections.Sort((a, b) => 
                string.Compare(a.FullName, b.FullName, StringComparison.Ordinal));

            return _cacheSelections;
        }

        private static void FilterTypes(Assembly assembly, ClassTypeConstraintAttribute filter, List<Type> output)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (filter != null && !filter.IsConstraintSatisfied(type))
                    continue;
                output.Add(type);
            }
        }

        #region Type Utility

        private static readonly Dictionary<string, Type> STypeMap = new();

        private static Type ResolveType(string classRef)
        {
            if (!STypeMap.TryGetValue(classRef, out var type))
            {
                type = !string.IsNullOrEmpty(classRef) ? Type.GetType(classRef) : null;
                STypeMap[classRef] = type;
            }

            return type;
        }

        private static readonly int SControlHint = typeof(ClassTypeReferencePropertyDrawer).GetHashCode();
        private static readonly GUIContent STempContent = new();

        private string DrawTypeSelectionControl(Rect position, GUIContent label, string classRef,
            ClassTypeConstraintAttribute filter)
        {
            if (label != null && label != GUIContent.none)
                position = EditorGUI.PrefixLabel(position, label);

            int controlID = GUIUtility.GetControlID(SControlHint, FocusType.Keyboard, position);

            bool triggerDropDown = false;

            switch (Event.current.GetTypeForControl(controlID))
            {
                case EventType.ExecuteCommand:
                    if (Event.current.commandName == "TypeReferenceUpdated")
                    {
                        if (_sSelectionControlID == controlID)
                        {
                            if (classRef != _sSelectedClassRef)
                            {
                                classRef = _sSelectedClassRef;
                                GUI.changed = true;
                            }

                            _sSelectionControlID = 0;
                            _sSelectedClassRef = null;
                        }
                    }

                    break;

                case EventType.MouseDown:
                    if (GUI.enabled && position.Contains(Event.current.mousePosition))
                    {
                        GUIUtility.keyboardControl = controlID;
                        triggerDropDown = true;
                        Event.current.Use();
                    }

                    break;

                case EventType.KeyDown:
                    if (GUI.enabled && GUIUtility.keyboardControl == controlID)
                    {
                        if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.Space)
                        {
                            triggerDropDown = true;
                            Event.current.Use();
                        }
                    }

                    break;

                case EventType.Repaint:
                    // Remove assembly name from content of popup control.
                    var classRefParts = classRef.Split(',');

                    STempContent.text = classRefParts[0].Trim();
                    if (STempContent.text == "")
                        STempContent.text = "(None)";
                    else if (ResolveType(classRef) == null)
                        STempContent.text += " {Missing}";

                    EditorStyles.popup.Draw(position, STempContent, controlID);
                    break;
            }

            if (triggerDropDown)
            {
                _sSelectionControlID = controlID;
                _sSelectedClassRef = classRef;

                var filteredTypes = GetFilteredTypes(filter);
                DisplayDropDown(position, filteredTypes, ResolveType(classRef), filter.Grouping);
            }

            return classRef;
        }

        private void DrawTypeSelectionControl(Rect position, SerializedProperty property, GUIContent label,
            ClassTypeConstraintAttribute filter)
        {
            bool restoreShowMixedValue = EditorGUI.showMixedValue;
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;

            property.stringValue = DrawTypeSelectionControl(position, label, property.stringValue, filter);

            EditorGUI.showMixedValue = restoreShowMixedValue;
        }

        private static void DisplayDropDown(Rect position, List<Type> types, Type selectedType,
            ClassGrouping grouping)
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("(None)"), selectedType == null, SOnSelectedTypeName, null);
            menu.AddSeparator("");

            foreach (var type in types)
            {
                string menuLabel = FormatGroupedTypeName(type, grouping);
                if (string.IsNullOrEmpty(menuLabel))
                    continue;

                var content = new GUIContent(menuLabel);
                menu.AddItem(content, type == selectedType, SOnSelectedTypeName, type);
            }

            menu.DropDown(position);
        }

        private static string FormatGroupedTypeName(Type type, ClassGrouping grouping)
        {
            string name = type.FullName;

            if (name is null)
                return null;

            switch (grouping)
            {
                default:
                case ClassGrouping.None:
                    return name;

                case ClassGrouping.ByNamespace:
                    return name.Replace('.', '/');

                case ClassGrouping.ByNamespaceFlat:
                    int lastPeriodIndex = name.LastIndexOf('.');
                    if (lastPeriodIndex != -1)
                        name = name[..lastPeriodIndex] + "/" + name[(lastPeriodIndex + 1)..];

                    return name;

                case ClassGrouping.ByAddComponentMenu:
                    var addComponentMenuAttributes = type.GetCustomAttributes(typeof(AddComponentMenu), false);
                    if (addComponentMenuAttributes.Length == 1)
                        return ((AddComponentMenu)addComponentMenuAttributes[0]).componentMenu;

                    return "Scripts/" + name.Replace('.', '/');
            }
        }

        private static int _sSelectionControlID;
        private static string _sSelectedClassRef;

        private static readonly GenericMenu.MenuFunction2 SOnSelectedTypeName = OnSelectedTypeName;

        private static void OnSelectedTypeName(object userData)
        {
            var selectedType = userData as Type;

            _sSelectedClassRef = ClassTypeReference.GetClassRef(selectedType);

            var typeReferenceUpdatedEvent = EditorGUIUtility.CommandEvent("TypeReferenceUpdated");
            EditorWindow.focusedWindow.SendEvent(typeReferenceUpdatedEvent);
        }

        #endregion

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorStyles.popup.CalcHeight(GUIContent.none, 0);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DrawTypeSelectionControl(position, property.FindPropertyRelative("classRef"), label,
                attribute as ClassTypeConstraintAttribute);
        }
    }
}