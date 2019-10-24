#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace UnityExtensions.Editor.AttributeDrawers
{
    [CustomPropertyDrawer(typeof(IndentAttribute))]
    class IndentDrawer : BasePropertyDrawer<IndentAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.indentLevel += attribute.indentLevel;
            base.OnGUI(position, property, label);
            EditorGUI.indentLevel -= attribute.indentLevel;
        }
    }

} // UnityExtensions.Editor.AttributeDrawers

#endif // UNITY_EDITOR