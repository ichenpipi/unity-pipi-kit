using UnityEngine;
using UnityEngine.UI;

namespace ChenPipi.UI
{

    /// <summary>
    /// 渐变色
    /// </summary>
    /// <author>陈皮皮</author>
    /// <version>20220926</version>
    [AddComponentMenu("UI/Effects/Gradient (2 Colors)")]
    public class Gradient2 : BaseMeshEffect
    {

        [SerializeField]
        public Color topColor = Color.white;

        [SerializeField]
        public Color bottomColor = Color.white;

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
                vertex.color *= Color.Lerp(bottomColor, topColor, t);
                vh.SetUIVertex(vertex, i);
            }
        }

    }

}
