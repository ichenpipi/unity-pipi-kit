using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChenPipi.PipiKit.UI
{

    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [AddComponentMenu("UI/ColorStylerGroup")]
    public class ColorStylerGroup : MonoBehaviour
    {

        [SerializeField, Tooltip("样式器列表")]
        public List<ColorStyler> stylerList = new List<ColorStyler>();

        [SerializeField, Tooltip("默认样式名称（组件 Start 时自动应用）")]
        public string defaultStyleName;

        protected void Start()
        {
            if (!string.IsNullOrEmpty(defaultStyleName))
            {
                ApplyStyle(defaultStyleName);
            }
        }

        public void ApplyStyle(string name)
        {
            foreach (ColorStyler styler in stylerList)
            {
                styler.ApplyStyle(name);
            }
        }

        public List<string> GetStyleNameList()
        {
            if (stylerList.Count == 0)
            {
                return new List<string>();
            }

            List<string[]> list = new List<string[]>();
            foreach (ColorStyler styler in stylerList)
            {
                list.Add(styler.GetStyleNames());
            }

            if (list.Count == 0)
            {
                return new List<string>();
            }

            List<string> intersection = list[0].ToList();
            for (int i = 1; i < list.Count; i++)
            {
                intersection = intersection.Intersect(list[i]).ToList();
            }
            return intersection;
        }

    }

}
