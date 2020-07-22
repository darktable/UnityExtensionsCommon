using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityExtensions.Editor;
#endif

namespace UnityExtensions
{
    /// <summary>
    /// ScriptableComponent
    /// </summary>
    public class ScriptableComponent : MonoBehaviour
    {

#if UNITY_EDITOR

        /// <summary>
        /// The editor can edit multiple targets.
        /// </summary>
        protected virtual void OnInspectorGUI(ScriptableEditor editor)
        {
            editor.DrawDefaultInspector();
        }


        [CustomEditor(typeof(ScriptableComponent), true)]
        [CanEditMultipleObjects]
        public class ScriptableEditor : BaseEditor<ScriptableComponent>
        {
            public override void OnInspectorGUI()
            {
                target.OnInspectorGUI(this);
            }
        }

#endif

    } // class ScriptableComponent

} // namespace UnityExtensions