
using System;
using System.Collections.Generic;
using UnityExtensions;
using UnityEngine;

namespace UnityExtensions
{
    [Serializable]
    public struct Vector2Array : IWrappedArray, IEquatable<Vector2Array>, IEquatable<Vector2[]>
    {
        public Vector2[] data;
        public int length => data.Length;
        public bool isNullOrEmpty => data == null || data.Length == 0;
        public ref Vector2 this[int index] => ref data[index];
        public static implicit operator Vector2[](Vector2Array wrap) => wrap.data;
        public static implicit operator Vector2Array(Vector2[] data) => new Vector2Array { data = data };
        public static bool operator ==(Vector2Array a, Vector2Array b) => a.data == b.data;
        public static bool operator !=(Vector2Array a, Vector2Array b) => a.data != b.data;
        public static bool operator ==(Vector2Array a, Vector2[] b) => a.data == b;
        public static bool operator !=(Vector2Array a, Vector2[] b) => a.data != b;
        public static bool operator ==(Vector2[] a, Vector2Array b) => a == b.data;
        public static bool operator !=(Vector2[] a, Vector2Array b) => a != b.data;
        public override bool Equals(object obj) => (obj is Vector2Array wrap) ? data == wrap.data : data == obj;
        public bool Equals(Vector2Array other) => data == other.data;
        public bool Equals(Vector2[] other) => data == other;
        public override int GetHashCode() => data.GetHashCode();
        public Enumerator GetEnumerator() => new Enumerator(data);

        public struct Enumerator
        {
            Vector2[] _data; int _index;
            internal Enumerator(Vector2[] data) { _data = data; _index = -1; }
            public Vector2 Current => _data[_index];
            public bool MoveNext() => (++_index) < _data.Length;
            public void Reset() => _index = -1;
        }
    }

    [Serializable]
    public struct Vector2List : IWrappedList, IEquatable<Vector2List>, IEquatable<List<Vector2>>
    {
        public List<Vector2> data;
        public int count => data.Count;
        public bool isNullOrEmpty => data == null || data.Count == 0;
        public Vector2 this[int index] { get => data[index]; set => data[index] = value; }
        public static implicit operator List<Vector2>(Vector2List wrap) => wrap.data;
        public static implicit operator Vector2List(List<Vector2> data) => new Vector2List { data = data };
        public static bool operator ==(Vector2List a, Vector2List b) => a.data == b.data;
        public static bool operator !=(Vector2List a, Vector2List b) => a.data != b.data;
        public static bool operator ==(Vector2List a, List<Vector2> b) => a.data == b;
        public static bool operator !=(Vector2List a, List<Vector2> b) => a.data != b;
        public static bool operator ==(List<Vector2> a, Vector2List b) => a == b.data;
        public static bool operator !=(List<Vector2> a, Vector2List b) => a != b.data;
        public override bool Equals(object obj) => (obj is Vector2List wrap) ? data == wrap.data : data == obj;
        public bool Equals(Vector2List other) => data == other.data;
        public bool Equals(List<Vector2> other) => data == other;
        public override int GetHashCode() => data.GetHashCode();
        public List<Vector2>.Enumerator GetEnumerator() => data.GetEnumerator();
    }
}
