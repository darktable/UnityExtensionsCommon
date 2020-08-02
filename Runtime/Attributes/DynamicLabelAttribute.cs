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
    /// Change the label text of a field
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class DynamicLabelAttribute : PropertyAttribute
    {
        public DynamicLabelAttribute(string labelProperty)
        {
#if UNITY_EDITOR
            _labelProperty = labelProperty;
#endif
        }

#if UNITY_EDITOR

        string _labelProperty;
        PropertyInfo _propertyInfo;

        [CustomPropertyDrawer(typeof(DynamicLabelAttribute))]
        class DynamicLabelDrawer : BasePropertyDrawer<DynamicLabelAttribute>
        {
            PropertyInfo propertyInfo
            {
                get
                {
                    if (attribute._labelProperty != null)
                    {
                        var prop = fieldInfo.DeclaringType.GetInstanceProperty(attribute._labelProperty);
                        if (prop?.PropertyType == typeof(string) && prop.CanRead)
                        {
                            attribute._propertyInfo = prop;
                        }
                        attribute._labelProperty = null;
                    }
                    return attribute._propertyInfo;
                }
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                object text = propertyInfo?.GetValue(property.GetParentObject(), null);
                if (text != null) label.text = (string)text;
                base.OnGUI(position, property, label);
            }
        }

#endif // UNITY_EDITOR

    }

} // namespace UnityExtensions