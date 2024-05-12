using UnityEngine;
using UnityEngine.UI;

namespace ChenPipi.PipiKit.UI
{

    /// <summary>
    /// 渐变色
    /// </summary>
    /// <author>陈皮皮</author>
    /// <version>20220926</version>
    [DisallowMultipleComponent]
    [AddComponentMenu("UI/PipiKit/Effect/Gradient (4 Colors)")]
    public class Gradient4 : BaseMeshEffect
    {

        [SerializeField]
        private Color m_TopLeftColor = Color.white;

        [SerializeField]
        private Color m_TopRightColor = Color.white;

        [SerializeField]
        private Color m_BottomLeftColor = Color.white;

        [SerializeField]
        private Color m_BottomRightColor = Color.white;

        public Color topLeftColor
        {
            get { return m_TopLeftColor; }
            set
            {
                m_TopLeftColor = value;
                if (graphic != null)
                {
                    graphic.SetVerticesDirty();
                }
            }
        }

        public Color topRightColor
        {
            get { return m_TopRightColor; }
            set
            {
                m_TopRightColor = value;
                if (graphic != null)
                {
                    graphic.SetVerticesDirty();
                }
            }
        }

        public Color bottomLeftColor
        {
            get { return m_BottomLeftColor; }
            set
            {
                m_BottomLeftColor = value;
                if (graphic != null)
                {
                    graphic.SetVerticesDirty();
                }
            }
        }

        public Color bottomRightColor
        {
            get { return m_BottomRightColor; }
            set
            {
                m_BottomRightColor = value;
                if (graphic != null)
                {
                    graphic.SetVerticesDirty();
                }
            }
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive())
            {
                return;
            }
            // 顶点数量
            int vertCount = vh.currentVertCount;
            if (vertCount == 0)
            {
                return;
            }
            // 获取所有顶点
            UIVertex[] vertexes = new UIVertex[vertCount];
            for (int i = 0; i < vertCount; i++)
            {
                UIVertex vertex = new UIVertex();
                vh.PopulateUIVertex(ref vertex, i);
                vertexes[i] = vertex;
            }
            // 找出顶点的最大最小 xy 值
            float top = vertexes[0].position.y;
            float bottom = vertexes[0].position.y;
            float left = vertexes[0].position.x;
            float right = vertexes[0].position.x;
            for (int i = 0; i < vertCount; i++)
            {
                float x = vertexes[i].position.x;
                if (x > right)
                {
                    right = x;
                }
                else if (x < left)
                {
                    left = x;
                }
                float y = vertexes[i].position.y;
                if (y > top)
                {
                    top = y;
                }
                else if (y < bottom)
                {
                    bottom = y;
                }
            }
            // 根据顶点 xy 值进行插值
            float width = right - left;
            float height = top - bottom;
            Vector2 t = new Vector2();
            for (int i = 0; i < vertCount; i++)
            {
                UIVertex vertex = vertexes[i];
                t.x = (vertex.position.x - left) / width;
                t.y = (vertex.position.y - bottom) / height;
                vertex.color = BiLerp(topLeftColor, topRightColor, bottomLeftColor, bottomRightColor, t);
                vh.SetUIVertex(vertex, i);
            }
        }

        /// <summary>
        /// 双线性插值
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private static Color BiLerp(Color a1, Color a2, Color b1, Color b2, Vector2 t)
        {
            Color a = Color.LerpUnclamped(a1, a2, t.x);
            Color b = Color.LerpUnclamped(b1, b2, t.x);
            return Color.LerpUnclamped(b, a, t.y);
        }

    }

}
