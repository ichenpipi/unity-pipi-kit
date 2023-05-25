//  
// using UnityEngine;
// using System.Collections.Generic;
// using UnityEngine.UI;
// using XLua;
//
// namespace UI.UGUIExtend
// {
//     /// <summary>
//     /// 滚动列表
//     /// </summary>
//     [LuaCallCSharp]
//     [ExecuteInEditMode]
//     [RequireComponent(typeof(RectTransform))]
//     public class ScrollRectItem : MonoBehaviour
//     {
//
//         /// <summary>
//         /// for index
//         /// </summary>
//         
//         [HideInInspector]
//         public List<string> names = new List<string>();
//
//         public RectTransform rectTransform;
//
//         public GameObject[] refers;
//
//         public System.Action<string, object> InvokeFunc;
//         [HideInInspector]
//         public Object[] monos;
//
//         public object data;
//
//         public float fdata;
//
//         //public int idata;
//
//         //public string sdata;
//
//         // Use this for initialization
//         public void Start()
//         {
//             if (rectTransform == null)
//                 rectTransform = this.GetComponent<RectTransform>();
//
//         }
//
//         public Object Get(string n)
//         {
//             int index = names.IndexOf(n);
//             if (index == -1)
//             {
//                 if(Application.isEditor)
//                 {
//                     War.Base.GameObjectPool.PrintWarning(gameObject.name + "ScrollRectItem : not found the key [" + n + "]");
//                 }
//                 return null;
//             }
//             else
//                 return Get(index + 1);
//         }
//
//         public Object Get(int index)
//         {
//             index = index - 1;
//             if (index >= 0 && index < monos.Length)
//             {
//                 return monos[index];
//             }
//             else
//             {
//                 if (Application.isEditor)
//                 {
//                     War.Base.GameObjectPool.PrintWarning(gameObject.name + "ScrollRectItem : not found the key [" + index + "]");
//                 }
//                 return null;
//             }
//         }
//         public void Deliver(string eName)
//         {
//             if(InvokeFunc!=null)
//             {
//                 InvokeFunc(eName, this);
//             }
//         }
//         /// <summary>
//         /// monos的长度
//         /// </summary>
//         public int Length
//         {
//             get
//             {
//                 if (monos != null)
//                     return monos.Length;
//                 else
//                     return 0;
//             }
//         }
//
//         public void OnDestroy()
//         {
//             InvokeFunc = null;
//         }
//     }
// }