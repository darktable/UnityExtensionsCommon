#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace UnityExtensions.Editor
{
    /// <summary>
    /// SerializableWindowSingleton<T>
    /// </summary>
    public abstract class SerializableWindowSingleton<T> : WindowSingleton<T> where T : SerializableWindowSingleton<T>
    {
        protected override void Initialize()
        {
            EditorJsonUtility.FromJsonOverwrite(Load(), this);
        }


        protected virtual void OnLostFocus()
        {
            Save(EditorJsonUtility.ToJson(this));
        }


        protected virtual void Save(string data)
        {
            EditorPrefs.SetString($"{typeof(T).FullName}@{Application.dataPath}", data);
        }


        protected virtual string Load()
        {
            return EditorPrefs.GetString($"{typeof(T).FullName}@{Application.dataPath}");
        }

    } // class SerializableWindowSingleton<T>

} // namespace UnityExtensions.Editor

#endif // UNITY_EDITOR