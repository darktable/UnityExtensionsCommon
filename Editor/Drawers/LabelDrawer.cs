#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace UnityExtensions.Editor.AttributeDrawers
{
    [CustomPropertyDrawer(typeof(LabelAttribute))]
    class LabelDrawer : BasePropertyDrawer<LabelAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label.text = attribute.label;
            base.OnGUI(position, property, label);
        }
    }

} // UnityExtensions.Editor.AttributeDrawers

#endif // UNITY_EDITOR