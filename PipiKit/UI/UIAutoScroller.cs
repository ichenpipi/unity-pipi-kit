using UnityEngine;
using UnityEngine.Serialization;

namespace ChenPipi.PipiKit.UI
{

    /// <summary>
    /// UI自动滚动器
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("UI/PipiKit/UIAutoScroller")]
    public class UIAutoScroller : MonoBehaviour
    {

        /// <summary>
        /// 滚动方向
        /// </summary>
        public enum ScrollDirection
        {
            LeftToRight,
            RightToLeft,
            BottomToTop,
            TopToBottom,
        }

        /// <summary>
        /// 是否开启滚动
        /// </summary>
        [Tooltip("是否开启滚动")]
        public bool isOn = true;

        /// <summary>
        /// 是否仅在内容节点超出容器节点时滚动
        /// </summary>
        [Tooltip("是否仅在内容节点超出容器节点时滚动")]
        public bool isOnlyScrollWhenContentExceedsContainer = true;

        /// <summary>
        /// 内容节点
        /// </summary>
        [Tooltip("内容节点")]
        public RectTransform content;

        /// <summary>
        /// 容器节点
        /// </summary>
        [Tooltip("容器节点")]
        public RectTransform container;

        /// <summary>
        /// 滚动方向
        /// </summary>
        [Tooltip("滚动方向")]
        public ScrollDirection direction = ScrollDirection.RightToLeft;

        /// <summary>
        /// 滚动速度（像素/秒）
        /// </summary>
        [Tooltip("滚动速度（像素/秒）")]
        public float speed = 80;

        protected void Awake()
        {
            this.CheckProperties();
        }

        protected void Reset()
        {
            if (this.transform.childCount == 1)
            {
                this.content = this.transform.GetChild(0) as RectTransform;
            }
            this.container = this.transform.GetComponent<RectTransform>();
        }

        protected void OnValidate()
        {
            if (this.speed < 0) this.speed = 0;
        }

        protected void Update()
        {
            if (this.content == null || this.container == null || !this.isOn || this.speed == 0)
            {
                return;
            }
            if (this.isOnlyScrollWhenContentExceedsContainer && !this.CheckIsContentExceedsContainer())
            {
                return;
            }
            this.Move();
        }

        /// <summary>
        /// 开始滚动
        /// </summary>
        /// <param name="onlyScrollWhenContentExceedsContainer">是否仅在内容节点超出容器节点时滚动</param>
        public void StartScrolling(bool onlyScrollWhenContentExceedsContainer = true)
        {
            this.isOn = true;
            this.isOnlyScrollWhenContentExceedsContainer = onlyScrollWhenContentExceedsContainer;
        }

        /// <summary>
        /// 停止滚动
        /// </summary>
        public void StopScrolling()
        {
            this.isOn = false;
        }

        /// <summary>
        /// 检查内容节点是否超出容器节点
        /// </summary>
        /// <returns></returns>
        public bool CheckIsContentExceedsContainer()
        {
            if (this.content == null || this.container == null)
            {
                return false;
            }
            Rect containerRect = this.container.rect;
            Rect contentRect = this.content.rect;
            if (this.direction == ScrollDirection.LeftToRight || this.direction == ScrollDirection.RightToLeft)
            {
                if (contentRect.width > containerRect.width)
                {
                    return true;
                }
            }
            else
            {
                if (contentRect.height > containerRect.height)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 移动
        /// </summary>
        protected void Move()
        {
            Rect containerRect = this.container.rect;
            Rect contentRect = this.content.rect;

            Vector2 contentAnchorMin = this.content.anchorMin;
            Vector2 contentPivot = this.content.pivot;

            Vector2 contentPos = this.content.anchoredPosition;

            float movement = this.speed * Time.deltaTime;
            if (this.direction == ScrollDirection.LeftToRight || this.direction == ScrollDirection.RightToLeft)
            {
                // 水平方向
                float containerWidth = containerRect.width;
                float contentWidth = contentRect.width;
                float contentAnchorMinX = contentAnchorMin.x;
                float contentPivotX = contentPivot.x;
                float minX = -(containerWidth * contentAnchorMinX) - (contentWidth * (1 - contentPivotX));
                float maxX = (containerWidth * (1 - contentAnchorMinX)) + (contentWidth * contentPivotX);
                if (this.direction == ScrollDirection.LeftToRight)
                {
                    // 向右
                    if (contentPos.x > maxX)
                    {
                        contentPos.x = minX;
                    }
                    else
                    {
                        contentPos.x += movement;
                    }
                }
                else
                {
                    // 向左
                    if (contentPos.x < minX)
                    {
                        contentPos.x = maxX;
                    }
                    else
                    {
                        contentPos.x -= movement;
                    }
                }
            }
            else
            {
                // 垂直方向
                float containerHeight = containerRect.height;
                float contentHeight = contentRect.height;
                float contentAnchorMinY = contentAnchorMin.y;
                float contentPivotY = contentPivot.y;
                float minY = -(containerHeight * contentAnchorMinY) - (contentHeight * (1 - contentPivotY));
                float maxY = (containerHeight * (1 - contentAnchorMinY)) + (contentHeight * contentPivotY);
                if (this.direction == ScrollDirection.BottomToTop)
                {
                    // 向上
                    if (contentPos.y > maxY)
                    {
                        contentPos.y = minY;
                    }
                    else
                    {
                        contentPos.y += movement;
                    }
                }
                else
                {
                    // 向下
                    if (contentPos.y < minY)
                    {
                        contentPos.y = maxY;
                    }
                    else
                    {
                        contentPos.y -= movement;
                    }
                }
            }

            this.content.anchoredPosition = contentPos;
        }

        protected bool CheckProperties()
        {
            Debug.Assert(this.content != null, "[CUIAutoScroller] Content 节点为空！", this);
            Debug.Assert(this.container != null, "[CUIAutoScroller] Container 节点为空！", this);
            return (this.content == null || this.container == null);
        }

        #region Reset Content

        public void ResetContentToLeft(float offset = 0)
        {
            if (this.CheckProperties())
            {
                return;
            }

            RectTransform container = this.container;
            RectTransform content = this.content;

            Vector2 contentPos = content.anchoredPosition;

            contentPos.x = -((container.rect.width * content.anchorMin.x) - (content.rect.width * content.pivot.x));
            contentPos.x += offset;

            content.anchoredPosition = contentPos;
        }

        public void ResetContentToRight(float offset = 0)
        {
            if (this.CheckProperties())
            {
                return;
            }

            RectTransform container = this.container;
            RectTransform content = this.content;

            Vector2 contentPos = content.anchoredPosition;

            contentPos.x = ((container.rect.width * (1 - content.anchorMin.x)) - (content.rect.width * (1 - content.pivot.x)));
            contentPos.x += offset;

            content.anchoredPosition = contentPos;
        }

        public void ResetContentToBottom(float offset = 0)
        {
            if (this.CheckProperties())
            {
                return;
            }

            RectTransform container = this.container;
            RectTransform content = this.content;

            Vector2 contentPos = content.anchoredPosition;

            contentPos.y = -((container.rect.height * content.anchorMin.y) - (content.rect.height * content.pivot.y));
            contentPos.y += offset;

            content.anchoredPosition = contentPos;
        }

        public void ResetContentToTop(float offset = 0)
        {
            if (this.CheckProperties())
            {
                return;
            }

            RectTransform container = this.container;
            RectTransform content = this.content;

            Vector2 contentPos = content.anchoredPosition;

            contentPos.y = ((container.rect.height * (1 - content.anchorMin.y)) - (content.rect.height * (1 - content.pivot.y)));
            contentPos.y += offset;

            content.anchoredPosition = contentPos;
        }

        #endregion

    }

}
