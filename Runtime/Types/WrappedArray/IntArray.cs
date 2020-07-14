
using System;
using System.Collections.Generic;
using UnityExtensions;

namespace UnityExtensions
{
    [Serializable]
    public struct IntArray : IWrappedArray, IEquatable<IntArray>, IEquatable<int[]>
    {
        public int[] data;
        public int length => data.Length;
        public bool isNullOrEmpty => data == null || data.Length == 0;
        public ref int this[int index] => ref data[index];
        public static implicit operator int[](IntArray wrap) => wrap.data;
        public static implicit operator IntArray(int[] data) => new IntArray { data = data };
        public static bool operator ==(IntArray a, IntArray b) => a.data == b.data;
        public static bool operator !=(IntArray a, IntArray b) => a.data != b.data;
        public static bool operator ==(IntArray a, int[] b) => a.data == b;
        public static bool operator !=(IntArray a, int[] b) => a.data != b;
        public static bool operator ==(int[] a, IntArray b) => a == b.data;
        public static bool operator !=(int[] a, IntArray b) => a != b.data;
        public override bool Equals(object obj) => (obj is IntArray wrap) ? data == wrap.data : data == obj;
        public bool Equals(IntArray other) => data == other.data;
        public bool Equals(int[] other) => data == other;
        public override int GetHashCode() => data.GetHashCode();
        public Enumerator GetEnumerator() => new Enumerator(data);

        public struct Enumerator
        {
            int[] _data; int _index;
            internal Enumerator(int[] data) { _data = data; _index = -1; }
            public int Current => _data[_index];
            public bool MoveNext() => (++_index) < _data.Length;
            public void Reset() => _index = -1;
        }
    }

    [Serializable]
    public struct IntList : IWrappedArray, IEquatable<IntList>, IEquatable<List<int>>
    {
        public List<int> data;
        public int count => data.Count;
        public bool isNullOrEmpty => data == null || data.Count == 0;
        public int this[int index] { get => data[index]; set => data[index] = value; }
        public static implicit operator List<int>(IntList wrap) => wrap.data;
        public static implicit operator IntList(List<int> data) => new IntList { data = data };
        public static bool operator ==(IntList a, IntList b) => a.data == b.data;
        public static bool operator !=(IntList a, IntList b) => a.data != b.data;
        public static bool operator ==(IntList a, List<int> b) => a.data == b;
        public static bool operator !=(IntList a, List<int> b) => a.data != b;
        public static bool operator ==(List<int> a, IntList b) => a == b.data;
        public static bool operator !=(List<int> a, IntList b) => a != b.data;
        public override bool Equals(object obj) => (obj is IntList wrap) ? data == wrap.data : data == obj;
        public bool Equals(IntList other) => data == other.data;
        public bool Equals(List<int> other) => data == other;
        public override int GetHashCode() => data.GetHashCode();
        public List<int>.Enumerator GetEnumerator() => data.GetEnumerator();
    }
}
