using UnityEditor;
using UnityEngine;

namespace PipiKit
{

    public class DisplayOnlyAttribute : PropertyAttribute
    {

    }

    [CustomPropertyDrawer(typeof(DisplayOnlyAttribute))]
    public class DisplayOnlyDrawer : PropertyDrawer
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

            if (value != null)
            {
                EditorGUI.LabelField(position, label.text, value);
            }
            else
            {
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property, label, true);
                GUI.enabled = true;
            }
        }

    }

}
