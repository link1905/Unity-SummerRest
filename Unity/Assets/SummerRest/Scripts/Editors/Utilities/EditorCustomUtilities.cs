using System;
using SummerRest.Utilities;
using UnityEditor;
using UnityEngine;

namespace SummerRest.Editors.Utilities
{
    public static class EditorCustomUtilities
    {
        public readonly struct HorizontalLayoutCommand : IDisposable
        {
            public Rect Rect { get; }

            public HorizontalLayoutCommand(GUIStyle style, GUILayoutOption[] options)
            {
                Rect = EditorGUILayout.BeginHorizontal(style, options);
            }

            public void Dispose()
            {
                EditorGUILayout.EndHorizontal();
            }
        }

        public static class Heights
        {
            public static float SingleLineHeight => EditorGUIUtility.singleLineHeight;
            public static float GetPropertyHeight(
                SerializedProperty property,
                GUIContent label = null,
                bool includeChildren = true)
            {
                return EditorGUI.GetPropertyHeight(property, label, includeChildren);
            }
            public static float GetGenericPropertyHeight(SerializedProperty property)
            {
                // Iterate over child properties
                var total = 0f;
                var iterator = property.Copy();
                var enterChildren = true;
                int firstDepth = iterator.depth + 1;
                while (iterator.Next(enterChildren) && iterator.depth == firstDepth)
                {
                    // Don't draw the parent property itself
                    if (SerializedProperty.EqualContents(iterator, property))
                        continue;
                    total += EditorGUI.GetPropertyHeight(iterator, true);
                    enterChildren = false;
                }
                return total;
            }
        }

        public static class LayoutOptions
        {
            public static GUILayoutOption Width(float width) => GUILayout.Width(width);
            public static GUILayoutOption MinWidth(float minWidth) => GUILayout.MinWidth(minWidth);
            public static GUILayoutOption MaxWidth(float maxWidth) => GUILayout.MaxWidth(maxWidth);
            public static GUILayoutOption Height(float height) => GUILayout.Height(height);
            public static GUILayoutOption SingleHeight = GUILayout.Height(Heights.SingleLineHeight);
            public static GUILayoutOption MinHeight(float minHeight) => GUILayout.MinHeight(minHeight);
            public static GUILayoutOption MaxHeight(float maxHeight) => GUILayout.MaxHeight(maxHeight);
            public static GUILayoutOption ExpandWidth(bool expand = true) => GUILayout.ExpandWidth(expand);
            public static GUILayoutOption ExpandHeight(bool expand = true) => GUILayout.ExpandHeight(expand);
            
        }
        
        public static GUILayoutOption Width(this string content, float space = 20f)
        {
            return Width(new GUIContent(content), space);
        }
        public static float RawWidth(this string content, float space = 20f)
        {
            return RawWidth(new GUIContent(content), space);
        }
        public static GUILayoutOption Width(this GUIContent content, float space = 20f)
        {
            return LayoutOptions.Width(content.RawWidth(space));
        }
        public static float RawWidth(this GUIContent content, float space = 20f)
        {
            var dim = GUI.skin.label.CalcSize(content);
            return dim.x + space;
        }

        public static HorizontalLayoutCommand DoHorizontalLayout(params GUILayoutOption[] options)
        {
            return new HorizontalLayoutCommand(GUIStyle.none, options);
        }

        public static HorizontalLayoutCommand DoHorizontalLayout(GUIStyle style, params GUILayoutOption[] options)
        {
            return new HorizontalLayoutCommand(style, options);
        }


        public static void DrawGenericField(Rect position, SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Enum:
                    property.enumValueIndex = EditorGUI.Popup(position, property.enumValueIndex, property.enumNames);
                    break;
                case SerializedPropertyType.ManagedReference:
                    DrawNonUnityObject(position, property);
                    break;
                default:
                    EditorGUI.PropertyField(position, property, GUIContent.none);
                    break;
            }
        }

        private static void DrawNonUnityObject(Rect position, SerializedProperty property)
        {
            EditorGUI.indentLevel++;
            // Iterate over child properties
            var iterator = property.Copy();
            var enterChildren = true;

            int firstDepth = iterator.depth + 1;
            while (iterator.Next(enterChildren) && iterator.depth == firstDepth)
            {
                // Don't draw the parent property itself
                if (SerializedProperty.EqualContents(iterator, property))
                    continue;
                EditorGUI.PropertyField(position, iterator, true);
                position.position += new Vector2(0, Heights.GetPropertyHeight(property));
                enterChildren = false;
            }

            EditorGUI.indentLevel--;
        }

        public class EditorGUIDrawHorizontalLayout : EditorGUIDrawScopeLayout<EditorGUIDrawHorizontalLayout>
        {
            public override Rect NextPosition
            {
                get
                {
                    var res = Origin;
                    res.position += new Vector2(0, Heights.SingleLineHeight);
                    return res;
                }
            }

            protected override Vector2 SetSize(float size)
            {
                if (size < 0)
                    size = Origin.width - DrawSize;
                if (size < 0)
                    size = EditorGUIUtility.currentViewWidth - CurrentRect.x;
                CurrentRect.width = size;
                return new Vector2(size + Space, 0);
            }
            public void LabelLeftField(GUIContent label, float space = 0, GUIStyle style = null)
            {
                var size = label.RawWidth(space);
                LabelField(label, size, style);
            }
            public void LabelField(GUIContent label, float size = -1, GUIStyle style = null)
            {
                LabelFieldInternal(label, size, style);
            }
            public int PopupField(string[] options, int select, float size = -1)
            {
                return PopupInternal(options, select, size);
            }
            public bool PropertyField(SerializedProperty property, float size = -1,
                GUIContent label = null, bool includeChildren = false)
            {
                return PropertyFieldInternal(property, size, label, includeChildren);
            }
            public TEnum EnumPopup<TEnum>(TEnum selected, float size = -1, GUIContent label = null, GUIStyle style = null) where TEnum : Enum
            {
                return EnumPopupInternal(size, selected, label, style);
            }
            public bool Toggle(bool value, float size = -1, GUIContent label = null, GUIStyle style = null)
            {
                return ToggleInternal(value, size, label, style);
            }
        }
        public class EditorGUIDrawVerticalLayout : EditorGUIDrawScopeLayout<EditorGUIDrawVerticalLayout>
        {
            public override Rect NextPosition => CurrentRect;
            protected override Vector2 SetSize(float size)
            {
                CurrentRect.height = size;
                return new Vector2(0f, size + Space);
            }
        }
        public abstract class EditorGUIDrawScopeLayout<T> : IDisposable where T : EditorGUIDrawScopeLayout<T>, new()
        {
            protected float Space;
            public float DrawSize { get; private set; }
            protected Rect Origin;
            protected Rect CurrentRect;
            public Rect SingleLineRect
            {
                get
                {
                    var res = CurrentRect;
                    res.height = Heights.SingleLineHeight;
                    return res;
                }
            } 
            public abstract Rect NextPosition { get; }
            protected virtual void Reset(Rect currentRect, float space = 0f)
            {
                Origin = currentRect;
                CurrentRect = currentRect;
                Space = space;
                DrawSize = default;
            }
            private static T _singleton;
            public static T Create(Rect currentRect, float space = 0f)
            {
                _singleton ??= new T();
                _singleton.Reset(currentRect, space);
                return _singleton;
            }
            public void Dispose()
            {
                DrawSize = Space = default;
                Origin = CurrentRect = default;
            }
            protected abstract Vector2 SetSize(float size);
            private TDrawRes DrawInternal<TDrawRes>(float size, Func<TDrawRes> draw)
            {
                var offset = SetSize(size);
                var res = draw.Invoke();
                CurrentRect.position += offset;
                DrawSize += offset.magnitude;
                return res;
            }
            private void DrawInternal(float size, Action draw)
            {
                DrawInternal<object>(size, () =>
                {
                    draw();
                    return null;
                });
            }

            protected bool ToggleInternal(bool value, float size, GUIContent label, GUIStyle style)
            {
                label ??= GUIContent.none;
                style ??= EditorStyles.toggle;
                return DrawInternal(size, () => EditorGUI.Toggle(SingleLineRect, label, value, style));
            }

            protected bool PropertyFieldInternal(SerializedProperty property, float size, 
                GUIContent label, bool includeChildren)
            {
                label ??= GUIContent.none;
                return DrawInternal(size, () => EditorGUI.PropertyField(CurrentRect, property, label, includeChildren));
            }
            protected TEnum EnumPopupInternal<TEnum>(float size, 
                TEnum selected, GUIContent label, GUIStyle style) where TEnum : Enum
            {
                label ??= GUIContent.none;
                style ??= EditorStyles.popup;
                return (TEnum)DrawInternal(size, () => EditorGUI.EnumPopup(SingleLineRect, label, selected, style));
            }

            protected int PopupInternal(string[] options, int select, float size)
            {
                return DrawInternal(size, () => EditorGUI.Popup(SingleLineRect, select, options));
            }
            protected void LabelFieldInternal(GUIContent label, float size, GUIStyle style)
            {                
                label ??= GUIContent.none;
                style ??= EditorStyles.label;
                DrawInternal(size, () => EditorGUI.LabelField(SingleLineRect, label, style));
            }
        }
    }
}