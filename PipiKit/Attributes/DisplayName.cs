using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace PipiKit
{

    public class DisplayNameAttribute : PropertyAttribute
    {

        public string Name { get; }

        /// <summary>
        /// Name to display in the Inspector.
        /// </summary>
        /// <param name="name"></param>
        public DisplayNameAttribute(string name)
        {
            Name = name;
        }

    }

    [CustomPropertyDrawer(typeof(DisplayNameAttribute))]
    public class DisplayNameDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool isElement = Regex.IsMatch(property.displayName, "Element \\d+");
            if (isElement)
            {
                label.text = property.displayName;
            }
            else
            {
                label.text = ((DisplayNameAttribute) attribute)?.Name;
            }
            EditorGUI.PropertyField(position, property, label);
        }

    }

}
