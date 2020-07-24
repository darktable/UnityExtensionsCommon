#if UNITY_EDITOR

using UnityEngine;

namespace UnityExtensions.Editor
{
    /// <summary>
    /// BaseEditor<T>
    /// </summary>
    public class BaseEditor<T> : UnityEditor.Editor where T : Object
    {
        public new T target => (T)base.target;

        public int targetCount => targets.Length;

        public bool hasMultipleTargets => targets.Length > 1;

    } // class BaseEditor<T>

} // namespace UnityExtensions.Editor

#endif // UNITY_EDITOR