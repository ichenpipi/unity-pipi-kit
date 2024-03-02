using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChenPipi.UI
{

    [Serializable]
    public class ColorStyle
    {

        [Tooltip("样式名称")]
        public string Name;

        [Tooltip("图形组件（Text/Image/RawImage）")]
        public bool EnableGraphic;

        [Tooltip("图形组件颜色")]
        public Color GraphicColor = new Color(1f, 1f, 1f, 1f);

        [Tooltip("描边（Outline）")]
        public bool EnableOutline;

        [Tooltip("描边颜色")]
        public Color OutlineColor = new Color(1f, 1f, 1f, 1f);

        [Tooltip("阴影（Shadow）")]
        public bool EnableShadow;

        [Tooltip("阴影颜色")]
        public Color ShadowColor = new Color(1f, 1f, 1f, 1f);

        [Tooltip("渐变（Gradient）")]
        public bool EnableGradient;

        [Tooltip("渐变颜色上")]
        public Color GradientTopColor = new Color(1f, 1f, 1f, 1f);

        [Tooltip("渐变颜色下")]
        public Color GradientBottomColor = new Color(1f, 1f, 1f, 1f);

    }

    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class UIColorStyler : MonoBehaviour
    {

        [SerializeField, Tooltip("样式列表")]
        public List<ColorStyle> StyleList = new List<ColorStyle>();

        [SerializeField, Tooltip("默认样式名称")]
        public string DefaultStyleName;

        private readonly Dictionary<string, ColorStyle> m_StyleMap = new Dictionary<string, ColorStyle>();

        #region TargetComponents

        private MaskableGraphic m_GraphicComp;

        private MaskableGraphic GraphicComp
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

        private Outline OutlineComp
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

        private Shadow ShadowComp
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

        private Gradient GradientComp
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

        protected void Awake()
        {
            if (!string.IsNullOrEmpty(DefaultStyleName))
            {
                ApplyStyleByName(DefaultStyleName);
            }
        }

        protected void OnValidate()
        {
            RefreshIndexingMap();
        }

        protected void RefreshIndexingMap()
        {
            m_StyleMap.Clear();
            foreach (ColorStyle style in StyleList)
            {
                if (m_StyleMap.ContainsKey(style.Name))
                {
                    Debug.LogError(string.Format("[CUIColorStyler] Style name '{0}' duplicated!", style.Name), this);
                    continue;
                }
                m_StyleMap.Add(style.Name, style);
            }
        }

        public ColorStyle GetStyle(string name)
        {
            if (StyleList.Count == 0)
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

        public void ApplyStyleByName(string name)
        {
            ColorStyle style = GetStyle(name);
            if (style == null)
            {
                Debug.LogError(string.Format("[CUIColorStyler] Cannot found style with name '{0}'!", name), this);
                return;
            }
            ApplyStyle(style);
        }

        public void ApplyStyleByIndex(int index)
        {
            if (index < 0 || index > StyleList.Count - 1)
            {
                Debug.LogError(string.Format("[CUIColorStyler] Cannot found style with index '{0}'!", index), this);
                return;
            }
            ApplyStyle(StyleList[index]);
        }

        public void ApplyStyle(ColorStyle style)
        {
            if (style.EnableGraphic) ApplyGraphicColor(style.GraphicColor);
            if (style.EnableOutline) ApplyOutlineColor(style.OutlineColor);
            if (style.EnableShadow) ApplyShadowColor(style.ShadowColor);
            if (style.EnableGradient) ApplyGradientColor(style.GradientTopColor, style.GradientBottomColor);
        }

        #region ApplyStyleImplementation

        public void ApplyGraphicColor(Color color)
        {
            if (GraphicComp == null)
            {
                Debug.LogWarning("[CUIColorStyler] Applying color failed, no 'MaskableGraphic' component found!", this);
                return;
            }
            m_GraphicComp.color = color;
        }

        public void ApplyOutlineColor(Color color)
        {
            if (OutlineComp == null)
            {
                Debug.LogWarning("[CUIColorStyler] Applying color failed, no 'Outline' component found!", this);
                return;
            }
            m_OutlineComp.effectColor = color;
        }

        public void ApplyShadowColor(Color color)
        {
            if (ShadowComp == null)
            {
                Debug.LogWarning("[CUIColorStyler] Applying color failed, no 'Shadow' component found!", this);
                return;
            }
            m_ShadowComp.effectColor = color;
        }

        public void ApplyGradientColor(Color topColor, Color bottomColor)
        {
            if (GradientComp == null)
            {
                Debug.LogWarning("[CUIColorStyler] Applying color failed, no 'Gradient' component found!", this);
                return;
            }

#if UNITY_EDITOR
            if (GraphicComp)
            {
                m_GraphicComp.SetVerticesDirty();
            }
#endif
        }

        #endregion

    }

}
