#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace UnityExtensions.Editor.AttributeDrawers
{
    [CustomPropertyDrawer(typeof(GetSetAttribute))]
    class GetSetDrawer : BasePropertyDrawer<GetSetAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (attribute.label != null) label.text = attribute.label;

            if (attribute.target == null)
            {
                attribute.target = property.serializedObject.targetObject;
                attribute.propertyInfo = ReflectionUtilities.GetPropertyInfo(attribute.target, attribute.propertyName);
            }

            if (attribute.propertyInfo == null)
            {
                EditorGUI.LabelField(position, label.text, "Can't find property");
                return;
            }

            if (fieldInfo.FieldType != attribute.propertyInfo.PropertyType)
            {
                EditorGUI.LabelField(position, label.text, "Mismatching property type");
                return;
            }

            if (!attribute.propertyInfo.CanRead || !attribute.propertyInfo.CanWrite)
            {
                EditorGUI.LabelField(position, label.text, "Property can't read or write");
                return;
            }

            using (var scope = ChangeCheckScope.New(property.serializedObject.targetObject))
            {
                object value = attribute.propertyInfo.GetValue(attribute.target, null);

                switch (property.propertyType)
                {
                    case SerializedPropertyType.AnimationCurve:
                        {
                            value = EditorGUI.CurveField(position, label, (AnimationCurve)value);
                            break;
                        }
                    case SerializedPropertyType.Boolean:
                        {
                            value = EditorGUI.Toggle(position, label, (bool)value);
                            break;
                        }
                    case SerializedPropertyType.Bounds:
                        {
                            value = EditorGUI.BoundsField(position, label, (Bounds)value);
                            break;
                        }
                    case SerializedPropertyType.Color:
                        {
                            value = EditorGUI.ColorField(position, label, (Color)value);
                            break;
                        }
                    case SerializedPropertyType.Enum:
                        {
                            value = EditorGUI.EnumPopup(position, label, (System.Enum)value);
                            break;
                        }
                    case SerializedPropertyType.Float:
                        {
                            value = EditorGUI.FloatField(position, label, (float)value);
                            break;
                        }
                    case SerializedPropertyType.Integer:
                        {
                            value = EditorGUI.IntField(position, label, (int)value);
                            break;
                        }
                    case SerializedPropertyType.ObjectReference:
                        {
                            value = EditorGUI.ObjectField(position, label, value as Object, fieldInfo.FieldType, !EditorUtility.IsPersistent(attribute.target));
                            break;
                        }
                    case SerializedPropertyType.Rect:
                        {
                            value = EditorGUI.RectField(position, label, (Rect)value);
                            break;
                        }
                    case SerializedPropertyType.String:
                        {
                            value = EditorGUI.TextField(position, label, (string)value);
                            break;
                        }
                    case SerializedPropertyType.Vector2:
                        {
                            value = EditorGUI.Vector2Field(position, label, (Vector2)value);
                            break;
                        }
                    case SerializedPropertyType.Vector3:
                        {
                            value = EditorGUI.Vector3Field(position, label, (Vector3)value);
                            break;
                        }
                    case SerializedPropertyType.Vector4:
                        {
                            value = EditorGUI.Vector4Field(position, label.text, (Vector4)value);
                            break;
                        }
                    default:
                        {
                            EditorGUI.LabelField(position, label.text, "Type is not supported");
                            break;
                        }
                }

                if (scope.changed) attribute.propertyInfo.SetValue(attribute.target, value, null);
            }

        } // OnGUI

    } // class GetSetDrawer

} // UnityExtensions.Editor.AttributeDrawers

#endif // UNITY_EDITOR