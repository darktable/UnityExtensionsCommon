#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace UnityExtensions.Editor
{
    /// <summary>
    /// BasePropertyDrawer<T>
    /// </summary>
    public class BasePropertyDrawer<T> : PropertyDrawer where T : PropertyAttribute
    {
        protected new T attribute => (T)base.attribute;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, property.hasVisibleChildren);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, property.hasVisibleChildren);
        }

    } // class BasePropertyDrawer<T>

} // namespace UnityExtensions.Editor

#endif // UNITY_EDITOR