using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ChenPipi.UI
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
                SerializedProperty styleList = serializedObject.FindProperty("StyleList");
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

                SerializedProperty defaultStyleName = serializedObject.FindProperty("DefaultStyleName");
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
            List<ColorStyle> list = m_TargetComp.StyleList;
            for (int index = 0; index < list.Count; index++)
            {
                ColorStyle style = list[index];
                if (style.Name.Equals(name))
                {
                    return index;
                }
            }
            return -1;
        }

        private string GetStyleNameByIndex(int index)
        {
            List<ColorStyle> list = m_TargetComp.StyleList;
            if (index < 0 || index > list.Count - 1)
            {
                return string.Empty;
            }
            return list[index].Name;
        }

        #region DropdownGUI

        private static readonly List<GUIContent> m_DropdownItemList = new List<GUIContent>();

        private int DrawDefaultStyleDropdownGUI(int selectedIndex)
        {
            m_DropdownItemList.Clear();
            m_DropdownItemList.Add(new GUIContent("无"));
            foreach (ColorStyle style in m_TargetComp.StyleList)
            {
                if (string.IsNullOrEmpty(style.Name))
                {
                    continue;
                }
                m_DropdownItemList.Add(new GUIContent(style.Name));
            }
            return EditorGUILayout.Popup(m_DefaultStyleDropdownGUIContent, selectedIndex, m_DropdownItemList.ToArray());
        }

        private int DrawPreviewStyleDropdownGUI(int selectedIndex)
        {
            m_DropdownItemList.Clear();
            foreach (ColorStyle style in m_TargetComp.StyleList)
            {
                if (string.IsNullOrEmpty(style.Name))
                {
                    continue;
                }
                m_DropdownItemList.Add(new GUIContent(style.Name));
            }
            return EditorGUILayout.Popup(m_PreviewStyleDropdownGUIContent, selectedIndex, m_DropdownItemList.ToArray());
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

            SerializedProperty name = property.FindPropertyRelative("Name");
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

                if (isNameEmpty || isNameDuplicated)
                {
                    GUIContent nameGUIContent = new GUIContent("<color=red>样式名称</color>");
                    name.stringValue = EditorGUILayout.TextField(nameGUIContent, name.stringValue);
                }
                else
                {
                    name.stringValue = EditorGUILayout.TextField("样式名称", name.stringValue);
                }

                DrawExpandableBoolProperty(property, "EnableGraphic", "GraphicColor");
                DrawExpandableBoolProperty(property, "EnableOutline", "OutlineColor");
                DrawExpandableBoolProperty(property, "EnableShadow", "ShadowColor");
                DrawExpandableBoolProperty(property, "EnableGradient", "GradientTopColor", "GradientBottomColor");

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
            foreach (ColorStyle style in colorStyler.StyleList)
            {
                if (style.Name.Equals(name))
                {
                    count++;
                }
            }
            return count > 1;
        }

        private static void ApplyStyleBySerializedProperty(ColorStyler colorStyler, SerializedProperty property)
        {
            SerializedProperty enableGraphic = property.FindPropertyRelative("EnableGraphic");
            if (enableGraphic.boolValue)
            {
                SerializedProperty graphicColor = property.FindPropertyRelative("GraphicColor");
                colorStyler.ApplyGraphicColor(graphicColor.colorValue);
            }

            SerializedProperty enableOutline = property.FindPropertyRelative("EnableOutline");
            if (enableOutline.boolValue)
            {
                SerializedProperty outlineColor = property.FindPropertyRelative("OutlineColor");
                colorStyler.ApplyOutlineColor(outlineColor.colorValue);
            }

            SerializedProperty enableShadow = property.FindPropertyRelative("EnableShadow");
            if (enableShadow.boolValue)
            {
                SerializedProperty shadowColor = property.FindPropertyRelative("ShadowColor");
                colorStyler.ApplyShadowColor(shadowColor.colorValue);
            }

            SerializedProperty enableGradient = property.FindPropertyRelative("EnableGradient");
            if (enableGradient.boolValue)
            {
                SerializedProperty gradientTopColor = property.FindPropertyRelative("GradientTopColor");
                SerializedProperty gradientBottomColor = property.FindPropertyRelative("GradientBottomColor");
                colorStyler.ApplyGradientColor(gradientTopColor.colorValue, gradientBottomColor.colorValue);
            }
        }

    }

}
