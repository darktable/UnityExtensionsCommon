#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace UnityExtensions.Editor.AttributeDrawers
{
    [CustomPropertyDrawer(typeof(MinAttribute))]
    class MinDrawer : BasePropertyDrawer<MinAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Float:
                    {
                        property.floatValue = Mathf.Max(
                            EditorGUI.FloatField(position, label, property.floatValue),
                            attribute.min);
                        break;
                    }
                case SerializedPropertyType.Integer:
                    {
                        property.intValue = Mathf.Max(
                            EditorGUI.IntField(position, label, property.intValue),
                            (int)attribute.min);
                        break;
                    }
                default:
                    {
                        EditorGUI.LabelField(position, label.text, "Use Min with float or int.");
                        break;
                    }
            }
        }

    } // class MinDrawer

} // UnityExtensions.Editor.AttributeDrawers

#endif // UNITY_EDITOR