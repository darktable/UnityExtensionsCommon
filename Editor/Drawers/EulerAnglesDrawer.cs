#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace UnityExtensions.Editor.AttributeDrawers
{
    [CustomPropertyDrawer(typeof(EulerAnglesAttribute))]
    class EulerAnglesDrawer : BasePropertyDrawer<EulerAnglesAttribute>
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (EditorGUIUtility.wideMode)
            {
                return EditorGUIUtility.singleLineHeight;
            }
            else
            {
                return EditorGUIUtility.singleLineHeight * 2;
            }
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (attribute.quaternion != property.quaternionValue)
            {
                property.quaternionValue = attribute.quaternion = property.quaternionValue.normalized;
                attribute.eulerAngles = attribute.quaternion.eulerAngles;
            }

            using (var scope = ChangeCheckScope.New())
            {
                attribute.eulerAngles = EditorGUI.Vector3Field(position, label, attribute.eulerAngles);
                if (scope.changed)
                {
                    property.quaternionValue = attribute.quaternion = Quaternion.Euler(attribute.eulerAngles).normalized;
                }
            }
        }

    } // class EulerAnglesDrawer

} // UnityExtensions.Editor.AttributeDrawers

#endif // UNITY_EDITOR