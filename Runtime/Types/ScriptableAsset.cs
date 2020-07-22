using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityExtensions.Editor;
#endif

namespace UnityExtensions
{
    /// <summary>
    /// ScriptableAsset
    /// </summary>
    public class ScriptableAsset : ScriptableObject
    {

#if UNITY_EDITOR

        /// <summary>
        /// The editor can edit multiple targets.
        /// </summary>
        protected virtual void OnInspectorGUI(ScriptableEditor editor)
        {
            editor.DrawDefaultInspector();
        }


        [CustomEditor(typeof(ScriptableAsset), true)]
        [CanEditMultipleObjects]
        public class ScriptableEditor : BaseEditor<ScriptableAsset>
        {
            public override void OnInspectorGUI()
            {
                target.OnInspectorGUI(this);
            }
        }

#endif

    } // class ScriptableAsset

} // namespace UnityExtensions