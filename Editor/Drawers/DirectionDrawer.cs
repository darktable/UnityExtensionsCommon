#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace UnityExtensions.Editor.AttributeDrawers
{
    [CustomPropertyDrawer(typeof(DirectionAttribute))]
    class DirectionDrawer : BasePropertyDrawer<DirectionAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var vector3 = property.vector3Value;

            if (attribute.direction != vector3)
            {
                if (vector3.magnitude < MathUtilities.OneMillionth)
                {
                    property.vector3Value = attribute.direction;
                }
                else
                {
                    property.vector3Value = attribute.direction = vector3 / vector3.magnitude * attribute.length;
                    attribute.eulerAngles = Quaternion.LookRotation(attribute.direction).eulerAngles;
                }
            }

            using (var scope = ChangeCheckScope.New())
            {
                attribute.eulerAngles = EditorGUI.Vector2Field(position, label, attribute.eulerAngles);
                if (scope.changed)
                {
                    property.vector3Value = attribute.direction = Quaternion.Euler(attribute.eulerAngles) * new Vector3(0, 0, attribute.length);
                }
            }
        }

    } // class DirectionDrawer

} // UnityExtensions.Editor.AttributeDrawers

#endif // UNITY_EDITOR