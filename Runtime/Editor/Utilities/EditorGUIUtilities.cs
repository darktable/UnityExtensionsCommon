#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections;

namespace UnityExtensions.Editor
{
    /// <summary>
    /// Menu item states.
    /// </summary>
    public enum MenuItemState
    {
        Normal,
        Selected,
        Disabled,
    }


    public struct SubFieldID : IEquatable<SubFieldID>
    {
        public object root;
        public string path;

        public SubFieldID(object root, string path)
        {
            this.root = root;
            this.path = path;
        }

        public SubFieldID(SerializedProperty property)
        {
            this.root = property.serializedObject.targetObject;
            this.path = property.propertyPath;
        }

        public static bool operator ==(SubFieldID a, SubFieldID b)
        {
            return a.root == b.root && a.path == b.path;
        }

        public static bool operator !=(SubFieldID a, SubFieldID b)
        {
            return a.root != b.root || a.path != b.path;
        }

        public bool Equals(SubFieldID other)
        {
            return root == other.root && path == other.path;
        }

        public override bool Equals(object obj) => throw new Exception("What are you doing?");

        public override int GetHashCode() => root.GetHashCode() ^ path.GetHashCode();
    }


    public struct SubFieldInfo
    {
        public FieldInfo fieldInfo;
        public int listIndex;
        public Type actualType;
    }


    /// <summary>
    /// Utilities for editor GUI.
    /// </summary>
    public partial struct EditorGUIUtilities
    {
        static GUIContent _tempContent = new GUIContent();

        static GUIStyle _buttonStyle;
        static GUIStyle _buttonLeftStyle;
        static GUIStyle _buttonMiddleStyle;
        static GUIStyle _buttonRightStyle;
        static GUIStyle _middleCenterLabelStyle;
        static GUIStyle _middleLeftLabelStyle;

        static Texture2D _paneOptionsIconDark;
        static Texture2D _paneOptionsIconLight;

        static int _dragState;
        static Vector2 _dragPos;

        static Dictionary<SubFieldID, SubFieldInfo[]> _subFieldsDictionary = new Dictionary<SubFieldID, SubFieldInfo[]>(256);
        static Stack<object> _parentObjects = new Stack<object>(16);


        public static GUIStyle buttonStyle
        {
            get
            {
                if (_buttonStyle == null) _buttonStyle = "Button";
                return _buttonStyle;
            }
        }


        public static GUIStyle buttonLeftStyle
        {
            get
            {
                if (_buttonLeftStyle == null) _buttonLeftStyle = "ButtonLeft";
                return _buttonLeftStyle;
            }
        }


        public static GUIStyle buttonMiddleStyle
        {
            get
            {
                if (_buttonMiddleStyle == null) _buttonMiddleStyle = "ButtonMid";
                return _buttonMiddleStyle;
            }
        }


        public static GUIStyle buttonRightStyle
        {
            get
            {
                if (_buttonRightStyle == null) _buttonRightStyle = "ButtonRight";
                return _buttonRightStyle;
            }
        }


        public static GUIStyle middleCenterLabelStyle
        {
            get
            {
                if (_middleCenterLabelStyle == null)
                {
                    _middleCenterLabelStyle = new GUIStyle(EditorStyles.label);
                    _middleCenterLabelStyle.alignment = TextAnchor.MiddleCenter;
                }
                return _middleCenterLabelStyle;
            }
        }


        public static GUIStyle middleLeftLabelStyle
        {
            get
            {
                if (_middleLeftLabelStyle == null)
                {
                    _middleLeftLabelStyle = new GUIStyle(EditorStyles.label);
                    _middleLeftLabelStyle.alignment = TextAnchor.MiddleLeft;
                }
                return _middleLeftLabelStyle;
            }
        }


        public static Texture2D paneOptionsIcon
        {
            get
            {
                return EditorGUIUtility.isProSkin ? paneOptionsIconDark : paneOptionsIconLight;
            }
        }


        public static Texture2D paneOptionsIconDark
        {
            get
            {
                if (_paneOptionsIconDark == null)
                {
                    _paneOptionsIconDark = (Texture2D)EditorGUIUtility.Load("Builtin Skins/DarkSkin/Images/pane options.png");
                }
                return _paneOptionsIconDark;
            }
        }


        public static Texture2D paneOptionsIconLight
        {
            get
            {
                if (_paneOptionsIconLight == null)
                {
                    _paneOptionsIconLight = (Texture2D)EditorGUIUtility.Load("Builtin Skins/LightSkin/Images/pane options.png");
                }
                return _paneOptionsIconLight;
            }
        }


        public static Color labelNormalColor
        {
            get { return EditorStyles.label.normal.textColor; }
        }


        public static float GetMultipleLinesHeight(int lines)
        {
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            return (EditorGUIUtility.singleLineHeight + spacing) * lines - spacing;
        }


        /// <summary>
        /// Get a temporary GUIContent（use this to avoid GC).
        /// </summary>
        public static GUIContent TempContent(string text = null, Texture image = null, string tooltip = null)
        {
            _tempContent.text = text;
            _tempContent.image = image;
            _tempContent.tooltip = tooltip;

            return _tempContent;
        }


        /// <summary>
        /// Draw a rect wireframe.
        /// </summary>
        public static void DrawWireRect(Rect rect, Color color, float borderWidth = 1f)
        {
            Rect border = new Rect(rect.x, rect.y, rect.width, borderWidth);
            EditorGUI.DrawRect(border, color);
            border.y = rect.yMax - borderWidth;
            EditorGUI.DrawRect(border, color);
            border.yMax = border.yMin;
            border.yMin = rect.yMin + borderWidth;
            border.width = borderWidth;
            EditorGUI.DrawRect(border, color);
            border.x = rect.xMax - borderWidth;
            EditorGUI.DrawRect(border, color);
        }


        /// <summary>
        /// Draw a indented button.
        /// </summary>
        public static bool IndentedButton(string text)
        {
            var rect = EditorGUILayout.GetControlRect(true);
            rect.xMin += EditorGUIUtility.labelWidth;
            return GUI.Button(rect, text, EditorStyles.miniButton);
        }


        /// <summary>
        /// Draw a indented toggle-button.
        /// </summary>
        public static bool IndentedToggleButton(string text, bool value)
        {
            var rect = EditorGUILayout.GetControlRect(true);
            rect.xMin += EditorGUIUtility.labelWidth;
            return GUI.Toggle(rect, value, text, EditorStyles.miniButton);
        }


        public static bool DrawReferenceDetails(UnityEngine.Object reference, bool foldout, string label, ref UnityEditor.Editor cachedEditor)
        {
            if (reference)
            {
                var rect = EditorGUILayout.GetControlRect();
                rect.xMin -= 13;

                foldout = GUI.Toggle(rect, foldout, label, EditorStyles.foldout);

                if (foldout)
                {
                    var start = EditorGUILayout.GetControlRect(false, 0f).position;

                    using (IndentLevelScope.New())
                    {
                        UnityEditor.Editor.CreateCachedEditor(reference, null, ref cachedEditor);
                        cachedEditor.OnInspectorGUI();
                    }

                    var end = EditorGUILayout.GetControlRect(false, 0f).position;
                    end.y -= 2f;

                    var color = labelNormalColor;
                    color.a = 0.4f;

                    EditorGUI.DrawRect(new Rect(start.x - 7f, start.y, 2f, end.y - start.y), color);
                }
            }

            return foldout;
        }


        public static Vector2 SingleLineVector2Field(Rect rect, GUIContent label, Vector2 value, string subLabel1 = "X", string subLabel2 = "Y")
        {
            rect = EditorGUI.PrefixLabel(rect, GUIUtility.GetControlID(label, FocusType.Keyboard, rect), label);

            using (LabelWidthScope.New(12))
            {
                using (IndentLevelScope.New(0, false))
                {
                    rect.width = (rect.width - 4) * 0.5f;
                    value.x = EditorGUI.FloatField(rect, subLabel1, value.x);
                    rect.x = rect.xMax + 4;
                    value.y = EditorGUI.FloatField(rect, subLabel2, value.y);
                }
            }
            return value;
        }


        public static Vector2Int SingleLineVector2IntField(Rect rect,GUIContent label, Vector2Int value, string subLabel1 = "X", string subLabel2 = "Y")
        {
            rect = EditorGUI.PrefixLabel(rect, GUIUtility.GetControlID(label, FocusType.Keyboard, rect), label);

            using (LabelWidthScope.New(12))
            {
                using (IndentLevelScope.New(0, false))
                {
                    rect.width = (rect.width - 4) * 0.5f;
                    value.x = EditorGUI.IntField(rect, subLabel1, value.x);
                    rect.x = rect.xMax + 4;
                    value.y = EditorGUI.IntField(rect, subLabel2, value.y);
                }
            }
            return value;
        }


        public static Vector4 TwoLinesVector4Field(Rect rect, GUIContent label, Vector4 value, string subLabel1 = "X", string subLabel2 = "Y", string subLabel3 = "Z", string subLabel4 = "W")
        {
            var xy = new Vector2(value.x, value.y);
            rect.height = EditorGUIUtility.singleLineHeight;
            xy = SingleLineVector2Field(rect, label, xy, subLabel1, subLabel2);

            var zw = new Vector2(value.z, value.w);
            rect.y = rect.yMax + EditorGUIUtility.standardVerticalSpacing;
            zw = SingleLineVector2Field(rect, TempContent(), zw, subLabel3, subLabel4);

            return new Vector4(xy.x, xy.y, zw.x, zw.y);
        }


        public static Vector4Int TwoLinesVector4IntField(Rect rect, GUIContent label, Vector4Int value, string subLabel1 = "X", string subLabel2 = "Y", string subLabel3 = "Z", string subLabel4 = "W")
        {
            var xy = new Vector2Int(value.x, value.y);
            rect.height = EditorGUIUtility.singleLineHeight;
            xy = SingleLineVector2IntField(rect, label, xy, subLabel1, subLabel2);

            var zw = new Vector2Int(value.z, value.w);
            rect.y = rect.yMax + EditorGUIUtility.standardVerticalSpacing;
            zw = SingleLineVector2IntField(rect, TempContent(), zw, subLabel3, subLabel4);

            return new Vector4Int(xy.x, xy.y, zw.x, zw.y);
        }


        public static Rect TwoLinesRectField(Rect rect, GUIContent label, Rect value)
        {
            var v4 = new Vector4(value.x, value.y, value.width, value.height);
            v4 = TwoLinesVector4Field(rect, label, v4, "X", "Y", "W", "H");
            return new Rect(v4.x, v4.y, v4.z, v4.w);
        }


        public static RectInt TwoLinesRectIntField(Rect rect, GUIContent label, RectInt value)
        {
            var v4 = new Vector4Int(value.x, value.y, value.width, value.height);
            v4 = TwoLinesVector4IntField(rect, label, v4, "X", "Y", "W", "H");
            return new RectInt(v4.x, v4.y, v4.z, v4.w);
        }


        /// <summary>
        /// Drag mouse to change value.
        /// </summary>
        public static float DragValue(Rect rect, float value, float sensitivity)
        {
            int id = GUIUtility.GetControlID(FocusType.Passive);
            Event current = Event.current;

            switch (current.GetTypeForControl(id))
            {
                case EventType.MouseDown:
                    if (rect.Contains(current.mousePosition) && current.button == 0)
                    {
                        EditorGUIUtility.editingTextField = false;
                        GUIUtility.hotControl = id;
                        _dragState = 1;
                        _dragPos = current.mousePosition;
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(1);
                    }
                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == id && _dragState != 0)
                    {
                        GUIUtility.hotControl = 0;
                        _dragState = 0;
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(0);
                    }
                    break;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl != id)
                    {
                        break;
                    }
                    switch (_dragState)
                    {
                        case 1:
                            if ((Event.current.mousePosition - _dragPos).sqrMagnitude > 4)
                            {
                                _dragState = 2;
                            }
                            current.Use();
                            break;

                        case 2:
                            value += HandleUtility.niceMouseDelta * sensitivity;
                            value = MathUtilities.RoundToSignificantDigitsFloat(value, 6);
                            GUI.changed = true;
                            current.Use();
                            break;
                    }
                    break;

                case EventType.Repaint:
                    EditorGUIUtility.AddCursorRect(rect, MouseCursor.SlideArrow);
                    break;
            }

            return value;
        }


        /// <summary>
        /// Drag mouse to change value.
        /// </summary>
        public static float DragValue(Rect rect, GUIContent content, float value, float sensitivity, GUIStyle style)
        {
            GUI.Label(rect, content, style);
            return DragValue(rect, value, sensitivity);
        }


        /// <summary>
        /// Draw a progress bar that can be dragged.
        /// </summary>
        public static float DragProgress(
            Rect rect,
            float value01,
            Color backgroundColor,
            Color foregroundColor,
            bool draggable = true)
        {
            var progressRect = rect;
            progressRect.width = Mathf.Round(progressRect.width * value01);

            EditorGUI.DrawRect(rect, backgroundColor);
            EditorGUI.DrawRect(progressRect, foregroundColor);

            int id = GUIUtility.GetControlID(FocusType.Passive);
            Event current = Event.current;

            switch (current.GetTypeForControl(id))
            {
                case EventType.MouseDown:
                    if (rect.Contains(current.mousePosition) && current.button == 0)
                    {
                        EditorGUIUtility.editingTextField = false;
                        GUIUtility.hotControl = id;
                        _dragState = 1;

                        if (draggable)
                        {
                            float offset = current.mousePosition.x - rect.x + 1f;
                            value01 = Mathf.Clamp01(offset / rect.width);
                        }

                        GUI.changed = true;

                        current.Use();
                    }
                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == id && _dragState != 0)
                    {
                        GUIUtility.hotControl = 0;
                        _dragState = 0;
                        current.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl != id)
                    {
                        break;
                    }
                    if (_dragState != 0)
                    {
                        if (draggable)
                        {
                            float offset = current.mousePosition.x - rect.x + 1f;
                            value01 = Mathf.Clamp01(offset / rect.width);
                        }

                        GUI.changed = true;

                        current.Use();
                    }
                    break;

                case EventType.Repaint:
                    if (draggable) EditorGUIUtility.AddCursorRect(rect, MouseCursor.SlideArrow);
                    break;
            }

            return value01;
        }


        /// <summary>
        /// Draw a progress bar that can be dragged.
        /// </summary>
        public static float DragProgress(
            Rect rect,
            float value01,
            Color backgroundColor,
            Color foregroundColor,
            Color borderColor,
            bool drawForegroundBorder = false,
            bool draggable = true)
        {
            float result = DragProgress(rect, value01, backgroundColor, foregroundColor, draggable);

            DrawWireRect(rect, borderColor);

            if (drawForegroundBorder)
            {
                rect.width = Mathf.Round(rect.width * value01);
                if (rect.width > 0f)
                {
                    rect.xMin = rect.xMax - 1f;
                    EditorGUI.DrawRect(rect, borderColor);
                }
            }

            return result;
        }


        /// <summary>
        /// Create a menu.
        /// </summary>
        /// <param name="itemCount"> Number of items, include separators and childs. </param>
        /// <param name="getItemContent"> Get content of an item, a separator must ends with '/' </param>
        /// <param name="getItemState"> Get state of an item </param>
        /// <returns> The created menu, use DropDown or ShowAsContext to show it. </returns>
        public static GenericMenu CreateMenu(
            int itemCount,
            Func<int, GUIContent> getItemContent,
            Func<int, MenuItemState> getItemState,
            Action<int> onSelect)
        {
            GenericMenu menu = new GenericMenu();
            GUIContent content;
            MenuItemState state;

            for(int i=0; i<itemCount; i++)
            {
                content = getItemContent(i);
                if(content.text.EndsWith("/"))
                {
                    menu.AddSeparator(content.text.Substring(0, content.text.Length - 1));
                }
                else
                {
                    state = getItemState(i);
                    if(state == MenuItemState.Disabled)
                    {
                        menu.AddDisabledItem(content);
                    }
                    else
                    {
                        int index = i;
                        menu.AddItem(content, state == MenuItemState.Selected, () => onSelect(index));
                    }
                }
            }

            return menu;
        }


        /// <summary>
        /// Create a menu.
        /// </summary>
        /// <param name="itemCount"> Number of items, include separators and childs. </param>
        /// <param name="getItemContent"> Get content of an item, a separator must ends with '/' </param>
        /// <param name="getItemState"> Get state of an item </param>
        /// <returns> The created menu, use DropDown or ShowAsContext to show it. </returns>
        public static GenericMenu CreateMenu<T>(
            IEnumerable<T> items,
            Func<T, GUIContent> getItemContent,
            Func<T, MenuItemState> getItemState,
            Action<T> onSelect)
        {
            GenericMenu menu = new GenericMenu();
            GUIContent content;
            MenuItemState state;

            foreach (var i in items)
            {
                content = getItemContent(i);
                if (content.text.EndsWith("/"))
                {
                    menu.AddSeparator(content.text.Substring(0, content.text.Length - 1));
                }
                else
                {
                    state = getItemState(i);
                    if (state == MenuItemState.Disabled)
                    {
                        menu.AddDisabledItem(content);
                    }
                    else
                    {
                        T current = i;
                        menu.AddItem(content, state == MenuItemState.Selected, () => onSelect(current));
                    }
                }
            }

            return menu;
        }


        public static bool IsArrayOrList(Type type)
        {
            return type.IsArray || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>));
        }


        public static Type GetArrayOrListElementType(Type type)
        {
            return type.IsArray ? type.GetElementType() : type.GetGenericArguments()[0];
        }


        /// <summary>
        /// Supports [SerializeReference]
        /// </summary>
        /// <param name="root"> the unity object </param>
        /// <param name="path"> "a.b.Array.data[3].c" </param>
        /// <param name="parent"> 0-self, 1-parent, 2-grandparent, ... </param>
        /// <returns></returns>
        public static void GetSetSerializedPropertyRepresentedObject(bool set, object root, string path, ref object value, int parent = 0)
        {
            SubFieldID id = new SubFieldID(root, path);
            if (set) _parentObjects.Clear();

            if (_subFieldsDictionary.TryGetValue(id, out var subFields))
            {
                parent = subFields.Length - parent;

                for (int i = 0; i < parent; i++)
                {
                    if (set) _parentObjects.Push(root);

                    ref var subField = ref subFields[i];

                    root = subField.fieldInfo.GetValue(root);
                    if (subField.listIndex >= 0) root = ((IList)root)[subField.listIndex];

                    var type = root?.GetType();
                    if (type != subField.actualType)
                    {
                        subField.actualType = type;

                        // Field actual type changed, need update all sub-fields info
                        for (i++; i < parent; i++)
                        {
                            if (set) _parentObjects.Push(root);

                            subField = ref subFields[i];
                            subField.fieldInfo = type.GetInstanceField(subField.fieldInfo.Name);

                            root = subField.fieldInfo.GetValue(root);
                            if (subField.listIndex >= 0) root = ((IList)root)[subField.listIndex];

                            subField.actualType = type = root?.GetType();
                        }

                        break;
                    }
                }

                if (!set) value = root;
            }
            else
            {
                var names = path.Replace(".Array.data[", "[").Split('.');
                subFields = new SubFieldInfo[names.Length];
                var type = root.GetType();

                parent = subFields.Length - parent;

                for (int i = 0; i < subFields.Length; i++)
                {
                    if (set)
                    {
                        if (i < parent) _parentObjects.Push(root);
                    }
                    else
                    {
                        if (i == parent) value = root;
                    }

                    ref var subField = ref subFields[i];

                    subField.listIndex = -1;
                    var name = names[i];

                    if (name[name.Length - 1] == ']')
                    {
                        int leftIndex = name.IndexOf('[');
                        subField.listIndex = int.Parse(name.Substring(leftIndex + 1, name.Length - leftIndex - 2));
                        name = name.Substring(0, leftIndex);
                    }

                    subField.fieldInfo = type.GetInstanceField(name);

                    root = subField.fieldInfo.GetValue(root);
                    if (subField.listIndex >= 0) root = ((IList)root)[subField.listIndex];

                    subField.actualType = type = root?.GetType();
                }

                _subFieldsDictionary.Add(id, subFields);
            }

            if (set)
            {
                var fieldValue = value;
                for (int i = parent - 1; i >= 0; i--)
                {
                    ref var subField = ref subFields[i];

                    root = _parentObjects.Pop();
                    if (subField.listIndex >= 0)
                    {
                        ((IList)subField.fieldInfo.GetValue(root))[subField.listIndex] = fieldValue;
                    }
                    else
                    {
                        subField.fieldInfo.SetValue(root, fieldValue);
                    }

                    fieldValue = root;
                }
            }
        }

    } // struct EditorGUIUtilities

} // namespace UnityExtensions.Editor

#endif // UNITY_EDITOR