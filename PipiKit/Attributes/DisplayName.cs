#if UNITY_EDITOR
using System.Text.RegularExpressions;
using UnityEditor;
#endif
using UnityEngine;

namespace ChenPipi.PipiKit
{

#if UNITY_EDITOR
    public class DisplayNameAttribute : PropertyAttribute
    {

        public string Name { get; }

        /// <summary>
        /// Specify property name to display in the inspector.
        /// </summary>
        /// <param name="name">Name to display in the inspector</param>
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
            bool isListElement = Regex.IsMatch(property.displayName, "Element \\d+");
            label.text = isListElement ? property.displayName : ((DisplayNameAttribute)attribute)?.Name;
            EditorGUI.PropertyField(position, property, label);
        }

    }
#else
    public class DisplayNameAttribute : PropertyAttribute { }
#endif

}
