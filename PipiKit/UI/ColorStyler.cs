using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ChenPipi.PipiKit.UI
{

    [Serializable]
    public class ColorStyle
    {

        [Tooltip("样式名称")]
        public string name;

        [Tooltip("图形组件（Text/Image/RawImage）")]
        public bool enableGraphic;

        [Tooltip("图形组件颜色")]
        public Color graphicColor;

        [Tooltip("描边（Outline）")]
        public bool enableOutline;

        [Tooltip("描边颜色")]
        public Color outlineColor;

        [Tooltip("阴影（Shadow）")]
        public bool enableShadow;

        [Tooltip("阴影颜色")]
        public Color shadowColor;

        [Tooltip("渐变（Gradient）")]
        public bool enableGradient;

        [Tooltip("渐变颜色上")]
        public Color gradientTopColor;

        [Tooltip("渐变颜色下")]
        public Color gradientBottomColor;

    }

    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [AddComponentMenu("UI/PipiKit/ColorStyler")]
    public class ColorStyler : MonoBehaviour
    {

        [SerializeField, Tooltip("样式列表")]
        public List<ColorStyle> styleList = new List<ColorStyle>();

        [SerializeField, Tooltip("默认样式名称（组件 Start 时自动应用）")]
        public string defaultStyleName;

        private readonly Dictionary<string, ColorStyle> m_StyleMap = new Dictionary<string, ColorStyle>();

        #region TargetComponents

        private MaskableGraphic m_GraphicComp;

        private MaskableGraphic graphicComp
        {
            get
            {
                if (m_GraphicComp == null)
                {
                    return m_GraphicComp = GetComponent<MaskableGraphic>();
                }
                return m_GraphicComp;
            }
        }

        private Outline m_OutlineComp;

        private Outline outlineComp
        {
            get
            {
                if (m_OutlineComp == null)
                {
                    return m_OutlineComp = GetComponent<Outline>();
                }
                return m_OutlineComp;
            }
        }

        private Shadow m_ShadowComp;

        private Shadow shadowComp
        {
            get
            {
                if (m_ShadowComp == null)
                {
                    // 处理节点同时拥有Outline和Shadow的情况
                    foreach (Shadow comp in GetComponents<Shadow>())
                    {
                        if (!(comp is Outline))
                        {
                            m_ShadowComp = comp;
                        }
                    }
                }
                return m_ShadowComp;
            }
        }

        private Gradient m_GradientComp;

        private Gradient gradientComp
        {
            get
            {
                if (m_GradientComp == null)
                {
                    return m_GradientComp = GetComponent<Gradient>();
                }
                return m_GradientComp;
            }
        }

        #endregion

        protected void Start()
        {
            if (!string.IsNullOrEmpty(defaultStyleName))
            {
                ApplyStyle(defaultStyleName);
            }
        }

        protected void OnValidate()
        {
            RefreshIndexingMap();
        }

        protected void RefreshIndexingMap()
        {
            m_StyleMap.Clear();
            foreach (ColorStyle style in styleList)
            {
                if (m_StyleMap.ContainsKey(style.name))
                {
                    Debug.LogError(string.Format("[ColorStyler] Style name '{0}' duplicated!", style.name), this);
                    continue;
                }
                m_StyleMap.Add(style.name, style);
            }
        }

        public void ApplyStyle(string name)
        {
            ColorStyle style = GetStyle(name);
            if (style == null)
            {
                Debug.LogError(string.Format("[ColorStyler] Cannot found style with name '{0}'!", name), this);
                return;
            }
            ApplyStyle(style);
        }

        public void ApplyStyleByIndex(int index)
        {
            if (index < 0 || index > styleList.Count - 1)
            {
                Debug.LogError(string.Format("[ColorStyler] Cannot found style with index '{0}'!", index), this);
                return;
            }
            ApplyStyle(styleList[index]);
        }

        public ColorStyle GetStyle(string name)
        {
            if (styleList.Count == 0)
            {
                return null;
            }

            if (m_StyleMap.Count == 0)
            {
                RefreshIndexingMap();
            }

            ColorStyle style;
            if (m_StyleMap.TryGetValue(name, out style))
            {
                return style;
            }

            return null;
        }

        public string[] GetStyleNames()
        {
            if (styleList.Count == 0)
            {
                return new string[] { };
            }

            if (m_StyleMap.Count == 0)
            {
                RefreshIndexingMap();
            }

            return m_StyleMap.Keys.ToArray();
        }

        public void ApplyStyle(ColorStyle style)
        {
            if (style.enableGraphic) ApplyGraphicColor(style.graphicColor);
            if (style.enableOutline) ApplyOutlineColor(style.outlineColor);
            if (style.enableShadow) ApplyShadowColor(style.shadowColor);
            if (style.enableGradient) ApplyGradientColor(style.gradientTopColor, style.gradientBottomColor);
        }

        #region Styling Implementation

        public void ApplyGraphicColor(Color color)
        {
            if (graphicComp == null)
            {
                Debug.LogWarning("[ColorStyler] Applying color failed, no 'MaskableGraphic' component found!", this);
                return;
            }
            m_GraphicComp.color = color;
        }

        public void ApplyOutlineColor(Color color)
        {
            if (outlineComp == null)
            {
                Debug.LogWarning("[ColorStyler] Applying color failed, no 'Outline' component found!", this);
                return;
            }
            m_OutlineComp.effectColor = color;
        }

        public void ApplyShadowColor(Color color)
        {
            if (shadowComp == null)
            {
                Debug.LogWarning("[ColorStyler] Applying color failed, no 'Shadow' component found!", this);
                return;
            }
            m_ShadowComp.effectColor = color;
        }

        public void ApplyGradientColor(Color topColor, Color bottomColor)
        {
            if (gradientComp == null)
            {
                Debug.LogWarning("[ColorStyler] Applying color failed, no 'Gradient' component found!", this);
                return;
            }
            m_GradientComp.topColor = topColor;
            m_GradientComp.bottomColor = bottomColor;
        }

        #endregion

    }

}
