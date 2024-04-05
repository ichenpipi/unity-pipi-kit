using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ChenPipi.UI
{

    [CustomEditor(typeof(ColorStylerGroup))]
    [CanEditMultipleObjects]
    public class CUIColorStylerGroupEditor : Editor
    {

        private ColorStylerGroup m_TargetComp;

        private GUIContent m_DefaultStyleDropdownGUIContent;

        private GUIContent m_PreviewStyleDropdownGUIContent;

        private int m_DefaultStyleIndex = -1;

        private int m_PreviewStyleIndex = -1;

        private void OnEnable()
        {
            m_TargetComp = target as ColorStylerGroup;

            m_DefaultStyleDropdownGUIContent = new GUIContent("默认样式", "组件初始化（Start）时自动应用的样式");
            m_PreviewStyleDropdownGUIContent = new GUIContent("应用样式", "下拉选择样式");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            {
                // 样式器列表
                SerializedProperty stylerList = serializedObject.FindProperty("StylerList");
                EditorGUILayout.PropertyField(stylerList, true);
                // 默认样式
                SerializedProperty defaultStyleName = serializedObject.FindProperty("DefaultStyleName");
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
                // 应用样式
                EditorGUILayout.BeginHorizontal();
                {
                    // 选择样式
                    m_PreviewStyleIndex = DrawPreviewStyleDropdownGUI(m_PreviewStyleIndex);
                    if (GUILayout.Button("应用"))
                    {
                        m_TargetComp.ApplyStyle(GetStyleNameByIndex(m_PreviewStyleIndex));
                    }
                    // 刷新SceneView
                    if (m_TargetComp.gameObject.activeSelf)
                    {
                        m_TargetComp.gameObject.SetActive(false);
                        m_TargetComp.gameObject.SetActive(true);
                    }
                }
                EditorGUILayout.EndHorizontal();
                // 一键收集组件
                if (GUILayout.Button("收集子节点的 ColorStyler 组件"))
                {
                    m_TargetComp.StylerList.Clear();
                    foreach (ColorStyler styler in m_TargetComp.GetComponentsInChildren<ColorStyler>())
                    {
                        m_TargetComp.StylerList.Add(styler);
                    }
                }
            }
            GUILayout.Space(5);
        }

        private int GetStyleIndexByName(string name)
        {
            List<string> list = m_TargetComp.GetStyleNameList();
            for (int index = 0; index < list.Count; index++)
            {
                if (list[index].Equals(name))
                {
                    return index;
                }
            }
            return -1;
        }

        private string GetStyleNameByIndex(int index)
        {
            List<string> list = m_TargetComp.GetStyleNameList();
            if (index < 0 || index > list.Count - 1)
            {
                return string.Empty;
            }
            return list[index];
        }

        #region DropdownGUI

        private static readonly List<GUIContent> m_DropdownItemList = new List<GUIContent>();

        private int DrawDefaultStyleDropdownGUI(int selectedIndex)
        {
            m_DropdownItemList.Clear();
            m_DropdownItemList.Add(new GUIContent("无"));
            foreach (string styleName in m_TargetComp.GetStyleNameList())
            {
                if (string.IsNullOrEmpty(styleName))
                {
                    continue;
                }
                m_DropdownItemList.Add(new GUIContent(styleName));
            }
            return EditorGUILayout.Popup(m_DefaultStyleDropdownGUIContent, selectedIndex, m_DropdownItemList.ToArray());
        }

        private int DrawPreviewStyleDropdownGUI(int selectedIndex)
        {
            m_DropdownItemList.Clear();
            foreach (string styleName in m_TargetComp.GetStyleNameList())
            {
                if (string.IsNullOrEmpty(styleName))
                {
                    continue;
                }
                m_DropdownItemList.Add(new GUIContent(styleName));
            }
            return EditorGUILayout.Popup(m_PreviewStyleDropdownGUIContent, selectedIndex, m_DropdownItemList.ToArray());
        }

        #endregion

    }

}
