#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace ChenPipi.UI
{

    [CustomEditor(typeof(PolygonImage), true)]
    public class PolygonImageEditor : ImageEditor
    {

        private PolygonImage m_PolygonImage;

        private SerializedProperty m_Vertices;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_PolygonImage = target as PolygonImage;

            m_Vertices = serializedObject.FindProperty("m_Vertices");
            m_Vertices.isExpanded = false;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space(5);

            serializedObject.Update();

            VerticesGUI();

            serializedObject.ApplyModifiedProperties();
        }

        private void VerticesGUI()
        {
            // 顶点列表
            EditorGUILayout.PropertyField(m_Vertices, true);

            // 将顶点限制在边界内
            if (m_PolygonImage.HasVertexOutOfBounds())
            {
                if (GUILayout.Button("Clamp vertices to bounds"))
                {
                    m_PolygonImage.ClampVerticesToBounds();
                    // 刷新节点的场景视图
                    if (m_PolygonImage.gameObject.activeSelf)
                    {
                        m_PolygonImage.gameObject.SetActive(false);
                        m_PolygonImage.gameObject.SetActive(true);
                    }
                    // 这个不管用
                    // SceneView.RepaintAll();
                }
            }

            // 提示
            EditorGUILayout.HelpBox("You can visually adjust vertices through \"PolygonCollider2D\" component.", MessageType.Info);

            // 可视化调整
            PolygonCollider2D polygonCollider2D = m_PolygonImage.gameObject.GetComponent<PolygonCollider2D>();
            if (polygonCollider2D == null)
            {
                // 添加 PolygonCollider2D 组件
                if (GUILayout.Button("Add PolygonCollider2D"))
                {
                    polygonCollider2D = m_PolygonImage.gameObject.AddComponent<PolygonCollider2D>();
                    if (m_PolygonImage.vertices.Count >= 3)
                    {
                        polygonCollider2D.points = m_PolygonImage.vertices.ToArray();
                    }
                }
            }
            else
            {
                // 同步 PolygonCollider2D 的顶点
                if (GUILayout.Button("Sync Vertices from PolygonCollider2D"))
                {
                    m_PolygonImage.vertices = new List<Vector2>(polygonCollider2D.points);
                    GameObject.DestroyImmediate(polygonCollider2D);
                }
                // 移除 PolygonCollider2D 组件
                if (GUILayout.Button("Remove PolygonCollider2D"))
                {
                    GameObject.DestroyImmediate(polygonCollider2D);
                }
            }
        }

    }

}
#endif
