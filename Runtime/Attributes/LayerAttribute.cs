using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityExtensions.Editor;
#endif

namespace UnityExtensions
{
    /// <summary>
    /// Show the int value as layer
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class LayerAttribute : PropertyAttribute
    {
#if UNITY_EDITOR

        [CustomPropertyDrawer(typeof(LayerAttribute))]
        class LayerDrawer : BasePropertyDrawer<LayerAttribute>
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                if (property.propertyType == SerializedPropertyType.Integer)
                {
                    using (var scope = ChangeCheckScope.New())
                    {
                        using (MixedValueScope.New(property.hasMultipleDifferentValues))
                        {
                            int value = EditorGUI.LayerField(position, label, property.intValue);
                            if (scope.changed) property.intValue = value;
                        }
                    }
                }
                else EditorGUI.LabelField(position, label.text, "LayerAttribute can only be used with int.");
            }

        } // class LayerDrawer

#endif // UNITY_EDITOR

    } // LayerAttribute

} // namespace UnityExtensions