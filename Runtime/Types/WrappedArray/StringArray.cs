
using System;
using System.Collections.Generic;
using UnityExtensions;

namespace UnityExtensions
{
    [Serializable]
    public struct StringArray : IWrappedArray, IEquatable<StringArray>, IEquatable<string[]>
    {
        public string[] data;
        public int length => data.Length;
        public bool isNullOrEmpty => data == null || data.Length == 0;
        public ref string this[int index] => ref data[index];
        public static implicit operator string[](StringArray wrap) => wrap.data;
        public static implicit operator StringArray(string[] data) => new StringArray { data = data };
        public static bool operator ==(StringArray a, StringArray b) => a.data == b.data;
        public static bool operator !=(StringArray a, StringArray b) => a.data != b.data;
        public static bool operator ==(StringArray a, string[] b) => a.data == b;
        public static bool operator !=(StringArray a, string[] b) => a.data != b;
        public static bool operator ==(string[] a, StringArray b) => a == b.data;
        public static bool operator !=(string[] a, StringArray b) => a != b.data;
        public override bool Equals(object obj) => (obj is StringArray wrap) ? data == wrap.data : data == obj;
        public bool Equals(StringArray other) => data == other.data;
        public bool Equals(string[] other) => data == other;
        public override int GetHashCode() => data.GetHashCode();
        public Enumerator GetEnumerator() => new Enumerator(data);

        public struct Enumerator
        {
            string[] _data; int _index;
            internal Enumerator(string[] data) { _data = data; _index = -1; }
            public string Current => _data[_index];
            public bool MoveNext() => (++_index) < _data.Length;
            public void Reset() => _index = -1;
        }
    }

    [Serializable]
    public struct StringList : IWrappedArray, IEquatable<StringList>, IEquatable<List<string>>
    {
        public List<string> data;
        public int count => data.Count;
        public bool isNullOrEmpty => data == null || data.Count == 0;
        public string this[int index] { get => data[index]; set => data[index] = value; }
        public static implicit operator List<string>(StringList wrap) => wrap.data;
        public static implicit operator StringList(List<string> data) => new StringList { data = data };
        public static bool operator ==(StringList a, StringList b) => a.data == b.data;
        public static bool operator !=(StringList a, StringList b) => a.data != b.data;
        public static bool operator ==(StringList a, List<string> b) => a.data == b;
        public static bool operator !=(StringList a, List<string> b) => a.data != b;
        public static bool operator ==(List<string> a, StringList b) => a == b.data;
        public static bool operator !=(List<string> a, StringList b) => a != b.data;
        public override bool Equals(object obj) => (obj is StringList wrap) ? data == wrap.data : data == obj;
        public bool Equals(StringList other) => data == other.data;
        public bool Equals(List<string> other) => data == other;
        public override int GetHashCode() => data.GetHashCode();
        public List<string>.Enumerator GetEnumerator() => data.GetEnumerator();
    }
}
