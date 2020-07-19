using System;
using UnityEngine;

#if UNITY_EDITOR
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityExtensions.Editor;
#endif

namespace UnityExtensions
{
    /// <summary>
    /// Hide a field
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class HideAttribute : PropertyAttribute
    {
        /// <summary>
        /// Use a value to enabled or disable field editing
        /// </summary>
        /// <param name="fieldOrProperty"> Name of a field or property of the same object. </param>
        /// <param name="hideValue"> the value of the field or property to hide editing. </param>
        /// <param name="indent"> Indent level of this field. </param>
        public HideAttribute(string fieldOrProperty, bool hideValue, int indent = 0)
        {
#if UNITY_EDITOR
            _name = fieldOrProperty;
            _value = hideValue;
            _indent = indent;
#endif
        }


#if UNITY_EDITOR

        string _name;
        bool _value;
        int _indent;

        int _state;     // 0-uninitialized, 1-field, 2-property, -1-error
        object _fieldOrProp;

        [CustomPropertyDrawer(typeof(HideAttribute))]
        class HideDrawer : BasePropertyDrawer<HideAttribute>
        {
            static Dictionary<SubFieldID, bool> _hideStates = new Dictionary<SubFieldID, bool>();

            void ValidateFieldOrPropertyInfo()
            {
                if (attribute._state == 0)
                {
                    var field = fieldInfo.DeclaringType.GetInstanceField(attribute._name);
                    if (field?.FieldType == typeof(bool))
                    {
                        attribute._fieldOrProp = field;
                        attribute._state = 1;
                    }
                    else
                    {
                        var prop = fieldInfo.DeclaringType.GetInstanceProperty(attribute._name);
                        if (prop?.PropertyType == typeof(bool) && prop.CanRead)
                        {
                            attribute._fieldOrProp = prop;
                            attribute._state = 2;
                        }
                        else attribute._state = -1;
                    }
                }
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                ValidateFieldOrPropertyInfo();

                if (attribute._state < 0) return EditorGUIUtility.singleLineHeight;

                bool hide = false;

                foreach (var target in property.serializedObject.targetObjects)
                {
                    var parent = property.GetParentObject(target);
                    if (attribute._state == 1)
                    {
                        hide = (bool)((FieldInfo)attribute._fieldOrProp).GetValue(parent) == attribute._value;
                    }
                    else
                    {
                        hide = (bool)((PropertyInfo)attribute._fieldOrProp).GetValue(parent, null) == attribute._value;
                    }

                    if (hide) break;
                }

                _hideStates[new SubFieldID(property)] = hide;
                return hide ? -EditorGUIUtility.standardVerticalSpacing : base.GetPropertyHeight(property, label);
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                if (attribute._state < 0)
                {
                    EditorGUI.LabelField(position, label.text, "Can not find a valid field or property");
                    return;
                }

                if (!_hideStates[new SubFieldID(property)])
                {
                    using (IndentLevelScope.New(attribute._indent))
                        base.OnGUI(position, property, label);
                }
            }

        } // class HideDrawer

#endif

    } // class HideAttribute

} // namespace UnityExtensions