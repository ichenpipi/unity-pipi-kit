using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChenPipi.UI
{

    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class ColorStylerGroup : MonoBehaviour
    {

        [SerializeField, Tooltip("样式器列表")]
        public List<ColorStyler> StylerList = new List<ColorStyler>();

        [SerializeField, Tooltip("默认样式名称（组件 Start 时自动应用）")]
        public string DefaultStyleName;

        protected void Start()
        {
            if (!string.IsNullOrEmpty(DefaultStyleName))
            {
                ApplyStyle(DefaultStyleName);
            }
        }

        public void ApplyStyle(string name)
        {
            foreach (ColorStyler styler in StylerList)
            {
                styler.ApplyStyle(name);
            }
        }

        public List<string> GetStyleNameList()
        {
            if (StylerList.Count == 0)
            {
                return new List<string>();
            }

            List<string[]> list = new List<string[]>();
            foreach (ColorStyler styler in StylerList)
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
