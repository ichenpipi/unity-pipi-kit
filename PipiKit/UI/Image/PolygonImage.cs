using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChenPipi.UI
{

    [AddComponentMenu("UI/PolygonImage", 12)]
    [ExecuteInEditMode]
    public class PolygonImage : Image
    {

        [SerializeField]
        private List<Vector2> m_Vertices = new List<Vector2>();

        public List<Vector2> vertices
        {
            get => m_Vertices;
            set
            {
                m_Vertices = value;
                SetVerticesDirty();
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            if (overrideSprite == null)
            {
                base.OnPopulateMesh(vh);
                return;
            }

            GeneratePolygonMesh(vh, m_Vertices);
        }

        protected void GeneratePolygonMesh(VertexHelper vh, List<Vector2> vertices)
        {
            Rect rect = GetPixelAdjustedRect();
            float width = rect.width,
                height = rect.height;

            // 根据中心点计算坐标偏移
            Vector2 rectPivot = rectTransform.pivot,
                vertexPivot = new Vector2(0.5f, 0.5f);
            float vertexOffsetX = (rectPivot.x - vertexPivot.x) * width,
                vertexPivotY = (rectPivot.y - vertexPivot.y) * height;

            float halfWidth = width / 2,
                halfHeight = height / 2;

            Color32 color32 = color;

            // 顶点数据
            List<UIVertex> uiVertices = new List<UIVertex>(vertices.Count);
            UIVertex tempUIVertex = new UIVertex();
            for (int i = 0; i < vertices.Count; ++i)
            {
                Vector2 vertex = vertices[i];
                // 坐标
                float x = vertex.x - vertexOffsetX,
                    y = vertex.y - vertexPivotY;
                tempUIVertex.position = new Vector3(x, y);
                // 颜色
                tempUIVertex.color = color32;
                // UV
                float u = (vertex.x + halfWidth) / width,
                    v = (vertex.y + halfHeight) / height;
                tempUIVertex.uv0 = new Vector4(u, v);

                // 填充
                uiVertices.Add(tempUIVertex);
            }

            // 顶点索引
            List<int> indices = PolygonImageUtility.EarClipping(m_Vertices);

            // 重新填充
            vh.Clear();
            vh.AddUIVertexStream(uiVertices, indices);
        }

        public void ClampVerticesToBounds()
        {
            Rect rect = GetPixelAdjustedRect();
            float xMin = rect.xMin,
                xMax = rect.xMax,
                yMin = rect.yMin,
                yMax = rect.yMax;

            for (int i = 0; i < m_Vertices.Count; ++i)
            {
                m_Vertices[i] = new Vector2(
                    Mathf.Clamp(m_Vertices[i].x, xMin, xMax),
                    Mathf.Clamp(m_Vertices[i].y, yMin, yMax)
                );
            }

            SetVerticesDirty();
        }

        public bool HasVertexOutOfBounds()
        {
            Rect rect = GetPixelAdjustedRect();
            float xMin = rect.xMin,
                xMax = rect.xMax,
                yMin = rect.yMin,
                yMax = rect.yMax;

            foreach (Vector2 vertex in m_Vertices)
            {
                if (vertex.x < xMin || vertex.x > xMax) return true;
                if (vertex.y < yMin || vertex.y > yMax) return true;
            }
            return false;
        }

    }

}
