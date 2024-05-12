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
    [AddComponentMenu("UI/PipiKit/Effect/Gradient")]
    public class Gradient : BaseMeshEffect
    {

        [SerializeField]
        private Color m_TopColor = Color.white;

        [SerializeField]
        private Color m_BottomColor = Color.white;

        public Color topColor
        {
            get { return m_TopColor; }
            set
            {
                m_TopColor = value;
                if (graphic != null)
                {
                    graphic.SetVerticesDirty();
                }
            }
        }

        public Color bottomColor
        {
            get { return m_BottomColor; }
            set
            {
                m_BottomColor = value;
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
            // 找出顶点的最大最小 y 值
            float top = vertexes[0].position.y;
            float bottom = vertexes[0].position.y;
            for (int i = 0; i < vertCount; i++)
            {
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
            // 根据顶点的 y 值进行插值
            float height = top - bottom;
            for (int i = 0; i < vertCount; i++)
            {
                UIVertex vertex = vertexes[i];
                float t = (vertex.position.y - bottom) / height;
                vertex.color = Color.Lerp(bottomColor, topColor, t);
                vh.SetUIVertex(vertex, i);
            }
        }

    }

}
