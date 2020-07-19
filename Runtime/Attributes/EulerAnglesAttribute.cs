using System;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityExtensions.Editor;
#endif

namespace UnityExtensions
{
    /// <summary>
    /// Used on Quaternion
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class EulerAnglesAttribute : PropertyAttribute
    {
#if UNITY_EDITOR

        [CustomPropertyDrawer(typeof(EulerAnglesAttribute))]
        class EulerAnglesDrawer : BasePropertyDrawer<EulerAnglesAttribute>
        {
            struct Value
            {
                public Quaternion quaternion;
                public Vector3 eulerAngles;
            }

            static Dictionary<SubFieldID, Value> _fieldValues = new Dictionary<SubFieldID, Value>();

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                var firstField = new SubFieldID(property.serializedObject.targetObject, property.propertyPath);
                _fieldValues.TryGetValue(firstField, out Value value);

                if (value.quaternion != property.quaternionValue)
                {
                    property.quaternionValue = value.quaternion = property.quaternionValue.normalized;
                    value.eulerAngles = value.quaternion.eulerAngles;
                    _fieldValues[firstField] = value;
                }

                using (WideModeScope.New(true))
                {
                    using (var scope = ChangeCheckScope.New())
                    {
                        using (MixedValueScope.New(property))
                        {
                            value.eulerAngles = EditorGUI.Vector3Field(position, label, value.eulerAngles);
                        }

                        if (scope.changed)
                        {
                            property.quaternionValue = value.quaternion = Quaternion.Euler(value.eulerAngles).normalized;

                            foreach (var target in property.serializedObject.targetObjects)
                            {
                                _fieldValues[new SubFieldID(target, property.propertyPath)] = value;
                            }
                        }
                    }
                }
            }

        } // class EulerAnglesDrawer

#endif // UNITY_EDITOR

    } // class EulerAnglesAttribute

} // namespace UnityExtensions