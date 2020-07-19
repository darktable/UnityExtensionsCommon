#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace UnityExtensions.Editor
{
    public class BasePropertyDrawer : PropertyDrawer
    {
        /// <summary>
        /// BasePropertyDrawer
        /// </summary>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            using (WideModeScope.New(true))
                return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (WideModeScope.New(true))
                EditorGUI.PropertyField(position, property, label, true);
        }

    } // BasePropertyDrawer

    /// <summary>
    /// BasePropertyDrawer<T>
    /// </summary>
    public class BasePropertyDrawer<T> : BasePropertyDrawer where T : PropertyAttribute
    {
        protected new T attribute => (T)base.attribute;

    } // class BasePropertyDrawer<T>

} // namespace UnityExtensions.Editor

#endif // UNITY_EDITOR