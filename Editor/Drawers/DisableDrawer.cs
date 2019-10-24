#if UNITY_EDITOR

using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace UnityExtensions.Editor.AttributeDrawers
{
    [CustomPropertyDrawer(typeof(DisableAttribute))]
    class DisableDrawer : BasePropertyDrawer<DisableAttribute>
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

            return attribute.fieldOrProp == null ? EditorGUIUtility.singleLineHeight : base.GetPropertyHeight(property, label);
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            object result = (attribute.fieldOrProp as FieldInfo)?.GetValue(property.serializedObject.targetObject);
            if (result == null) result = (attribute.fieldOrProp as PropertyInfo)?.GetValue(property.serializedObject.targetObject, null);

            if (result != null)
            {
                using (DisabledScope.New((bool)result == attribute.value))
                {
                    base.OnGUI(position, property, label);
                }
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Field or Property has error!");
            }
        }

    } // class DisableDrawer

} // UnityExtensions.Editor.AttributeDrawers

#endif // UNITY_EDITOR