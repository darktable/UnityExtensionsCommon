#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace UnityExtensions.Editor.AttributeDrawers
{
    [CustomPropertyDrawer(typeof(LayerAttribute))]
    class LayerDrawer : BasePropertyDrawer<LayerAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Integer)
            {
                property.intValue = EditorGUI.LayerField(position, label, property.intValue);
            }
            else EditorGUI.LabelField(position, label, "Use LayerAttribute with int.");
        }

    } // class LayerDrawer

} // UnityExtensions.Editor.AttributeDrawers

#endif // UNITY_EDITOR