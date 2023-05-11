using System.Collections.Generic;
using UnityEngine;

namespace ChenPipi.UI
{

    public static class PolygonImageUtility
    {

        #region Ear Clipping

        public static List<int> EarClipping(List<Vector2> polygon)
        {
            if (polygon.Count < 3) return new List<int>();
            if (polygon.Count == 3) return new List<int>() { 0, 1, 2 };

            // 建立顶点索引速查表
            Dictionary<Vector2, int> indexMap = new Dictionary<Vector2, int>();
            for (int i = 0; i < polygon.Count; i++)
            {
                indexMap[polygon[i]] = i;
            }

            // 顶点索引列表
            List<int> indices = new List<int>();

            // 创建一份顶点副本
            List<Vector2> verts = new List<Vector2>(polygon);

            int index = 0;
            while (verts.Count > 3)
            {
                int count = verts.Count;
                // int prevIndex = (index - 1 + count) % count,
                //     currIndex = index % count,
                //     nextIndex = (index + 1) % count;
                int prevIndex = index % count,
                    currIndex = (index + 1) % count,
                    nextIndex = (index + 2) % count;
                Vector2 prev = verts[prevIndex],
                    curr = verts[currIndex],
                    next = verts[nextIndex];

                // 当前组合是一个凹角，不是耳朵
                Vector2 v1 = curr - prev,
                    v2 = next - curr;
                if (v1.Cross(v2) < 0)
                {
                    index = currIndex;
                    continue;
                }

                // 检查当前组合（三角形）内是否包含其他顶点
                bool hasPoint = false;
                for (int i = 0; i < count; i++)
                {
                    if (i == prevIndex || i == currIndex || i == nextIndex) continue;
                    if (IsPointInTriangle(verts[i], prev, curr, next))
                    {
                        hasPoint = true;
                        break;
                    }
                }
                // 当前组合（三角形）内包含其他顶点，不是耳朵
                if (hasPoint)
                {
                    index = currIndex;
                    continue;
                }

                // 切掉耳朵（当前组合），得到一个三角形
                indices.Add(indexMap[next]);
                indices.Add(indexMap[curr]);
                indices.Add(indexMap[prev]);

                // 移除耳朵节点
                verts.RemoveAt(currIndex);
            }

            // 最后一个三角形
            indices.Add(indexMap[verts[2]]);
            indices.Add(indexMap[verts[1]]);
            indices.Add(indexMap[verts[0]]);

            return indices;
        }

        public static bool IsPointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        {
            Vector2 ab = b - a,
                ac = c - a,
                bc = c - b,
                bd = p - b,
                ad = p - a;
            return (
                (ab.Cross(ac) >= 0 ^ ab.Cross(ad) < 0) &&
                (ab.Cross(ac) >= 0 ^ ac.Cross(ad) >= 0) &&
                (bc.Cross(ab) > 0 ^ bc.Cross(bd) >= 0)
            );
        }

        #endregion

        #region Vector2 Extension

        private static float Cross(this Vector2 self, Vector2 other)
        {
            return (float)((double)self.x * (double)other.y - (double)self.y * (double)other.x);
        }

        private static bool SameLine(this Vector2 self, Vector2 other)
        {
            return (double)Mathf.Abs(self.Cross(other)) < 9.999999747378752E-06;
        }

        private static bool SameDirection(this Vector2 self, Vector2 other)
        {
            return self.SameLine(other) && (double)Vector2.Dot(self, other) > 0.0;
        }

        #endregion

    }

}
