using UnityEditor;
using UnityEngine;

namespace PipiKit
{

    public class ReadOnlyAttribute : PropertyAttribute { }

    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string value;
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    value = property.intValue.ToString();
                    break;
                case SerializedPropertyType.Boolean:
                    value = property.boolValue.ToString();
                    break;
                case SerializedPropertyType.Float:
                    value = property.floatValue.ToString("0.00000");
                    break;
                case SerializedPropertyType.String:
                    value = property.stringValue;
                    break;
                case SerializedPropertyType.Enum:
                    value = property.enumNames[property.enumValueIndex];
                    break;
                default:
                    value = null;
                    break;
            }

            EditorGUI.BeginDisabledGroup(true);
            {
                if (value != null)
                {
                    // EditorGUI.LabelField(position, label.text, value);
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField(label, GUILayout.Width(EditorGUIUtility.labelWidth));
                        EditorGUILayout.SelectableLabel(value, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    }
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUI.PropertyField(position, property, label, true);
                }
            }
            EditorGUI.EndDisabledGroup();
        }

    }

}
