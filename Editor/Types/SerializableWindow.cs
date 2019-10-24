#if UNITY_EDITOR

using UnityEditor;

namespace UnityExtensions.Editor
{
    /// <summary>
    /// SerializableWindow
    /// </summary>
    public abstract class SerializableWindow : EditorWindow
    {
        protected abstract string uniqueName { get; }


        void OnEnable()
        {
            var data = EditorPrefs.GetString(uniqueName);
            if (!string.IsNullOrEmpty(data))
            {
                EditorJsonUtility.FromJsonOverwrite(data, this);
                AfterEnable(true);
            }
            else
            {
                AfterEnable(false);
            }
            Undo.undoRedoPerformed += OnUndoRedoPerformed;
        }


        void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
            BeforeDisable();
            var data = EditorJsonUtility.ToJson(this);
            EditorPrefs.SetString(uniqueName, data);
        }


        protected virtual void OnUndoRedoPerformed()
        {
            Repaint();
        }


        protected virtual void AfterEnable(bool deserialized)
        {
        }


        protected virtual void BeforeDisable()
        {
        }


    } // class SerializableWindow

} // namespace UnityExtensions.Editor

#endif // UNITY_EDITOR