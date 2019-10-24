#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace UnityExtensions.Editor.AttributeDrawers
{
    [CustomPropertyDrawer(typeof(ToggleButtonAttribute))]
    class ToggleButtonDrawer : BasePropertyDrawer<ToggleButtonAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label.text = attribute.label;
            if (attribute.trueText != null && attribute.falseText != null)
            {
                position = EditorGUI.PrefixLabel(position, label);
                if (GUI.Button(position, property.boolValue ? attribute.trueText : attribute.falseText, EditorStyles.miniButton))
                {
                    property.boolValue = !property.boolValue;
                }
            }
            else
            {
                if (attribute.indent) position.xMin += EditorGUIUtility.labelWidth;
                property.boolValue = GUI.Toggle(position, property.boolValue, label, EditorStyles.miniButton);
            }
        }

    } // class ToggleButtonDrawer

} // UnityExtensions.Editor.AttributeDrawers

#endif // UNITY_EDITOR