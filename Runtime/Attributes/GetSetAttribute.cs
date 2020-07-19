using UnityEngine;

#if UNITY_EDITOR
using System;
using System.Reflection;
using System.Collections.Generic;
using UnityObject = UnityEngine.Object;
using UnityEditor;
using UnityExtensions.Editor;
#endif

namespace UnityExtensions
{
    /// <summary>
    /// A param to control the stype of GetSet GUI
    /// </summary>
    [Flags]
    public enum GetSetMode
    {
        Default = 0,

        BoolToggleLeft = 1,

        IntLayer = 1,

        EnumFlags = 1,

        ColorAlphaFlag = 1 << 0,
        ColorHDRFlag = 1 << 1,

        StringArea = 1,
        StringTag = 2,
        StringPassword = 3,

        GradientHDRFlag = 1 << 1,

        ObjectSceneOnly = 1,
        ObjectAssetOnly = 2,
    }


    /// <summary>
    /// If you want to use GetSetAttribute on your custom type, you can implement this interface
    /// </summary>
    public interface ICustomDrawer
    {
#if UNITY_EDITOR

        float GetHeight(bool isExpanded);

        /// <summary>
        /// You should call Undo.RecordObjects before changing any fields.
        /// </summary>
        /// <returns> new expanding state </returns>
        bool OnGUI(Rect position, bool isExpanded, GUIContent label, UnityObject[] objects);
#endif
    }


    /// <summary>
    /// Use a property instead of field.
    ///     The property must belong to the same object;
    ///     The property must can read;
    ///     The property can only change the fields which serialized in the current Unity Object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class GetSetAttribute : PropertyAttribute
    {
        public GetSetAttribute(string propertyName, string label = null, GetSetMode mode = GetSetMode.Default)
        {
#if UNITY_EDITOR
            _propertyName = propertyName;
            _label = label;
            _mode = mode;
#endif
        }

#if UNITY_EDITOR

        string _propertyName;
        string _label;
        GetSetMode _mode;
        PropertyInfo _propertyInfo;

        [CustomPropertyDrawer(typeof(GetSetAttribute))]
        class GetSetDrawer : BasePropertyDrawer<GetSetAttribute>
        {
            static Dictionary<SubFieldID, (object parent, object property)> _propertyValues = new Dictionary<SubFieldID, (object, object)>();

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                if (attribute._propertyName != null)
                {
                    attribute._propertyInfo = fieldInfo.DeclaringType.GetInstanceProperty(attribute._propertyName);
                    attribute._propertyName = null;
                }

                if (attribute._propertyInfo == null || !attribute._propertyInfo.CanRead)
                {
                    return EditorGUIUtility.singleLineHeight;
                }

                var parentObject = property.GetParentObject();
                var propertyValue = attribute._propertyInfo.GetValue(parentObject, null);
                _propertyValues[new SubFieldID(property)] = (parentObject, propertyValue);

                if (propertyValue is ICustomDrawer drawer)
                {
                    return drawer.GetHeight(property.isExpanded);
                }

                return GetBuiltInHeight(propertyValue, property.isExpanded);
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                if (attribute._label != null) label.text = attribute._label;

                if (attribute._propertyInfo == null)
                {
                    EditorGUI.LabelField(position, label.text, "Can't find the property");
                    return;
                }

                if (!attribute._propertyInfo.CanRead)
                {
                    EditorGUI.LabelField(position, label.text, "The property can't read");
                    return;
                }

                var (parentObject, propertyValue) = _propertyValues[new SubFieldID(property)];

                using (DisabledScope.New(!attribute._propertyInfo.CanWrite))
                {
                    using (var scope = ChangeCheckScope.New())
                    {
                        if (propertyValue is ICustomDrawer drawer)
                        {
                            property.isExpanded = drawer.OnGUI(position, property.isExpanded, label, property.serializedObject.targetObjects);
                        }
                        else
                        {
                            property.isExpanded = OnBuiltInGUI(ref propertyValue, position, property.isExpanded, label, property.serializedObject.targetObjects);
                        }

                        if (scope.changed)
                        {
                            attribute._propertyInfo.SetValue(parentObject, propertyValue, null);
                            property.SetParentObject(parentObject);

                            foreach (var target in property.serializedObject.targetObjects)
                            {
                                if (target != property.serializedObject.targetObject)
                                {
                                    parentObject = property.GetParentObject(target);
                                    attribute._propertyInfo.SetValue(parentObject, propertyValue, null);
                                    property.SetParentObject(target, parentObject);
                                }
                            }

                        } // if (scope.changed)

                    } // ChangeCheckScope

                } // DisabledScope

            } // OnGUI

            float GetBuiltInHeight(object propertyValue, bool isExpanded)
            {
                int lines = 1;

                switch (propertyValue)
                {
                    case string _:
                        if (attribute._mode == GetSetMode.StringArea) lines = 6;
                        break;

                    case Vector4 _:
                    case Vector4Int _:
                    case Rect _:
                    case RectInt _:
                        lines = 2;
                        break;

                    case Bounds _:
                    case BoundsInt _:
                        lines = 3;
                        break;
                }

                return EditorGUIUtilities.GetMultipleLinesHeight(lines);
            }

            bool OnBuiltInGUI(ref object propertyValue, Rect position, bool isExpanded, GUIContent label, UnityObject[] objects)
            {
                using (var scope = ChangeCheckScope.New(objects))
                {
                    if (typeof(UnityObject).IsAssignableFrom(attribute._propertyInfo.PropertyType))
                    {
                        bool allowScene = false;
                        if (attribute._mode != GetSetMode.ObjectAssetOnly)
                        {
                            allowScene = !EditorUtility.IsPersistent(objects[0]);
                        }

                        var value = (UnityObject)propertyValue;
                        value = EditorGUI.ObjectField(position, label, value, attribute._propertyInfo.PropertyType, allowScene);

                        if (scope.changed)
                        {
                            if (attribute._mode != GetSetMode.ObjectSceneOnly || !EditorUtility.IsPersistent(value))
                                propertyValue = value;
                            else
                                EditorUtility.DisplayDialog("Unacceptable Target", "You can not select a target which is outside scenes.", "OK");
                        }
                    }
                    else
                    {
                        switch (propertyValue)
                        {
                            case bool value:
                                {
                                    if (attribute._mode == GetSetMode.BoolToggleLeft)
                                        value = EditorGUI.ToggleLeft(position, label, value);
                                    else
                                        value = EditorGUI.Toggle(position, label, value);
                                    if (scope.changed) propertyValue = value;
                                    break;
                                }

                            case int value:
                                {
                                    if (attribute._mode == GetSetMode.IntLayer)
                                        value = EditorGUI.LayerField(position, label, value);
                                    else
                                        value = EditorGUI.IntField(position, label, value);
                                    if (scope.changed) propertyValue = value;
                                    break;
                                }

                            case float value:
                                {
                                    value = EditorGUI.FloatField(position, label, value);
                                    if (scope.changed) propertyValue = value;
                                    break;
                                }

                            case long value:
                                {
                                    value = EditorGUI.LongField(position, label, value);
                                    if (scope.changed) propertyValue = value;
                                    break;
                                }

                            case double value:
                                {
                                    value = EditorGUI.DoubleField(position, label, value);
                                    if (scope.changed) propertyValue = value;
                                    break;
                                }

                            case string value:
                                {
                                    switch (attribute._mode)
                                    {
                                        case GetSetMode.StringArea:
                                            {
                                                var labelRect = position;
                                                labelRect.height = EditorGUIUtility.singleLineHeight;
                                                position.yMin = labelRect.yMax + 2;
                                                EditorGUI.LabelField(labelRect, label);
                                                value = EditorGUI.TextArea(position, value);
                                                break;
                                            }

                                        case GetSetMode.StringTag:
                                            value = EditorGUI.TagField(position, label, value);
                                            break;

                                        case GetSetMode.StringPassword:
                                            value = EditorGUI.PasswordField(position, label, value);
                                            break;

                                        default:
                                            value = EditorGUI.DelayedTextField(position, label, value);
                                            break;
                                    }
                                    if (scope.changed) propertyValue = value;
                                    break;
                                }

                            case Vector2 value:
                                {
                                    value = EditorGUIUtilities.SingleLineVector2Field(position, label, value);
                                    if (scope.changed) propertyValue = value;
                                    break;
                                }

                            case Vector2Int value:
                                {
                                    value = EditorGUIUtilities.SingleLineVector2IntField(position, label, value);
                                    if (scope.changed) propertyValue = value;
                                    break;
                                }

                            case Vector3 value:
                                {
                                    using (WideModeScope.New(true))
                                        value = EditorGUI.Vector3Field(position, label, value);
                                    if (scope.changed) propertyValue = value;
                                    break;
                                }

                            case Vector3Int value:
                                {
                                    using (WideModeScope.New(true))
                                        value = EditorGUI.Vector3IntField(position, label, value);
                                    if (scope.changed) propertyValue = value;
                                    break;
                                }

                            case Vector4 value:
                                {
                                    value = EditorGUIUtilities.TwoLinesVector4Field(position, label, value);
                                    if (scope.changed) propertyValue = value;
                                    break;
                                }

                            case Vector4Int value:
                                {
                                    value = EditorGUIUtilities.TwoLinesVector4IntField(position, label, value);
                                    if (scope.changed) propertyValue = value;
                                    break;
                                }

                            case Rect value:
                                {
                                    value = EditorGUIUtilities.TwoLinesRectField(position, label, value);
                                    if (scope.changed) propertyValue = value;
                                    break;
                                }

                            case RectInt value:
                                {
                                    value = EditorGUIUtilities.TwoLinesRectIntField(position, label, value);
                                    if (scope.changed) propertyValue = value;
                                    break;
                                }

                            case Bounds value:
                                {
                                    using (WideModeScope.New(true))
                                        value = EditorGUI.BoundsField(position, label, value);
                                    if (scope.changed) propertyValue = value;
                                    break;
                                }

                            case BoundsInt value:
                                {
                                    using (WideModeScope.New(true))
                                        value = EditorGUI.BoundsIntField(position, label, value);
                                    if (scope.changed) propertyValue = value;
                                    break;
                                }

                            case Color value:
                                {
                                    bool alpha = (attribute._mode & GetSetMode.ColorAlphaFlag) != GetSetMode.Default;
                                    bool hdr = (attribute._mode & GetSetMode.ColorHDRFlag) != GetSetMode.Default;
                                    value = EditorGUI.ColorField(position, label, value, true, alpha, hdr);
                                    if (scope.changed) propertyValue = value;
                                    break;
                                }

                            case Enum value:
                                {
                                    if (attribute._mode == GetSetMode.EnumFlags)
                                        value = EditorGUI.EnumFlagsField(position, label, value);
                                    else
                                        value = EditorGUI.EnumPopup(position, label, value);
                                    if (scope.changed) propertyValue = value;
                                    break;
                                }

                            case AnimationCurve value:
                                {
                                    value = EditorGUI.CurveField(position, label, value);
                                    if (scope.changed) propertyValue = value;
                                    break;
                                }

                            case Gradient value:
                                {
                                    bool hdr = (attribute._mode & GetSetMode.GradientHDRFlag) != GetSetMode.Default;
                                    value = EditorGUI.GradientField(position, label, value, hdr);
                                    if (scope.changed) propertyValue = value;
                                    break;
                                }

                            case null:
                                {
                                    EditorGUI.LabelField(position, label.text, "null");
                                    break;
                                }

                            default:
                                {
                                    EditorGUI.LabelField(position, label.text, "type is not supported");
                                    break;
                                }

                        } // switch
                    } // else
                } // scope

                return isExpanded;
            }

        } // class GetSetDrawer

#endif

    } // class GetSetAttribute

} // namespace UnityExtensions