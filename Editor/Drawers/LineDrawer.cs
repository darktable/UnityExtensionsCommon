#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace UnityExtensions.Editor.AttributeDrawers
{
    [CustomPropertyDrawer(typeof(LineAttribute))]
    class LineDrawer : BaseDecoratorDrawer<LineAttribute>
    {
        public override bool CanCacheInspectorGUI()
        {
            return false;
        }

        public override float GetHeight()
        {
            return attribute.spaceBefore + attribute.spaceAfter + attribute.height;
        }

        public override void OnGUI(Rect position)
        {
            position.y += attribute.spaceBefore;
            position.height = attribute.height;
            var color = EditorGUIUtilities.labelNormalColor;
            color.a = 0.4f;
            EditorGUI.DrawRect(position, color);
        }

    } // LineDrawer

} // UnityExtensions.Editor.AttributeDrawers

#endif // UNITY_EDITOR