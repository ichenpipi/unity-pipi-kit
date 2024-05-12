using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ChenPipi.PipiKit.UI
{

    [CustomEditor(typeof(ColorStyler))]
    [CanEditMultipleObjects]
    public class ColorStylerEditor : Editor
    {

        private ColorStyler m_TargetComp;

        private GUIContent m_DefaultStyleDropdownGUIContent;

        private GUIContent m_PreviewStyleDropdownGUIContent;

        private int m_DefaultStyleIndex = -1;

        private int m_PreviewStyleIndex = -1;

        private void OnEnable()
        {
            m_TargetComp = target as ColorStyler;

            m_DefaultStyleDropdownGUIContent = new GUIContent("默认样式", "组件初始化（Start）时自动应用的样式");
            m_PreviewStyleDropdownGUIContent = new GUIContent("应用样式", "下拉选择样式");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            {
                SerializedProperty styleList = serializedObject.FindProperty("styleList");
                EditorGUILayout.PropertyField(styleList, true);
                // styleList.isExpanded = EditorGUILayout.Foldout(styleList.isExpanded, styleList.displayName);
                // if (styleList.isExpanded)
                // {
                //     EditorGUI.indentLevel++;
                //     for (int i = 0; i < styleList.arraySize; i++)
                //     {
                //         EditorGUILayout.PropertyField(styleList.GetArrayElementAtIndex(i));
                //     }
                //     EditorGUI.indentLevel--;
                // }

                SerializedProperty defaultStyleName = serializedObject.FindProperty("defaultStyleName");
                // EditorGUILayout.PropertyField(defaultStyleName);
                m_DefaultStyleIndex = GetStyleIndexByName(defaultStyleName.stringValue) + 1;
                EditorGUI.BeginChangeCheck();
                m_DefaultStyleIndex = DrawDefaultStyleDropdownGUI(m_DefaultStyleIndex);
                if (EditorGUI.EndChangeCheck())
                {
                    defaultStyleName.stringValue = (m_DefaultStyleIndex == 0 ? "" : GetStyleNameByIndex(m_DefaultStyleIndex - 1));
                }
            }
            serializedObject.ApplyModifiedProperties();

            // 编辑器附加功能
            GUILayout.Space(5);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    m_PreviewStyleIndex = DrawPreviewStyleDropdownGUI(m_PreviewStyleIndex);
                    if (GUILayout.Button("应用"))
                    {
                        m_TargetComp.ApplyStyle(GetStyleNameByIndex(m_PreviewStyleIndex));
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.Space(5);
        }

        private int GetStyleIndexByName(string name)
        {
            List<ColorStyle> list = m_TargetComp.styleList;
            for (int index = 0; index < list.Count; index++)
            {
                ColorStyle style = list[index];
                if (style.name.Equals(name))
                {
                    return index;
                }
            }
            return -1;
        }

        private string GetStyleNameByIndex(int index)
        {
            List<ColorStyle> list = m_TargetComp.styleList;
            if (index < 0 || index > list.Count - 1)
            {
                return string.Empty;
            }
            return list[index].name;
        }

        #region DropdownGUI

        private static readonly List<GUIContent> s_DropdownItemList = new List<GUIContent>();

        private int DrawDefaultStyleDropdownGUI(int selectedIndex)
        {
            s_DropdownItemList.Clear();
            s_DropdownItemList.Add(new GUIContent("无"));
            foreach (ColorStyle style in m_TargetComp.styleList)
            {
                if (string.IsNullOrEmpty(style.name))
                {
                    continue;
                }
                s_DropdownItemList.Add(new GUIContent(style.name));
            }
            return EditorGUILayout.Popup(m_DefaultStyleDropdownGUIContent, selectedIndex, s_DropdownItemList.ToArray());
        }

        private int DrawPreviewStyleDropdownGUI(int selectedIndex)
        {
            s_DropdownItemList.Clear();
            foreach (ColorStyle style in m_TargetComp.styleList)
            {
                if (string.IsNullOrEmpty(style.name))
                {
                    continue;
                }
                s_DropdownItemList.Add(new GUIContent(style.name));
            }
            return EditorGUILayout.Popup(m_PreviewStyleDropdownGUIContent, selectedIndex, s_DropdownItemList.ToArray());
        }

        #endregion

    }

    [CustomPropertyDrawer(typeof(ColorStyle))]
    public class ColorStylePropertyDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ColorStyler targetComp = (ColorStyler)property.serializedObject.targetObject;

            label = EditorGUI.BeginProperty(position, label, property);

            EditorStyles.foldout.richText = true;
            EditorStyles.label.richText = true;

            SerializedProperty name = property.FindPropertyRelative("name");
            bool isNameEmpty = string.IsNullOrEmpty(name.stringValue);
            bool isNameDuplicated = CheckStyleNameDuplicates(targetComp, name.stringValue);
            if (isNameEmpty || isNameDuplicated)
            {
                string errorText = (isNameEmpty ? "未设置样式名称" : "样式名称重复");
                label.text = string.Format("<color=red>{0}（{1}）</color>", label.text, errorText);
            }
            property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, label);
            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                // 名称
                string nameLabel = (isNameEmpty || isNameDuplicated ? "<color=red>样式名称</color>" : "样式名称");
                name.stringValue = EditorGUILayout.TextField(nameLabel, name.stringValue);
                // 样式
                DrawExpandableBoolProperty(property, "enableGraphic", "graphicColor");
                DrawExpandableBoolProperty(property, "enableOutline", "outlineColor");
                DrawExpandableBoolProperty(property, "enableShadow", "shadowColor");
                DrawExpandableBoolProperty(property, "enableGradient", "gradientTopColor", "gradientBottomColor");

                // 编辑器附加功能
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(30);
                    if (GUILayout.Button(string.Format("应用样式（{0}）", name.stringValue)))
                    {
                        ApplyStyleBySerializedProperty(targetComp, property);
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.Space(5);

                EditorGUI.indentLevel--;
            }

            EditorStyles.foldout.richText = false;
            EditorStyles.label.richText = false;

            EditorGUI.EndProperty();
        }

        private static void DrawExpandableBoolProperty(SerializedProperty property, string boolPropertyName, params string[] subPropertyNames)
        {
            SerializedProperty boolProperty = property.FindPropertyRelative(boolPropertyName);
            EditorGUILayout.PropertyField(boolProperty);
            if (boolProperty.boolValue)
            {
                EditorGUI.indentLevel++;
                foreach (string subPropertyName in subPropertyNames)
                {
                    SerializedProperty subProperty = property.FindPropertyRelative(subPropertyName);
                    EditorGUILayout.PropertyField(subProperty);
                }
                EditorGUI.indentLevel--;
            }
        }

        private static bool CheckStyleNameDuplicates(ColorStyler colorStyler, string name)
        {
            int count = 0;
            foreach (ColorStyle style in colorStyler.styleList)
            {
                if (style.name.Equals(name))
                {
                    count++;
                }
            }
            return count > 1;
        }

        private static void ApplyStyleBySerializedProperty(ColorStyler colorStyler, SerializedProperty property)
        {
            SerializedProperty enableGraphic = property.FindPropertyRelative("enableGraphic");
            if (enableGraphic.boolValue)
            {
                SerializedProperty graphicColor = property.FindPropertyRelative("graphicColor");
                colorStyler.ApplyGraphicColor(graphicColor.colorValue);
            }

            SerializedProperty enableOutline = property.FindPropertyRelative("enableOutline");
            if (enableOutline.boolValue)
            {
                SerializedProperty outlineColor = property.FindPropertyRelative("outlineColor");
                colorStyler.ApplyOutlineColor(outlineColor.colorValue);
            }

            SerializedProperty enableShadow = property.FindPropertyRelative("enableShadow");
            if (enableShadow.boolValue)
            {
                SerializedProperty shadowColor = property.FindPropertyRelative("shadowColor");
                colorStyler.ApplyShadowColor(shadowColor.colorValue);
            }

            SerializedProperty enableGradient = property.FindPropertyRelative("enableGradient");
            if (enableGradient.boolValue)
            {
                SerializedProperty gradientTopColor = property.FindPropertyRelative("gradientTopColor");
                SerializedProperty gradientBottomColor = property.FindPropertyRelative("gradientBottomColor");
                colorStyler.ApplyGradientColor(gradientTopColor.colorValue, gradientBottomColor.colorValue);
            }
        }

    }

}
