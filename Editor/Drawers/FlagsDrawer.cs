#if UNITY_EDITOR

using System;
using UnityEngine;
using UnityEditor;

namespace UnityExtensions.Editor.AttributeDrawers
{
    [CustomPropertyDrawer(typeof(FlagsAttribute))]
    class FlagsDrawer : BasePropertyDrawer<FlagsAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var value = (Enum)Enum.ToObject(fieldInfo.FieldType, property.intValue);

            using (var scope = ChangeCheckScope.New())
            {
                value = EditorGUI.EnumFlagsField(position, label, value, attribute.includeObsolete);
                if (scope.changed)
                {
                    property.intValue = value.GetHashCode();
                }
            }
        }

    } // class FlagsDrawer

} // UnityExtensions.Editor.AttributeDrawers

#endif // UNITY_EDITOR