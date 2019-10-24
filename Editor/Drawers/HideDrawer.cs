#if UNITY_EDITOR

using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace UnityExtensions.Editor.AttributeDrawers
{
    [CustomPropertyDrawer(typeof(HideAttribute))]
    class HideDrawer : BasePropertyDrawer<HideAttribute>
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (attribute.fieldOrProp == null)
            {
                var field = ReflectionUtilities.GetFieldInfo(property.serializedObject.targetObject, attribute.name);
                if (field?.FieldType == typeof(bool))
                {
                    attribute.fieldOrProp = field;
                }
                else
                {
                    var prop = ReflectionUtilities.GetPropertyInfo(property.serializedObject.targetObject, attribute.name);
                    if (prop?.PropertyType == typeof(bool) && prop.CanRead)
                    {
                        attribute.fieldOrProp = prop;
                    }
                }
            }

            object result = (attribute.fieldOrProp as FieldInfo)?.GetValue(property.serializedObject.targetObject);
            if (result == null) result = (attribute.fieldOrProp as PropertyInfo)?.GetValue(property.serializedObject.targetObject, null);

            if (result != null)
            {
                if ((bool)result == attribute.value)
                {
                    attribute.result = 0;
                    return -2f;
                }
                else
                {
                    attribute.result = 1;
                    return base.GetPropertyHeight(property, label);
                }
            }
            else
            {
                attribute.result = -1;
                return EditorGUIUtility.singleLineHeight;
            }
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (attribute.result == 1)
            {
                base.OnGUI(position, property, label);
            }
            else if (attribute.result == -1)
            {
                EditorGUI.LabelField(position, label.text, "Field or Property has error!");
            }
        }

    } // class HideDrawer

} // UnityExtensions.Editor.AttributeDrawers

#endif // UNITY_EDITOR