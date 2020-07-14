#if UNITY_EDITOR
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
#endif

namespace UnityExtensions
{
    public interface IWrappedArray
    {
        int length { get; }
        bool isNullOrEmpty { get; }
    }

    public interface IWrappedList
    {
        int count { get; }
        bool isNullOrEmpty { get; }
    }

    public static class WrappedArrayCodeGenerator
    {
        public static string Generate(string[] usingItems, string elementTypeName, string wrappedTypeNamePrefix)
        {
            string usings = "";
            if (usingItems != null)
            {
                foreach (var ns in usingItems)
                {
                    if (!string.IsNullOrWhiteSpace(ns))
                        usings += $"\r\nusing {ns};";
                }
            }

            var T = wrappedTypeNamePrefix;
            var t = elementTypeName;

            return
//-----------------------------------------------------------------------------
$@"
using System;
using System.Collections.Generic;{usings}

namespace UnityExtensions
{{

    [Serializable]
    public struct {T}Array : IWrappedArray, IEquatable<{T}Array>, IEquatable<{t}[]>
    {{
        public {t}[] data;
        public int length => data.Length;
        public bool isNullOrEmpty => data == null || data.Length == 0;
        public ref {t} this[int index] => ref data[index];
        public static implicit operator {t}[]({T}Array wrap) => wrap.data;
        public static implicit operator {T}Array({t}[] data) => new {T}Array {{ data = data }};
        public static bool operator ==({T}Array a, {T}Array b) => a.data == b.data;
        public static bool operator !=({T}Array a, {T}Array b) => a.data != b.data;
        public static bool operator ==({T}Array a, {t}[] b) => a.data == b;
        public static bool operator !=({T}Array a, {t}[] b) => a.data != b;
        public static bool operator ==({t}[] a, {T}Array b) => a == b.data;
        public static bool operator !=({t}[] a, {T}Array b) => a != b.data;
        public override bool Equals(object obj) => (obj is {T}Array wrap) ? data == wrap.data : data == obj;
        public bool Equals({T}Array other) => data == other.data;
        public bool Equals({t}[] other) => data == other;
        public override int GetHashCode() => data.GetHashCode();
    }}


    [Serializable]
    public struct {T}List : IWrappedList, IEquatable<{T}List>, IEquatable<List<{t}>>
    {{
        public List<{t}> data;
        public int count => data.Count;
        public bool isNullOrEmpty => data == null || data.Count == 0;
        public {t} this[int index] {{ get => data[index]; set => data[index] = value; }}
        public static implicit operator List<{t}>({T}List wrap) => wrap.data;
        public static implicit operator {T}List(List<{t}> data) => new {T}List {{ data = data }};
        public static bool operator ==({T}List a, {T}List b) => a.data == b.data;
        public static bool operator !=({T}List a, {T}List b) => a.data != b.data;
        public static bool operator ==({T}List a, List<{t}> b) => a.data == b;
        public static bool operator !=({T}List a, List<{t}> b) => a.data != b;
        public static bool operator ==(List<{t}> a, {T}List b) => a == b.data;
        public static bool operator !=(List<{t}> a, {T}List b) => a != b.data;
        public override bool Equals(object obj) => (obj is {T}List wrap) ? data == wrap.data : data == obj;
        public bool Equals({T}List other) => data == other.data;
        public bool Equals(List<{t}> other) => data == other;
        public override int GetHashCode() => data.GetHashCode();
    }}

}}
";
//-----------------------------------------------------------------------------
        }
    }

#if UNITY_EDITOR

    namespace Editor
    {
        class WrappedArrayCodeGeneratorWindow : EditorWindow
        {
            [SerializeField] string[] _usingItems = new string[] { "UnityObject = UnityEngine.Object" };
            [SerializeField] string _elementTypeName = "UnityObject";
            [SerializeField] string _wrappedTypeNamePrefix = "Object";

            SerializedObject _serializedObject;
            SerializedProperty _usingItemsProp;
            SerializedProperty _elementTypeNameProp;
            SerializedProperty _wrappedTypeNamePrefixProp;
            string _generatedCode;
            Vector2 _scrollPosition;

            [MenuItem("Assets/Create/Unity Extensions/Wrapped Array Code...")]
            public static void ShowWindow()
            {
                var window = GetWindow<WrappedArrayCodeGeneratorWindow>("Wrapped Array Code Generator");
                window.minSize = new Vector2(640, 480);
                window.Show();
            }

            private void OnEnable()
            {
                _serializedObject = new SerializedObject(this);
                _usingItemsProp = _serializedObject.FindProperty(nameof(_usingItems));
                _elementTypeNameProp = _serializedObject.FindProperty(nameof(_elementTypeName));
                _wrappedTypeNamePrefixProp = _serializedObject.FindProperty(nameof(_wrappedTypeNamePrefix));
            }

            private void OnValidate()
            {
                _generatedCode = null;
            }

            private void OnGUI()
            {
                using (EditorGUILayoutScrollViewScope.New(ref _scrollPosition))
                {
                    using (LabelWidthScope.New(position.width * 0.4f))
                    {
                        _serializedObject.Update();
                        EditorGUILayout.PropertyField(_usingItemsProp);
                        EditorGUILayout.PropertyField(_elementTypeNameProp);
                        EditorGUILayout.PropertyField(_wrappedTypeNamePrefixProp);
                        _serializedObject.ApplyModifiedProperties();
                    }

                    var rect = EditorGUILayout.GetControlRect();
                    rect.width = (rect.width - 8) / 2;
                    using (DisabledScope.New(string.IsNullOrWhiteSpace(_elementTypeName) || string.IsNullOrWhiteSpace(_wrappedTypeNamePrefix)))
                    {
                        if (GUI.Button(rect, "Generate Code"))
                        {
                            _generatedCode = WrappedArrayCodeGenerator.Generate(_usingItems, _elementTypeName, _wrappedTypeNamePrefix);
                        }
                    }

                    rect.x = rect.xMax + 8;
                    using (DisabledScope.New(string.IsNullOrWhiteSpace(_generatedCode)))
                    {
                        if (GUI.Button(rect, "Save \".cs\" File..."))
                        {
                            var path = EditorUtility.SaveFilePanelInProject("Save \".cs\" File...", $"{_wrappedTypeNamePrefix}Array", "cs", "");
                            if (!string.IsNullOrWhiteSpace(path))
                            {
                                File.WriteAllText(path, _generatedCode, System.Text.Encoding.Default);
                                AssetDatabase.Refresh();
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(_generatedCode))
                    {
                        EditorGUILayout.TextArea(_generatedCode);
                    }
                }
            }
        }


        [CustomPropertyDrawer(typeof(IWrappedArray), true)]
        [CustomPropertyDrawer(typeof(IWrappedList), true)]
        public class WrappedArrayDrawer : PropertyDrawer
        {
            ReorderableList _list;
            static FieldInfo _serializedObjectInfo;

            void ValidateList(SerializedProperty property)
            {
                if (_list == null)
                {
                    _list = new ReorderableList(property.serializedObject, property.FindPropertyRelative("data"), true, false, false, false);
                    _list.elementHeightCallback = ElementHeight;
                    _list.drawElementBackgroundCallback = DrawElementBackground;
                    _list.drawElementCallback = DrawElement;
                    _list.showDefaultBackground = false;
                    _list.headerHeight = 0;
                    _list.footerHeight = 0;

                }
                else
                {
                    if (_serializedObjectInfo == null)
                        _serializedObjectInfo = typeof(ReorderableList).GetField("m_SerializedObject", BindingFlags.Instance | BindingFlags.NonPublic);

                    _serializedObjectInfo.SetValue(_list, property.serializedObject);
                    _list.serializedProperty = property.FindPropertyRelative("data");
                }
            }

            float ElementHeight(int index)
            {
                using (WideModeScope.New(true))
                {
                    var property = _list.serializedProperty.GetArrayElementAtIndex(index);
                    return EditorGUI.GetPropertyHeight(property, property.isExpanded) + 2;
                }
            }

            void DrawElementBackground(Rect rect, int index, bool isActive, bool isFocused)
            {
                if (isFocused)
                {
                    EditorGUI.DrawRect(rect, new Color(0, 0.5f, 1f, 0.5f));
                }
                else if (isActive)
                {
                    var color = EditorGUIUtilities.labelNormalColor;
                    color.a = 0.125f;
                    EditorGUI.DrawRect(rect, color);
                }
            }

            void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
            {
                using (WideModeScope.New(true))
                {
                    var property = _list.serializedProperty.GetArrayElementAtIndex(index);
                    if (!property.hasVisibleChildren) rect.y += 1f;
                    rect.height -= 2f;

                    if (property.hasVisibleChildren)
                    {
                        rect.xMin += 12;
                        using (LabelWidthScope.New(-12, true))
                        {
                            EditorGUI.PropertyField(rect, property, property.isExpanded);
                        }
                    }
                    else EditorGUI.PropertyField(rect, property, property.isExpanded);
                }
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                if (!property.isExpanded)
                    return EditorGUIUtility.singleLineHeight;

                ValidateList(property);
                return (_list.count > 0 ? _list.GetHeight() : 0) + (EditorGUIUtility.singleLineHeight + 2f) * 2;
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                var dataProperty = property.FindPropertyRelative("data");

                var headRect = position;
                headRect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(headRect, property, label, false);

                headRect.xMin += EditorGUIUtility.labelWidth + 2;
                EditorGUI.LabelField(headRect, dataProperty.hasMultipleDifferentValues ? "-" : dataProperty.arraySize.ToString());

                if (property.isExpanded)
                {
                    if (_list != null && _list.count > 0)
                    {
                        var listRect = position;
                        listRect.yMin = headRect.yMax + 2;
                        listRect.yMax = position.yMax - EditorGUIUtility.singleLineHeight - 2;
                        _list.DoList(listRect);
                    }

                    ValidateList(property);

                    position.yMin = position.yMax - EditorGUIUtility.singleLineHeight - 4;
                    position.yMax -= 4;
                    position.xMin += EditorGUIUtility.labelWidth + EditorGUIUtility.singleLineHeight + 4;
                    position.width = (position.width - 6) / 2;
                    if (GUI.Button(position, "Add", EditorStyles.miniButtonLeft))
                    {
                        if (_list.index < 0)
                        {
                            dataProperty.arraySize++;
                            _list.index = dataProperty.arraySize - 1;
                        }
                        else
                        {
                            dataProperty.InsertArrayElementAtIndex(_list.index);
                            _list.index++;
                        }
                    }

                    using (DisabledScope.New(_list.index < 0))
                    {
                        position.x = position.xMax;
                        if (GUI.Button(position, "Remove", EditorStyles.miniButtonRight))
                        {
                            dataProperty.DeleteArrayElementAtIndex(_list.index);
                            if (_list.index > 0 || dataProperty.arraySize == 0) _list.index--;
                        }
                    }
                }
            }
        }
    }

#endif // UNITY_EDITOR

} // namespace UnityExtensions