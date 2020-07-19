using System;
using UnityEngine;

#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityExtensions.Editor;
#endif

namespace UnityExtensions
{
    /// <summary>
    /// DisableAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class DisableAttribute : PropertyAttribute
    {
        public DisableAttribute(int indent = 0)
        {
#if UNITY_EDITOR
            _indent = indent;
            _mode = 0;
#endif
        }

        /// <summary>
        /// Use a value to enabled or disable field editing
        /// </summary>
        /// <param name="fieldOrProperty"> Name of a field or property of the same object. </param>
        /// <param name="disableValue"> the value of the field or property to disable editing. </param>
        /// <param name="indent"> Indent level of this field. </param>
        public DisableAttribute(string fieldOrProperty, bool disableValue, int indent = 0)
        {
#if UNITY_EDITOR
            _name = fieldOrProperty;
            _value = disableValue;
            _indent = indent;
            _mode = 3;
#endif
        }


#if UNITY_EDITOR

        string _name;
        bool _value;
        int _indent;
        int _mode; // -1:fieldOrPropertyError 0:default, 1:field, 2:property, 3:fieldOrProperty
        object _fieldOrProp;

        [CustomPropertyDrawer(typeof(DisableAttribute))]
        class DisableDrawer : BasePropertyDrawer<DisableAttribute>
        {
            void ValidateFieldOrPropertyInfo()
            {
                if (attribute._mode == 3)
                {
                    var field = fieldInfo.DeclaringType.GetInstanceField(attribute._name);
                    if (field?.FieldType == typeof(bool))
                    {
                        attribute._fieldOrProp = field;
                        attribute._mode = 1;
                    }
                    else
                    {
                        var prop = fieldInfo.DeclaringType.GetInstanceProperty(attribute._name);
                        if (prop?.PropertyType == typeof(bool) && prop.CanRead)
                        {
                            attribute._fieldOrProp = prop;
                            attribute._mode = 2;
                        }
                        else attribute._mode = -1;
                    }
                }
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                ValidateFieldOrPropertyInfo();

                return attribute._mode < 0 ? EditorGUIUtility.singleLineHeight : base.GetPropertyHeight(property, label);
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                if (attribute._mode < 0)
                {
                    EditorGUI.LabelField(position, label.text, "Can not find a valid field or property");
                    return;
                }

                bool disabled = true;

                if (attribute._mode > 0)
                {
                    foreach (var target in property.serializedObject.targetObjects)
                    {
                        var parent = property.GetParentObject(target);
                        if (attribute._mode == 1)
                        {
                            disabled = (bool)((FieldInfo)attribute._fieldOrProp).GetValue(parent) == attribute._value;
                        }
                        else
                        {
                            disabled = (bool)((PropertyInfo)attribute._fieldOrProp).GetValue(parent, null) == attribute._value;
                        }

                        if (disabled) break;
                    }
                }

                using (DisabledScope.New(disabled))
                {
                    using (IndentLevelScope.New(attribute._indent))
                        base.OnGUI(position, property, label);
                }
            }

        } // class DisableDrawer

#endif // UNITY_EDITOR

    } // class DisableAttribute

} // namespace UnityExtensions