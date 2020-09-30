using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityExtensions
{
    interface ISerializableInterface { }

    [System.Serializable]
    public struct SerializableInterface<T> : ISerializableInterface where T : class
    {
        public Object reference;

        public T item
        {
            get => (T)(object)reference;
            set => reference = (Object)(object)value;
        }

        public static implicit operator T(SerializableInterface<T> a)
            => (T)(object)a.reference;

        public static implicit operator SerializableInterface<T>(T a)
            => new SerializableInterface<T> { reference = (Object)(object)a };
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(ISerializableInterface), true)]
    class SerializableInterfaceDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property = property.FindPropertyRelative("reference");

            EditorGUI.BeginChangeCheck();

            var result = EditorGUI.ObjectField(
                position,
                label,
                property.objectReferenceValue,
                typeof(Object),
                true);

            if (EditorGUI.EndChangeCheck())
            {
                System.Type type = fieldInfo.FieldType;
                if (type.IsArray)
                {
                    type = type.GetElementType();
                }
                else if (type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    type = type.GenericTypeArguments[0];
                }
                
                type = type.GenericTypeArguments[0];

                if (result)
                {
                    if (result is GameObject go)
                    {
                        result = go.GetComponent(type);
                        if (result) property.objectReferenceValue = result;
                    }
                    else if (result is Component cp)
                    {
                        result = cp.GetComponent(type);
                        if (result) property.objectReferenceValue = result;
                    }
                    else if (type.IsAssignableFrom(result.GetType()))
                    {
                        property.objectReferenceValue = result;
                    }
                }
                else property.objectReferenceValue = null;
            }
        }
    }

#endif
}