
using System;
using System.Collections.Generic;
using UnityExtensions;
using UnityEngine;

namespace UnityExtensions
{
    [Serializable]
    public struct ColorArray : IWrappedArray, IEquatable<ColorArray>, IEquatable<Color[]>
    {
        public Color[] data;
        public int length => data.Length;
        public bool isNullOrEmpty => data == null || data.Length == 0;
        public ref Color this[int index] => ref data[index];
        public static implicit operator Color[](ColorArray wrap) => wrap.data;
        public static implicit operator ColorArray(Color[] data) => new ColorArray { data = data };
        public static bool operator ==(ColorArray a, ColorArray b) => a.data == b.data;
        public static bool operator !=(ColorArray a, ColorArray b) => a.data != b.data;
        public static bool operator ==(ColorArray a, Color[] b) => a.data == b;
        public static bool operator !=(ColorArray a, Color[] b) => a.data != b;
        public static bool operator ==(Color[] a, ColorArray b) => a == b.data;
        public static bool operator !=(Color[] a, ColorArray b) => a != b.data;
        public override bool Equals(object obj) => (obj is ColorArray wrap) ? data == wrap.data : data == obj;
        public bool Equals(ColorArray other) => data == other.data;
        public bool Equals(Color[] other) => data == other;
        public override int GetHashCode() => data.GetHashCode();
        public Enumerator GetEnumerator() => new Enumerator(data);

        public struct Enumerator
        {
            Color[] _data; int _index;
            internal Enumerator(Color[] data) { _data = data; _index = -1; }
            public Color Current => _data[_index];
            public bool MoveNext() => (++_index) < _data.Length;
            public void Reset() => _index = -1;
        }
    }

    [Serializable]
    public struct ColorList : IWrappedList, IEquatable<ColorList>, IEquatable<List<Color>>
    {
        public List<Color> data;
        public int count => data.Count;
        public bool isNullOrEmpty => data == null || data.Count == 0;
        public Color this[int index] { get => data[index]; set => data[index] = value; }
        public static implicit operator List<Color>(ColorList wrap) => wrap.data;
        public static implicit operator ColorList(List<Color> data) => new ColorList { data = data };
        public static bool operator ==(ColorList a, ColorList b) => a.data == b.data;
        public static bool operator !=(ColorList a, ColorList b) => a.data != b.data;
        public static bool operator ==(ColorList a, List<Color> b) => a.data == b;
        public static bool operator !=(ColorList a, List<Color> b) => a.data != b;
        public static bool operator ==(List<Color> a, ColorList b) => a == b.data;
        public static bool operator !=(List<Color> a, ColorList b) => a != b.data;
        public override bool Equals(object obj) => (obj is ColorList wrap) ? data == wrap.data : data == obj;
        public bool Equals(ColorList other) => data == other.data;
        public bool Equals(List<Color> other) => data == other;
        public override int GetHashCode() => data.GetHashCode();
        public List<Color>.Enumerator GetEnumerator() => data.GetEnumerator();
    }
}
