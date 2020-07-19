#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace UnityExtensions.Editor
{
    /// <summary>
    /// Extensions for serialization.
    /// </summary>
    public static partial class Extensions
    {
        public static object GetObject(this SerializedProperty property)
        {
            object value = null;
            EditorGUIUtilities.GetSetSerializedPropertyRepresentedObject(
                false,
                property.serializedObject.targetObject,
                property.propertyPath,
                ref value);
            return value;
        }

        public static object GetParentObject(this SerializedProperty property)
        {
            object value = null;
            EditorGUIUtilities.GetSetSerializedPropertyRepresentedObject(
                false,
                property.serializedObject.targetObject,
                property.propertyPath,
                ref value,
                1);
            return value;
        }

        public static object GetObject(this SerializedProperty property, Object targetObject)
        {
            object value = null;
            EditorGUIUtilities.GetSetSerializedPropertyRepresentedObject(
                false,
                targetObject,
                property.propertyPath,
                ref value);
            return value;
        }

        public static object GetParentObject(this SerializedProperty property, Object targetObject)
        {
            object value = null;
            EditorGUIUtilities.GetSetSerializedPropertyRepresentedObject(
                false,
                targetObject,
                property.propertyPath,
                ref value,
                1);
            return value;
        }

        public static void SetObject(this SerializedProperty property, object value)
        {
            EditorGUIUtilities.GetSetSerializedPropertyRepresentedObject(
                true,
                property.serializedObject.targetObject,
                property.propertyPath,
                ref value);
        }

        public static void SetParentObject(this SerializedProperty property, object value)
        {
            EditorGUIUtilities.GetSetSerializedPropertyRepresentedObject(
                true,
                property.serializedObject.targetObject,
                property.propertyPath,
                ref value,
                1);
        }

        public static void SetObject(this SerializedProperty property, Object targetObject, object value)
        {
            EditorGUIUtilities.GetSetSerializedPropertyRepresentedObject(
                true,
                targetObject,
                property.propertyPath,
                ref value);
        }

        public static void SetParentObject(this SerializedProperty property, Object targetObject, object value)
        {
            EditorGUIUtilities.GetSetSerializedPropertyRepresentedObject(
                true,
                targetObject,
                property.propertyPath,
                ref value,
                1);
        }

        public static void SetMultipleObjects(this SerializedProperty property, object value)
        {
            foreach (var targetObject in property.serializedObject.targetObjects)
            {
                EditorGUIUtilities.GetSetSerializedPropertyRepresentedObject(
                    true,
                    targetObject,
                    property.propertyPath,
                    ref value);
            }
        }

        public static void SetMultipleParentObjects(this SerializedProperty property, object value)
        {
            foreach (var targetObject in property.serializedObject.targetObjects)
            {
                EditorGUIUtilities.GetSetSerializedPropertyRepresentedObject(
                    true,
                    targetObject,
                    property.propertyPath,
                    ref value,
                    1);
            }
        }

    } // class Extensions

} // namespace UnityExtensions.Editor

#endif // UNITY_EDITOR