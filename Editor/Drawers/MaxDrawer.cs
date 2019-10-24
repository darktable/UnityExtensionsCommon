#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace UnityExtensions.Editor.AttributeDrawers
{
    [CustomPropertyDrawer(typeof(MaxAttribute))]
    class MaxDrawer : BasePropertyDrawer<MaxAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Float:
                    {
                        property.floatValue = Mathf.Min(
                            EditorGUI.FloatField(position, label, property.floatValue),
                            attribute.max);
                        break;
                    }
                case SerializedPropertyType.Integer:
                    {
                        property.intValue = Mathf.Min(
                            EditorGUI.IntField(position, label, property.intValue),
                            (int)attribute.max);
                        break;
                    }
                default:
                    {
                        EditorGUI.LabelField(position, label.text, "Use Max with float or int.");
                        break;
                    }
            }
        }

    } // class MaxDrawer

} // UnityExtensions.Editor.AttributeDrawers

#endif // UNITY_EDITOR