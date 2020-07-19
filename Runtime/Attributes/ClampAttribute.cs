using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityExtensions.Editor;
#endif

namespace UnityExtensions
{
    /// <summary>
    /// Set minimum & maximum allowed values for int & float field
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class ClampAttribute : PropertyAttribute
    {
        /// <summary>
        /// Set minimum & maximum allowed values for int & float field
        /// </summary>
        public ClampAttribute(float min, float max)
        {
#if UNITY_EDITOR
            _min = min;
            _max = max;
#endif
        }

#if UNITY_EDITOR

        float _min;
        float _max;

        [CustomPropertyDrawer(typeof(ClampAttribute))]
        class ClampDrawer : BasePropertyDrawer<ClampAttribute>
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                switch (property.propertyType)
                {
                    case SerializedPropertyType.Float:
                        using (MixedValueScope.New(property.hasMultipleDifferentValues))
                        {
                            using (var scope = ChangeCheckScope.New())
                            {
                                float value = Mathf.Clamp(
                                    EditorGUI.FloatField(position, label, property.floatValue),
                                    attribute._min,
                                    attribute._max);

                                if (scope.changed) property.floatValue = value;
                            }
                            break;
                        }
                    case SerializedPropertyType.Integer:
                        using (MixedValueScope.New(property.hasMultipleDifferentValues))
                        {
                            using (var scope = ChangeCheckScope.New())
                            {
                                int value = Mathf.Clamp(
                                    EditorGUI.IntField(position, label, property.intValue),
                                    (int)attribute._min,
                                    (int)attribute._max);

                                if (scope.changed) property.intValue = value;
                            }
                            break;
                        }
                    default:
                        {
                            EditorGUI.LabelField(position, label.text, "Use ClampAttribute with float or int.");
                            break;
                        }
                }
            }

        } // class ClampDrawer

#endif // UNITY_EDITOR

    } // class ClampAttribute

} // namespace UnityExtensions