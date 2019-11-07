#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace UnityExtensions.Editor.AttributeDrawers
{
    [CustomPropertyDrawer(typeof(IndentBeforeAttribute))]
    [CustomPropertyDrawer(typeof(IndentAfterAttribute))]
    class IndentBeforeAfterDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (attribute is IndentBeforeAttribute)
            {
                EditorGUI.indentLevel += (attribute as IndentBeforeAttribute).indentLevel;
            }

            base.OnGUI(position, property, label);

            if (attribute is IndentAfterAttribute)
            {
                EditorGUI.indentLevel += (attribute as IndentAfterAttribute).indentLevel;
            }
        }

    } // IndentBeforeAfterDrawer

} // UnityExtensions.Editor.AttributeDrawers

#endif // UNITY_EDITOR