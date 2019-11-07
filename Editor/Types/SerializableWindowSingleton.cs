#if UNITY_EDITOR

using UnityEditor;

namespace UnityExtensions.Editor
{
    /// <summary>
    /// SerializableWindowSingleton<T>
    /// </summary>
    public abstract class SerializableWindowSingleton<T> : WindowSingleton<T> where T : SerializableWindowSingleton<T>
    {
        protected void Serialize()
        {
            BeforeSerialize();
            var data = EditorJsonUtility.ToJson(this);
            EditorPrefs.SetString(typeof(T).FullName, data);
        }


        protected void Deserialize()
        {
            var data = EditorPrefs.GetString(typeof(T).FullName);
            EditorJsonUtility.FromJsonOverwrite(data, this);
            AfterDeserialize();
        }


        protected override void Initialize()
        {
            Deserialize();
        }


        protected virtual void OnLostFocus()
        {
            Serialize();
        }


        protected virtual void BeforeSerialize()
        {
        }


        protected virtual void AfterDeserialize()
        {
        }

    } // class SerializableWindowSingleton<T>

} // namespace UnityExtensions.Editor

#endif // UNITY_EDITOR