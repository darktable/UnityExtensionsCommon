
using System;
using System.Collections.Generic;
using UnityExtensions;
using UnityEngine;

namespace UnityExtensions
{
    [Serializable]
    public struct Vector3Array : IWrappedArray, IEquatable<Vector3Array>, IEquatable<Vector3[]>
    {
        public Vector3[] data;
        public int length => data.Length;
        public bool isNullOrEmpty => data == null || data.Length == 0;
        public ref Vector3 this[int index] => ref data[index];
        public static implicit operator Vector3[](Vector3Array wrap) => wrap.data;
        public static implicit operator Vector3Array(Vector3[] data) => new Vector3Array { data = data };
        public static bool operator ==(Vector3Array a, Vector3Array b) => a.data == b.data;
        public static bool operator !=(Vector3Array a, Vector3Array b) => a.data != b.data;
        public static bool operator ==(Vector3Array a, Vector3[] b) => a.data == b;
        public static bool operator !=(Vector3Array a, Vector3[] b) => a.data != b;
        public static bool operator ==(Vector3[] a, Vector3Array b) => a == b.data;
        public static bool operator !=(Vector3[] a, Vector3Array b) => a != b.data;
        public override bool Equals(object obj) => (obj is Vector3Array wrap) ? data == wrap.data : data == obj;
        public bool Equals(Vector3Array other) => data == other.data;
        public bool Equals(Vector3[] other) => data == other;
        public override int GetHashCode() => data.GetHashCode();
        public Enumerator GetEnumerator() => new Enumerator(data);

        public struct Enumerator
        {
            Vector3[] _data; int _index;
            internal Enumerator(Vector3[] data) { _data = data; _index = -1; }
            public Vector3 Current => _data[_index];
            public bool MoveNext() => (++_index) < _data.Length;
            public void Reset() => _index = -1;
        }
    }

    [Serializable]
    public struct Vector3List : IWrappedList, IEquatable<Vector3List>, IEquatable<List<Vector3>>
    {
        public List<Vector3> data;
        public int count => data.Count;
        public bool isNullOrEmpty => data == null || data.Count == 0;
        public Vector3 this[int index] { get => data[index]; set => data[index] = value; }
        public static implicit operator List<Vector3>(Vector3List wrap) => wrap.data;
        public static implicit operator Vector3List(List<Vector3> data) => new Vector3List { data = data };
        public static bool operator ==(Vector3List a, Vector3List b) => a.data == b.data;
        public static bool operator !=(Vector3List a, Vector3List b) => a.data != b.data;
        public static bool operator ==(Vector3List a, List<Vector3> b) => a.data == b;
        public static bool operator !=(Vector3List a, List<Vector3> b) => a.data != b;
        public static bool operator ==(List<Vector3> a, Vector3List b) => a == b.data;
        public static bool operator !=(List<Vector3> a, Vector3List b) => a != b.data;
        public override bool Equals(object obj) => (obj is Vector3List wrap) ? data == wrap.data : data == obj;
        public bool Equals(Vector3List other) => data == other.data;
        public bool Equals(List<Vector3> other) => data == other;
        public override int GetHashCode() => data.GetHashCode();
        public List<Vector3>.Enumerator GetEnumerator() => data.GetEnumerator();
    }
}
