#if UNITY_EDITOR

using UnityEngine;

namespace UnityExtensions.Editor
{
    /// <summary>
    /// WindowSingleton
    /// </summary>
    public abstract class WindowSingleton<T> : BaseWindow where T : WindowSingleton<T>
    {
        static T _instance;
        public static T instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = GetWindow();
                    _instance.Initialize();
                }
                return _instance;
            }
        }


        protected virtual void Initialize()
        {
        }


        static T GetWindow()
        {
            var array = Resources.FindObjectsOfTypeAll(typeof(T));
            T window = (array.Length != 0) ? (array[0] as T) : null;
            if (!window) window = CreateInstance<T>();
            return window;
        }

    } // class WindowSingleton<T>

} // namespace UnityExtensions.Editor

#endif // UNITY_EDITOR