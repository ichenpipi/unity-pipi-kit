//
// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;
// using System;
// using XLua;
// using War.Script;
// using War.Base;
//
// namespace UI.UGUIExtend
// {
//     /// <summary>
//     /// ScrollRectTable
//     /// </summary>
//     [ExecuteInEditMode]
//     [AddComponentMenu("UGUI/ScrollRectTable")]
//     [LuaCallCSharp]
//     public class ScrollRectTable : MonoBehaviour
//     {
//         #region public static
//         /// <summary>
//         /// insert data
//         /// </summary>
//         public static string DataInsertStr = @"return function(data,index,script)
//       if script.data==nil then script.data={} end
//       table.insert(script.data,index,data)
//   end";
//
//         /// <summary>
//         /// remove data from table
//         /// </summary>
//         public static string DataRemoveStr = @"return function(data,index,script)
//       table.remove(data,index)
//   end";
//         /// <summary>
//         /// remove data from table
//         /// </summary>
//         public static string DataGetLenStr = @"onGetLen = function(data)
//       return data and #data
//   end";
//
//         #endregion
//
//         #region public attribute
//         public enum Direction
//         {
//             Down,
//             Up,
//         }
//         public Direction direction = Direction.Down;
//         
//         public RectTransform moveContainer;
//         public ScrollRectItem tileItem;//the template item
// 		public GameObject emptyItem;//the empty Item
//         public LuaFunction onItemRender;//function(tileItemClone,index,dataItem)
//         public LuaFunction onItemDispose;//function(tileItemClone,index)
//         public LuaFunction onPreRender;//function(tileItemClone,index,dataItem)
//         public LuaFunction onItemRenderComplete;
//         public static LuaFunction onDataRemove;//function(data,index,ScrollRectTable)
//         public static LuaFunction onDataInsert;//function(data,index,ScrollRectTable)
//         public static LuaFunction onScrollToComplete;//function(data,index,ScrollRectTable)
//         public static System.Func<object,int> onGetLen;//function(data)
//
//         public Action<ScrollRectItem, int, LuaTable> onItemRenderCS;
//
//         public int pageSize = 5;
//         public float renderPerFrames = 1;
//
//         public int recordCount  //
//         {
//             get;
//             private set;
//         }
//
//         public int columns = 0;
//         public Vector2 padding = Vector2.zero;
//
//         public Vector3 tileSize = new Vector3(0, 0, 1);
//
//         public LuaTable data
//         {
//             get { return _data; }
//             set
//             {
//                 if (_data != null)
//                 {
//                     _data.Dispose();
//                 }
//                 _data = value;
//                 // currFirstIndex = 0;
//                 Clear();
//                 CalcBounds();
//                 CalcPage();
//             }
//         }
//         public void SetData(LuaTable data,int len)
//         {
//             if(_data != null)
//             {
//                 _data.Dispose();
//             }
//             _data = data; 
//             Clear();
//             CalcBounds();
//
//             recordCount = len; 
//             SetRangeSymbol(recordCount);
//         }
//         #endregion
//
//
//         #region private attribute
//         public LuaTable _data;//data
//         public int headIndex
//         {
//             get; ////the camera position index
//             private set;
//         }
//         public int currFirstIndex { get; private set; }//=0;//current pageBegin data index
//         public int lastHeadIndex = 0;
//
//         //所有clone项目
//         public List<ScrollRectItem> repositionTileList = new List<ScrollRectItem>();
//         //当前item的数据索引
//         public List<int> repositionTileIndexList = new List<int>();
//         //预渲染队列
//         public Queue<ScrollRectItem> preRenderList = new Queue<ScrollRectItem>();//
//         //预渲染索引
//         public Queue<int> preRepositionIntList = new Queue<int>();
//
//         public Vector3 dtmove;
//         public Vector3 beginPosition;
//         public Vector3 currPosition;
//
//         public enum ANIM_TYPE {
//             none,
//             scale,
//             rightToLeft,
//             downToUp,
//         }
//         public ANIM_TYPE anim;
//         
//         public bool mStarted = false;
//
//         public Rect itemRect { private set; get;}
//         //private Vector2 sizeDelta;
//         #endregion
//
//
//         #region public method
//         public int GetIndex(ScrollRectItem item)
//         {
//             int i = this.repositionTileList.IndexOf(item);
//             int j = -1;
//             if (i >= 0)
//             {
//                 i = repositionTileIndexList[i];
//             } 
//             return j;
//         }
//
//         public LuaTable GetDataFromIndex(int index)
//         {
//             data.Get(index + 1, out LuaTable luaData);
//             return luaData;
//         }
//         LuaEnv luaEnv
//         {
//             get
//             {
//                 return LuaManager.Instance ? LuaManager.Instance.luaEnv:null;
//             }
//         }
//         public int RemoveChild(ScrollRectItem item)
//         {
//             int i = GetIndex(item);
//             if (i >= 0)
//             {
//                 if (onDataRemove == null && luaEnv != null)
//                 {
//                     onDataRemove = luaEnv.LoadString(DataRemoveStr, "onDataRemove");
//                 }
//                 //onDataRemove.Call(this.data, i + 1, this);
//                 onDataRemove?.Action(data, i + 1, this);
//                 this.CalcPage();
//             }
//             return i;
//         }
//
//         public int InsertData(object item, int index)
//         {
//             if (index >= this.recordCount || index < 0) index = this.recordCount;
//             if (onDataInsert == null && luaEnv != null)
//             {
//                 onDataInsert = (LuaFunction)luaEnv.LoadString(DataInsertStr, "onDataInsert");
//             }
//             //if (onDataInsert != null)onDataInsert.Call(item, index + 1, this);
//             onDataInsert?.Action(item, index + 1, this);
//             this.CalcPage();
//             return index;
//         }
//
//         public int RemoveDataAt(int index)
//         {
//             if (index >= 0 && index < this.recordCount)
//             {
//                 if (onDataRemove == null && luaEnv != null)
//                 {
//                     onDataRemove = (LuaFunction)luaEnv.LoadString(DataRemoveStr, "onDataRemove");
//                 }
//                 //if (onDataRemove != null) onDataRemove.Call(data, index + 1, this);
//                 onDataRemove?.Action(data, index + 1, this);
//                 this.CalcPage();
//                 return index;
//             }
//
//             return -1;
//         }
//
//         public void Clear()
//         {
//             for(int i=0;i<repositionTileList.Count;i++)
//             {
//                 this.PreRender(repositionTileList[i],i);
//             }
//             for(int i = 0;i<repositionTileIndexList.Count;i++)
//                 repositionTileIndexList[i] = -1;
//         }
//
//         public void ItemsDispose()
//         {
//             OnItemsDispose();
//         }
//
//         public void ScrollTo(int index)
//         {
//             Vector3 currPos = moveContainer.anchoredPosition;
//             if (index < 0) index = 0;
//             if (columns == 0)
//             {
//                 float x = index * (itemRect.width+this.padding.x);
//                 currPos.x = beginPosition.x - x;
//                 currPos.y = beginPosition.y;
//                 currPos.z = beginPosition.z;
//
//             }
//             else if (columns > 0)
//             {
//                 float y = ((int)((float)index / (float)columns)) * (itemRect.height+this.padding.y);
//
//                 currPos.x = beginPosition.x;
//                 currPos.z = beginPosition.z;
//                 if (this.direction == Direction.Down)
//                     currPos.y = Math.Abs(beginPosition.y + y + this.padding.y);//pos.y=-(itemRect.height*y+ this.padding.y);
//                 else
//                     currPos.y = beginPosition.y - (y + itemRect.height + this.padding.y); //beginPosition.y + y + this.padding.y;//pos.y=(itemRect.height*y+ this.padding.y);
//             }
//
//             moveContainer.localPosition = currPos;
//             onScrollToComplete?.Action();
//         }
//
//         /// <summary>
//         /// Refresh the form give begin data Index.
//         /// </summary>
//         /// <param name='begin'>
//         /// Begin.
//         /// </param>
//         public void Refresh(int begin = -1, int end = -1)
//         {
//             if (!mStarted) return;
//             int bg = 0, ed = 0;
//             if (begin < 0)
//             {
//                 bg = 0;
//                 ed = this.pageSize;
//                 if (moveContainer != null)
//                     moveContainer.anchoredPosition = this.beginPosition;
//
//                 currFirstIndex = 0; //强行设置为0
//                 ClearPreRenderList();
//                 Scroll(0, true);
//             }
//             else
//             {
//
//                 bg = begin;
//                 if(bg<this.currFirstIndex)bg=this.currFirstIndex;
//                 if(bg>this.currFirstIndex+this.pageSize)bg=this.currFirstIndex+this.pageSize;
//                 if (end == -1) {
//                     end = bg + this.pageSize;
//                     if (end > this.recordCount) end = this.recordCount;
//                 }
//                 DoRefresh(bg, end);
//             }
//
// 			CheckShowEmpty ();
//         }
//
//         /// <summary>
//         /// Refresh the specified item's position.
//         /// </summary>
//         /// <param name='item'>
//         /// Item.
//         /// </param>
//         public void Refresh(ScrollRectItem item)
//         {
//             int i = GetIndex(item);
//             if (i >= 0)
//             {
//                 i = i + currFirstIndex;
//                 PreRefresh(i,currFirstIndex,currFirstIndex,true);
//             }
//         }
//
//         #endregion
//
//         #region private method
//
//         internal ScrollRectItem GetItemAndSetPreRender(int currIndex,int newIndex)
//         {
//             int i = repositionTileIndexList.IndexOf(currIndex);
//             if (i == -1 ) i = repositionTileIndexList.IndexOf(-1);
//             ScrollRectItem item = null;
//             if (i >= 0)
//             {
//                 repositionTileIndexList[i] = newIndex;
//                 item = repositionTileList[i];
//                 preRenderList.Enqueue(item);
//                 preRepositionIntList.Enqueue(newIndex);
//             } 
//             return item;
//         }
//
//         public void ClearPreRenderList()
//         {
//             preRenderList.Clear();
//             preRepositionIntList.Clear();
//         }
//
//         internal void CalcPage () {
//             if (this._data != null) {
//
//                 //if (onGetLen == null && luaEnv != null) onGetLen = luaEnv.LoadString(DataGetLenStr, "onGetLen");
//                 if(onGetLen==null)
//                 {
//                     luaEnv.DoString(DataGetLenStr, "Init", luaEnv.Global);
//                     onGetLen = luaEnv.Global.Get<Func<object, int>>("onGetLen");
//                 }
//                 //recordCount = this._data.Length;
//                 recordCount = onGetLen(this._data);
//
//             }
//             else {
//                 recordCount = 0;
//             }
//             SetRangeSymbol(recordCount);
//         }
//
//         
//         public void SetRangeSymbol(int itemCount) // for editor
//         {
//             if (moveContainer != null)
//             {
//                 var delt = moveContainer.sizeDelta;
//                 if (columns <= 0)
//                 {
//                     delt.x = itemCount * (itemRect.width +this.padding.x) + this.padding.x ;
//                     delt.y = itemRect.height + this.padding.y + this.padding.y;
//                 }
//                 else
//                 {
//                     delt.x = columns * (itemRect.width + this.padding.x) + this.padding.x;
//                     int y = (int)Mathf.Ceil((float)itemCount / (float)columns);
//                     if (this.direction == Direction.Down)
//                         delt.y = (itemRect.height+this.padding.y) * y + this.padding.y;
//                     else
//                         delt.y = -((itemRect.height+this.padding.y) * y + this.padding.y);
//                 }
//                 moveContainer.pivot = new Vector2 (0f, 1f);
//                 moveContainer.SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, delt.x);
//                 moveContainer.SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, delt.y);
//                 //sizeDelta = delt;
//             }
//
//         }
//
//         internal void CalcBounds()
//         {
//             if (tileItem && tileItem.rectTransform)
//             {
//                 float wi, he;
//                 RectTransform rectTrans = tileItem.rectTransform;
//                 var size = rectTrans.sizeDelta;
//
//                 if (tileSize.x <= 1 && tileSize.x > 0) {
//                     Rect rect = ((RectTransform) transform).rect;
//                     wi = rect.size.x * tileSize.x;
//                     size.x = wi;
//                     rectTrans.sizeDelta = size;
//                 } else {
//                     wi = tileSize.x <= 0 ? size.x : tileSize.x;
//                 }
//
//                 he = tileSize.y <= 0 ? size.y : tileSize.y;
//
//                 if(columns==0 ) //如果只有一排
//                 {
//                     var dtAnchor = rectTrans.anchorMax - rectTrans.anchorMin;
//                     if (1 - dtAnchor.y <= 0.001 ) // 而且是高度适配 
//                     {
//                         he = moveContainer.rect.height;//-this.padding.y;
//                         if(wi<=0) wi = moveContainer.rect.width;//-this.padding.x;
//                     }
//                 }else if(columns==1) //如果只有一列
//                 {
//                     var dtAnchor = rectTrans.anchorMax - rectTrans.anchorMin;
//                     if (1 - dtAnchor.x <= 0.001 ) // 而且是宽度适配 
//                     {
//                         wi = moveContainer.rect.width;//-this.padding.x;
//                         if(he<=0) he = moveContainer.rect.height;//-this.padding.y;
//                     }
//                 }
//                 itemRect = new Rect(0, 0, wi, he);//new Rect(0, 0, wi + padding.x, he + padding.y);
//
//             }
//         }
//
//         public void OnItemsDispose()
//         {
//             // onItemDispose
//              for(int i=0;i<repositionTileList.Count;i++)
//             {
//                 this.ItemDispose(repositionTileList[i],i);
//             }
//             repositionTileList.Clear ();
//             preRenderList.Clear ();
//             repositionTileIndexList.Clear ();
//             preRepositionIntList.Clear ();
//
//
//             if (onItemRender != null) onItemRender.Dispose();
//             this.onItemRender = null;
//
//         }
//
//         public float renderframs = 0;
//         public void RenderItem()
//         {
//             if (renderPerFrames < 1)
//             {
//                 renderframs += renderPerFrames;
//                 if (renderframs >= 1)
//                 {
//                     renderframs = 0;
//                     Render();
//                 }
//             }
//             else
//             {
//                 for (int i = 0; i < this.renderPerFrames; i++)
//                 {
//                     Render();
//                 }
//             }
//
//             onItemRenderComplete?.Action();
//         }
//
//         public void Render()
//         {
//             if (this.preRepositionIntList.Count > 0)
//             {
//                 ScrollRectItem item = preRenderList.Dequeue();
//                 int currRenderIndex = this.preRepositionIntList.Dequeue();
//                
//                 if (currRenderIndex + 1 <= recordCount)
//                 {
//                     SetPosition(item.rectTransform, currRenderIndex);
//                     //if (onItemRender != null) onItemRender.Call(item, currRenderIndex + 1, data[currRenderIndex + 1]);
//                     onItemRender?.Action(item, currRenderIndex + 1, data[currRenderIndex + 1]);
//                     onItemRenderCS?.Invoke(item, currRenderIndex + 1, (LuaTable)data[currRenderIndex + 1]);
//                 }
//             }
//         }
//          
//         public void SetPosition(RectTransform trans, int index)
//         {
//             if (trans.parent != this.transform) trans.SetParent(this.transform,false);
//             var pos = trans.localPosition;
//             if (this.columns == 0) 
// 			{
//                 pos.x = (itemRect.width + this.padding.x) * index + this.padding.x + itemRect.width * .5f;
//                 pos.y = -this.padding.y - itemRect.height * 0.5f;
//             } else {
//                 int y = index / columns;
//                 int x = index % columns;
//                 if (this.direction == Direction.Down)
//                     pos.y = -(itemRect.height+this.padding.y) * y - itemRect.height*.5f - this.padding.y;//+ itemRect.height * .5f
//                 else
//                     pos.y = (itemRect.height+this.padding.y) * y + itemRect.height*.5f + this.padding.y ;//+ itemRect.height * .5f;
//
//                 pos.x = (itemRect.width+this.padding.x)* x + this.padding.x + itemRect.width * .5f; 
//             }
//             trans.localPosition = pos;
//
//             if (anim != ANIM_TYPE.none)
//             {
//                 try
//                 {
//                     switch (anim)
//                     {
//                         case ANIM_TYPE.scale:
//                             trans.transform.localScale = Vector3.one * 0.8f;
//                             LeanTween.scale(trans.gameObject, Vector3.one, 0.2f).setEase(LeanTweenType.easeOutBack);
//                             break;
//                         case ANIM_TYPE.rightToLeft:
//                             trans.localPosition = new Vector3(pos.x - 100, pos.y, pos.z);
//                             LeanTween.moveLocalX(trans.gameObject, pos.x, 0.2f).setEase(LeanTweenType.easeOutQuart);
//                             break;
//                         case ANIM_TYPE.downToUp:
//                             trans.localPosition = new Vector3(pos.x, pos.y - 200, pos.z);
//                             LeanTween.moveLocalY(trans.gameObject, pos.y, 0.2f).setEase(LeanTweenType.easeOutQuart);
//                             break;
//                     }
//                 }
//                 catch (Exception e)
//                 {
//                     Debug.LogWarning("leanTween work error >>>>>" + e);
//                 }
//             }
//         }
//
//         public void PreRender(ScrollRectItem item, int index)
//         {
//             if (onPreRender!=null)
//             {
//                 object dataI = index + 1 <= recordCount ? data[index + 1] : null;
//                 //onPreRender.Call(item, index + 1, dataI);
//                 onPreRender?.Action(item, index + 1, dataI);
//             }
//             else
//                 onLocalPreRender(item, index + 1);
//         }
//
//         public void onLocalPreRender(ScrollRectItem item, int index)
//         {
//             // item.name="PreItem";  
//             var pos = item.transform.localPosition;
//             pos.x = -10000;
//             item.transform.localPosition = pos;
//         }
//
//         public void ItemDispose(ScrollRectItem item, int index)
//         {
//             //if(onItemDispose!=null)
//             //{
//             //    onItemDispose.Call(item, index + 1);
//             //}
//             onItemDispose?.Action(item, index + 1);
//             if (item) {
//                 GameObjectPool.CheckRef(item.gameObject);
//                 GameObject.DestroyImmediate (item.gameObject);
//                 item = null;
//             }
//         }
//
//         public void PreRefresh(int i,int newHeadIndex,int currIndex,bool force=false)
//         {
//             if (repositionTileList.Count < this.pageSize)
//             {
//                 GameObject obj = this.tileItem.gameObject;
//                 GameObject clone = (GameObject)GameObject.Instantiate(obj);
//                 ScrollRectItem cloneRefer = clone.GetComponent<ScrollRectItem>();
//                 repositionTileList.Add(cloneRefer);
//                 repositionTileIndexList.Add(-1);
//                 clone.transform.SetParent(this.transform, false);
//             }
//
//             if(!(i>=currIndex && i< currIndex+this.pageSize && !force)) //dont need refresh
//             {
//                 int moveIndex = i;
//                 if (currIndex != newHeadIndex) moveIndex = currIndex + pageSize - ( i - newHeadIndex ) - 1;
//                 ScrollRectItem tile = GetItemAndSetPreRender(moveIndex,i);//查找对应的移动索引项目，如果没找到就从-1开始查找
//                 if(tile)PreRender(tile, i);
//             }
//
//         }
//
//         public void Scroll(int newHead, bool force)
//         {
//             if (newHead < 0) newHead = 0;
//             if (newHead > this.recordCount ) newHead = recordCount;
//             var currIndex = currFirstIndex;
//             int moveStep = newHead - currIndex;
//             if(moveStep!=0 || force)
//             {
//                 for(int i = newHead;i < newHead + pageSize;i++)
//                 {
//                     PreRefresh(i,newHead,currIndex,force);
//                 }
//
//                 this.currFirstIndex = newHead;
//             }
//         }
//
//         public void DoRefresh(int begin, int end)
//         {
//             for (int i = begin; i <= end; i++)
//             {
//                 if (i >= this.currFirstIndex){
//                      PreRefresh(i,currFirstIndex,currFirstIndex,true);
//                 }
//             }
//         }
//
// 		public void CheckShowEmpty()
// 		{
// 			if(this.emptyItem != null)
// 			{
// 				if( recordCount > 0 )
// 					this.emptyItem.SetActive (false);
// 				else if( recordCount <= 0)
// 					this.emptyItem.SetActive (true);
// 			}
// 		}
//
//         #endregion
//
//
//         /// <summary>
//         /// Position the grid's contents when the script starts.
//         /// </summary>
//         /// 
//         public void Start()
//         {
//             //renderPerFrames = this.pageSize;
//
//             mStarted = true;
//             if (moveContainer == null)moveContainer = this.GetComponent<RectTransform>();
//             if(tileItem==null)tileItem = this.GetComponentInChildren<ScrollRectItem>(true);
//
//             if (moveContainer != null)
//             {
//                 Vector3 bg = moveContainer.anchoredPosition;
//                 beginPosition = new Vector3(bg.x, bg.y, bg.z);
//                 moveContainer.pivot = new Vector2(0f,1f);
//                 if(this.tileItem)PreRender(this.tileItem,0);
//             }
//
//
//             CalcBounds();
//
//             if(this.data!=null)Scroll(0, true);
//
//         }
//
//         public void Update()
//         {
//             if (moveContainer != null && data != null)
//             {
//                 currPosition = moveContainer.anchoredPosition;
//                 if (direction == Direction.Down)
//                     dtmove = beginPosition - currPosition;
//                 else
//                     dtmove = beginPosition + currPosition;
//
//                 if (columns == 0)
//                 {
//                     headIndex = (int)(dtmove.x / (itemRect.width+this.padding.x));
//                     Scroll(headIndex, false);
//                 }
//                 else if (columns > 0)
//                 {
//                     int cloumnIndex = (int)(dtmove.y / (itemRect.height + this.padding.y));
//                     headIndex = Mathf.CeilToInt((float)(cloumnIndex * this.columns) / (float)this.columns) * columns;//
//                     if (headIndex != lastHeadIndex && headIndex <= 0)
//                     {
//                         Scroll(Mathf.Abs(headIndex), false);
//                     }
//                 }
//                 lastHeadIndex = headIndex;
//             }
//         }
//
//         /// <summary>
//         /// Is it time to reposition? Do so now.
//         /// </summary>
//         public void LateUpdate()
//         {
//             if (this.preRepositionIntList.Count > 0) RenderItem();
//         }
//
//         /// <summary>
//         /// This function is called when the MonoBehaviour will be destroyed.
//         /// </summary>
//         public void OnDestroy()
//         {
//             OnItemsDispose();
//             _data?.Dispose();
//             _data = null;
//
//             onItemRender?.Dispose();
//             onItemDispose?.Dispose();
//             onItemRenderComplete?.Dispose();
//             onPreRender?.Dispose();
//             onDataRemove?.Dispose();
//             onDataInsert?.Dispose();
//
//             onItemRender = null;
//             onItemDispose = null;
//             onItemRenderComplete = null;
//             onPreRender = null;
//             onDataRemove = null;
//             onDataInsert = null;
//         }
//
//     }
// }