#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace UnityExtensions.Editor.AttributeDrawers
{
    [CustomPropertyDrawer(typeof(MinMaxAttribute))]
    class MinMaxDrawer : BasePropertyDrawer<MinMaxAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Float:
                    {
                        property.floatValue = Mathf.Clamp(
                            EditorGUI.FloatField(position, label, property.floatValue),
                            attribute.min,
                            attribute.max);
                        break;
                    }
                case SerializedPropertyType.Integer:
                    {
                        property.intValue = Mathf.Clamp(
                            EditorGUI.IntField(position, label, property.intValue),
                            (int)attribute.min,
                            (int)attribute.max);
                        break;
                    }
                default:
                    {
                        EditorGUI.LabelField(position, label.text, "Use MinMax with float or int.");
                        break;
                    }
            }
        }

    } // class MinMaxDrawer

} // UnityExtensions.Editor.AttributeDrawers

#endif // UNITY_EDITOR