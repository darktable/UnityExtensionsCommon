using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityExtensions.Editor;
#endif

namespace UnityExtensions
{
    /// <summary>
    /// Show a bool as toggle button
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class ToggleButtonAttribute : PropertyAttribute
    {
        public ToggleButtonAttribute(string text = null, bool indent = true)
        {
#if UNITY_EDITOR
            _label = text;
            _indent = indent;
#endif
        }


        public ToggleButtonAttribute(string label, string trueText, string falseText)
        {
#if UNITY_EDITOR
            _label = label;
            _trueText = trueText;
            _falseText = falseText;
#endif
        }

#if UNITY_EDITOR

        string _label;
        string _trueText;
        string _falseText;
        bool _indent;


        [CustomPropertyDrawer(typeof(ToggleButtonAttribute))]
        class ToggleButtonDrawer : BasePropertyDrawer<ToggleButtonAttribute>
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                if (attribute._label != null) label.text = attribute._label;

                if (attribute._trueText != null && attribute._falseText != null)
                {
                    position = EditorGUI.PrefixLabel(position, label);
                    var text = property.hasMultipleDifferentValues ? "--" : (property.boolValue ? attribute._trueText : attribute._falseText);
                    if (GUI.Button(position, text, EditorStyles.miniButton))
                    {
                        property.boolValue = !property.boolValue;
                    }
                }
                else
                {
                    if (attribute._indent) position.xMin += EditorGUIUtility.labelWidth + 2;
                    using (var scope = ChangeCheckScope.New())
                    {
                        bool value = GUI.Toggle(position, property.boolValue, property.hasMultipleDifferentValues ? "--" : label.text, EditorStyles.miniButton);
                        if (scope.changed) property.boolValue = value;
                    }
                }
            }

        } // class ToggleButtonDrawer

#endif // UNITY_EDITOR

    } // class ToggleButtonAttribute

} // namespace UnityExtensions