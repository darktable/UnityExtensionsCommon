using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
using UnityExtensions.Editor;
#endif

namespace UnityExtensions
{
    public enum ObjectSource
    {
        All = 0,
        Scene = 1,
        Assets = 2,
    }

    /// <summary>
    /// Filter object references.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class FilterAttribute : PropertyAttribute
    {
        public FilterAttribute(ObjectSource source)
        {
#if UNITY_EDITOR
            _source = source;
#endif
        }

#if UNITY_EDITOR
        ObjectSource _source;

        [CustomPropertyDrawer(typeof(FilterAttribute))]
        class FilterDrawer : BasePropertyDrawer<FilterAttribute>
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                if (property.propertyType != SerializedPropertyType.ObjectReference)
                {
                    EditorGUI.LabelField(position, label.text, "Use Filter with Unity Object field.");
                    return;
                }

                bool allowScene = false;
                if (attribute._source != ObjectSource.Assets)
                {
                    allowScene = !EditorUtility.IsPersistent(property.serializedObject.targetObject);
                }

                var reference = property.objectReferenceValue;
                using (MixedValueScope.New(property))
                {
                    using (var scope = ChangeCheckScope.New())
                    {
                        var result = EditorGUI.ObjectField(position, label, reference, fieldInfo.FieldType, allowScene);
                        if (scope.changed)
                        {
                            if (attribute._source != ObjectSource.Scene || !EditorUtility.IsPersistent(result))
                                property.objectReferenceValue = result;
                            else
                                EditorUtility.DisplayDialog("Unacceptable Object", "You can not select an object which is outside the scene.", "OK");
                        }
                    }
                }

            } // OnGUI

        } // class FilterDrawer

#endif

    } // class FilterAttribute

} // namespace UnityExtensions