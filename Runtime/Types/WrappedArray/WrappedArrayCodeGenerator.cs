using UnityEngine;

#if UNITY_EDITOR
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
#endif

namespace UnityExtensions
{
    public interface IWrappedArray
    {
    }

    public interface IWrappedPolymorphicArray : IWrappedArray
    {
    }

    public static class WrappedArrayCodeGenerator
    {
        public static string Generate(string[] usingItems, string nameSpace, string elementTypeName, string wrappedTypeNamePrefix, bool polymorphic)
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
            var attr = polymorphic ? $"[{nameof(SerializeReference)}] " : "";
            var parent = polymorphic ? nameof(IWrappedPolymorphicArray) : nameof(IWrappedArray);

            return
//-----------------------------------------------------------------------------
$@"
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityExtensions;{usings}

namespace {nameSpace}
{{
    [Serializable]
    public struct {T}Array : {parent}, IEquatable<{T}Array>, IEquatable<{t}[]>
    {{
        {attr}public {t}[] data;
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
        public Enumerator GetEnumerator() => new Enumerator(data);

        public struct Enumerator
        {{
            {t}[] _data; int _index;
            internal Enumerator({t}[] data) {{ _data = data; _index = -1; }}
            public {t} Current => _data[_index];
            public bool MoveNext() => (++_index) < _data.Length;
            public void Reset() => _index = -1;
        }}
    }}

    [Serializable]
    public struct {T}List : {parent}, IEquatable<{T}List>, IEquatable<List<{t}>>
    {{
        {attr}public List<{t}> data;
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
        public List<{t}>.Enumerator GetEnumerator() => data.GetEnumerator();
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
            [SerializeField] StringArray _usingItems = new string[] { "UnityObject = UnityEngine.Object" };
            [SerializeField] string _nameSpace = "UnityExtensions";
            [SerializeField] string _elementTypeName = "UnityObject";
            [SerializeField] string _wrappedTypeNamePrefix = "Object";
            [SerializeField] bool _polymorphic = false;

            SerializedObject _serializedObject;
            SerializedProperty _nameSpaceProp;
            SerializedProperty _usingItemsProp;
            SerializedProperty _elementTypeNameProp;
            SerializedProperty _wrappedTypeNamePrefixProp;
            SerializedProperty _polymorphicProp;

            string _generatedCode;
            Vector2 _scrollPosition;

            [MenuItem("Assets/Create/Unity Extensions/Wrapped Array Script...")]
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
                _nameSpaceProp = _serializedObject.FindProperty(nameof(_nameSpace));
                _elementTypeNameProp = _serializedObject.FindProperty(nameof(_elementTypeName));
                _wrappedTypeNamePrefixProp = _serializedObject.FindProperty(nameof(_wrappedTypeNamePrefix));
                _polymorphicProp = _serializedObject.FindProperty(nameof(_polymorphic));
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
                        EditorGUILayout.PropertyField(_nameSpaceProp);
                        EditorGUILayout.PropertyField(_elementTypeNameProp);
                        EditorGUILayout.PropertyField(_wrappedTypeNamePrefixProp);
                        EditorGUILayout.PropertyField(_polymorphicProp);
                        _serializedObject.ApplyModifiedProperties();
                    }

                    var rect = EditorGUILayout.GetControlRect();
                    rect.width = (rect.width - 8) / 2;
                    using (DisabledScope.New(string.IsNullOrWhiteSpace(_elementTypeName) || string.IsNullOrWhiteSpace(_wrappedTypeNamePrefix)))
                    {
                        if (GUI.Button(rect, "Generate Code"))
                        {
                            _generatedCode = WrappedArrayCodeGenerator.Generate(_usingItems, _nameSpace, _elementTypeName, _wrappedTypeNamePrefix, _polymorphic);
                        }
                    }

                    rect.x = rect.xMax + 8;
                    using (DisabledScope.New(string.IsNullOrWhiteSpace(_generatedCode)))
                    {
                        if (GUI.Button(rect, "Save \".cs\" File..."))
                        {
                            var path = EditorUtility.SaveFilePanel("Save \".cs\" File...", "Assets", $"{_wrappedTypeNamePrefix}Array", "cs");
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
        public class WrappedArrayDrawer : PropertyDrawer
        {
            static Dictionary<SubFieldID, (ReorderableList list, SerializedObject obj)> _lists
                = new Dictionary<SubFieldID, (ReorderableList, SerializedObject)>();

            protected const string wrappedArrayName = "data";

            static FieldInfo _listSerializedObjectInfo;

            static ReorderableList GetList(SerializedProperty wrappedProperty, SerializedProperty arrayProperty)
            {
                var id = new SubFieldID(wrappedProperty);
                if (!_lists.TryGetValue(id, out var data))
                {
                    data.obj = wrappedProperty.serializedObject;
                    data.list = new ReorderableList(data.obj, arrayProperty, true, false, false, false);
                    data.list.elementHeightCallback = index => GetElementHeight(data.list, index);
                    data.list.drawElementBackgroundCallback = DrawElementBackground;
                    data.list.drawElementCallback = (rect, index, active, focus) => DrawElement(data.list, rect, index, active, focus);
                    data.list.showDefaultBackground = false;
                    data.list.headerHeight = 0;
                    data.list.footerHeight = 0;

                    _lists[id] = data;
                }
                else
                {
                    if (wrappedProperty.serializedObject != data.obj)
                    {
                        data.obj = wrappedProperty.serializedObject;

                        if (_listSerializedObjectInfo == null)
                            _listSerializedObjectInfo = typeof(ReorderableList).GetInstanceField("m_SerializedObject");

                        _listSerializedObjectInfo.SetValue(data.list, data.obj);
                        data.list.index = -1;

                        _lists[id] = data;
                    }
                    data.list.serializedProperty = arrayProperty;
                }
                return data.list;
            }

            static float GetElementHeight(ReorderableList list, int index)
            {
                var property = list.serializedProperty.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(property, true) + 2;
            }

            static void DrawElementBackground(Rect rect, int index, bool isActive, bool isFocused)
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

            static void DrawElement(ReorderableList list, Rect rect, int index, bool isActive, bool isFocused)
            {
                var property = list.serializedProperty.GetArrayElementAtIndex(index);
                if (!property.hasVisibleChildren) rect.y += 1f;
                rect.height -= 2f;

                if (property.hasVisibleChildren)
                {
                    rect.xMin += 12;
                    using (LabelWidthScope.New(-12, true))
                    {
                        EditorGUI.PropertyField(rect, property, true);
                    }
                }
                else EditorGUI.PropertyField(rect, property, true);
            }

            protected virtual bool showAddDropDown => false;

            protected virtual void SetAddDropDownMenu(GenericMenu menu, ReorderableList list)
            {
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                if (!property.isExpanded)
                    return EditorGUIUtility.singleLineHeight;

                var arrayProperty = property.FindPropertyRelative(wrappedArrayName);
                var list = GetList(property, arrayProperty);
                return (list.count > 0 ? list.GetHeight() : 0) + (EditorGUIUtility.singleLineHeight + 2) * 2;
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                var arrayProperty = property.FindPropertyRelative(wrappedArrayName);

                var headRect = position;
                headRect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(headRect, property, label, false);

                headRect.xMin += EditorGUIUtility.labelWidth + 2;
                if (!property.hasMultipleDifferentValues)
                    EditorGUI.LabelField(headRect, arrayProperty.arraySize.ToString());

                if (property.isExpanded)
                {
                    var list = GetList(property, arrayProperty);

                    if (list.count > 0)
                    {
                        var listRect = position;
                        listRect.yMin = headRect.yMax + 2;
                        listRect.yMax = position.yMax - EditorGUIUtility.singleLineHeight - 2;
                        list.DoList(listRect);
                    }

                    position.yMax -= 3;
                    position.yMin = position.yMax - EditorGUIUtility.singleLineHeight;
                    position.xMin += EditorGUIUtility.labelWidth + EditorGUIUtility.singleLineHeight + 4;
                    position.xMax -= 6;

                    position.width = position.width / 2;

                    if (GUI.Button(position, showAddDropDown ? "Add..." : "Add", EditorStyles.miniButtonLeft))
                    {
                        if (showAddDropDown)
                        {
                            GenericMenu menu = new GenericMenu();
                            SetAddDropDownMenu(menu, list);
                            menu.DropDown(position);
                        }
                        else
                        {
                            if (list.index < 0)
                            {
                                arrayProperty.arraySize++;
                                list.index = arrayProperty.arraySize - 1;
                            }
                            else
                            {
                                arrayProperty.InsertArrayElementAtIndex(list.index);
                                list.index++;
                            }
                        }
                    }

                    position.x = position.xMax;

                    using (DisabledScope.New(list.index < 0))
                    {
                        if (GUI.Button(position, "Remove", EditorStyles.miniButtonRight))
                        {
                            arrayProperty.DeleteArrayElementAtIndex(list.index);
                            if (list.index > 0 || arrayProperty.arraySize == 0) list.index--;
                        }
                    }
                }
            }
        }


        [CustomPropertyDrawer(typeof(IWrappedPolymorphicArray), true)]
        public class WrappedPolymorphicArrayDrawer : WrappedArrayDrawer
        {
            static Dictionary<Type, List<Type>> _availableTypes = new Dictionary<Type, List<Type>>();

            static bool IsTypeAvailable(Type type)
            {
                var constructorFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

                return type.IsClass
                    && !type.IsAbstract
                    && !type.IsGenericType
                    && type.GetCustomAttribute<SerializableAttribute>(false) != null
                    && type.GetConstructor(constructorFlags, null, Type.EmptyTypes, null) != null;
            }

            List<Type> GetAvailableTypes()
            {
                var type = fieldInfo.FieldType;

                if (!_availableTypes.TryGetValue(type, out var list))
                {
                    var elementType = type.GetInstanceField(wrappedArrayName).FieldType;
                    elementType = EditorGUIUtilities.GetArrayOrListElementType(elementType);

                    list = new List<Type>();
                    if (IsTypeAvailable(elementType)) list.Add(elementType);

                    foreach (var t in TypeCache.GetTypesDerivedFrom(elementType))
                    {
                        if (IsTypeAvailable(t)) list.Add(t);
                    }

                    _availableTypes.Add(type, list);
                }
                return list;
            }

            protected override bool showAddDropDown => true;

            protected override void SetAddDropDownMenu(GenericMenu menu, ReorderableList list)
            {
                var types = GetAvailableTypes();

                if (types.Count == 0)
                {
                    menu.AddDisabledItem(new GUIContent("No Available Type"));
                }
                else
                {
                    foreach (var t in types)
                    {
                        menu.AddItem(new GUIContent(t.Name), false, type =>
                        {
                            var obj = Activator.CreateInstance((Type)type);
                            if (list.index < 0)
                            {
                                list.serializedProperty.arraySize++;
                                list.index = list.serializedProperty.arraySize - 1;
                            }
                            else
                            {
                                list.serializedProperty.InsertArrayElementAtIndex(list.index);
                                list.index++;
                            }

                            list.serializedProperty.GetArrayElementAtIndex(list.index).managedReferenceValue = obj;
                            list.serializedProperty.serializedObject.ApplyModifiedProperties();
                            list.serializedProperty.serializedObject.Update();
                        }, t);
                    }
                }
            }
        }
    }

#endif // UNITY_EDITOR

} // namespace UnityExtensions