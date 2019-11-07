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
        protected new T attribute => base.attribute as T;

    } // class BasePropertyDrawer<T>

} // namespace UnityExtensions.Editor

#endif // UNITY_EDITOR