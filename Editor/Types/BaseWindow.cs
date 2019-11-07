#if UNITY_EDITOR

using UnityEditor;

namespace UnityExtensions.Editor
{
    /// <summary>
    /// BaseWindow
    /// </summary>
    public abstract class BaseWindow : EditorWindow
    {
        [System.NonSerialized] bool _visible;


        public bool visible => _visible;


        protected virtual void OnEnable()
        {
            _visible = true;
            Undo.undoRedoPerformed += OnUndoRedoPerformed;
        }


        protected virtual void OnDisable()
        {
            _visible = false;
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
        }


        protected virtual void OnUndoRedoPerformed()
        {
            Repaint();
        }


        public void RepaintIfVisible()
        {
            if (_visible) Repaint();
        }

    } // class BaseWindow

} // namespace UnityExtensions.Editor

#endif // UNITY_EDITOR