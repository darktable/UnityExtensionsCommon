
using System;
using System.Collections.Generic;
using UnityExtensions;

namespace UnityExtensions
{
    [Serializable]
    public struct FloatArray : IWrappedArray, IEquatable<FloatArray>, IEquatable<float[]>
    {
        public float[] data;
        public int length => data.Length;
        public bool isNullOrEmpty => data == null || data.Length == 0;
        public ref float this[int index] => ref data[index];
        public static implicit operator float[](FloatArray wrap) => wrap.data;
        public static implicit operator FloatArray(float[] data) => new FloatArray { data = data };
        public static bool operator ==(FloatArray a, FloatArray b) => a.data == b.data;
        public static bool operator !=(FloatArray a, FloatArray b) => a.data != b.data;
        public static bool operator ==(FloatArray a, float[] b) => a.data == b;
        public static bool operator !=(FloatArray a, float[] b) => a.data != b;
        public static bool operator ==(float[] a, FloatArray b) => a == b.data;
        public static bool operator !=(float[] a, FloatArray b) => a != b.data;
        public override bool Equals(object obj) => (obj is FloatArray wrap) ? data == wrap.data : data == obj;
        public bool Equals(FloatArray other) => data == other.data;
        public bool Equals(float[] other) => data == other;
        public override int GetHashCode() => data.GetHashCode();
        public Enumerator GetEnumerator() => new Enumerator(data);

        public struct Enumerator
        {
            float[] _data; int _index;
            internal Enumerator(float[] data) { _data = data; _index = -1; }
            public float Current => _data[_index];
            public bool MoveNext() => (++_index) < _data.Length;
            public void Reset() => _index = -1;
        }
    }

    [Serializable]
    public struct FloatList : IWrappedList, IEquatable<FloatList>, IEquatable<List<float>>
    {
        public List<float> data;
        public int count => data.Count;
        public bool isNullOrEmpty => data == null || data.Count == 0;
        public float this[int index] { get => data[index]; set => data[index] = value; }
        public static implicit operator List<float>(FloatList wrap) => wrap.data;
        public static implicit operator FloatList(List<float> data) => new FloatList { data = data };
        public static bool operator ==(FloatList a, FloatList b) => a.data == b.data;
        public static bool operator !=(FloatList a, FloatList b) => a.data != b.data;
        public static bool operator ==(FloatList a, List<float> b) => a.data == b;
        public static bool operator !=(FloatList a, List<float> b) => a.data != b;
        public static bool operator ==(List<float> a, FloatList b) => a == b.data;
        public static bool operator !=(List<float> a, FloatList b) => a != b.data;
        public override bool Equals(object obj) => (obj is FloatList wrap) ? data == wrap.data : data == obj;
        public bool Equals(FloatList other) => data == other.data;
        public bool Equals(List<float> other) => data == other;
        public override int GetHashCode() => data.GetHashCode();
        public List<float>.Enumerator GetEnumerator() => data.GetEnumerator();
    }
}
