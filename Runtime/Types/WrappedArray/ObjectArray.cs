
using System;
using System.Collections.Generic;
using UnityObject = UnityEngine.Object;

namespace UnityExtensions
{

    [Serializable]
    public struct ObjectArray : IWrappedArray, IEquatable<ObjectArray>, IEquatable<UnityObject[]>
    {
        public UnityObject[] data;
        public int length => data.Length;
        public bool isNullOrEmpty => data == null || data.Length == 0;
        public ref UnityObject this[int index] => ref data[index];
        public static implicit operator UnityObject[](ObjectArray wrap) => wrap.data;
        public static implicit operator ObjectArray(UnityObject[] data) => new ObjectArray { data = data };
        public static bool operator ==(ObjectArray a, ObjectArray b) => a.data == b.data;
        public static bool operator !=(ObjectArray a, ObjectArray b) => a.data != b.data;
        public static bool operator ==(ObjectArray a, UnityObject[] b) => a.data == b;
        public static bool operator !=(ObjectArray a, UnityObject[] b) => a.data != b;
        public static bool operator ==(UnityObject[] a, ObjectArray b) => a == b.data;
        public static bool operator !=(UnityObject[] a, ObjectArray b) => a != b.data;
        public override bool Equals(object obj) => (obj is ObjectArray wrap) ? data == wrap.data : data == obj;
        public bool Equals(ObjectArray other) => data == other.data;
        public bool Equals(UnityObject[] other) => data == other;
        public override int GetHashCode() => data.GetHashCode();
    }


    [Serializable]
    public struct ObjectList : IWrappedList, IEquatable<ObjectList>, IEquatable<List<UnityObject>>
    {
        public List<UnityObject> data;
        public int count => data.Count;
        public bool isNullOrEmpty => data == null || data.Count == 0;
        public UnityObject this[int index] { get => data[index]; set => data[index] = value; }
        public static implicit operator List<UnityObject>(ObjectList wrap) => wrap.data;
        public static implicit operator ObjectList(List<UnityObject> data) => new ObjectList { data = data };
        public static bool operator ==(ObjectList a, ObjectList b) => a.data == b.data;
        public static bool operator !=(ObjectList a, ObjectList b) => a.data != b.data;
        public static bool operator ==(ObjectList a, List<UnityObject> b) => a.data == b;
        public static bool operator !=(ObjectList a, List<UnityObject> b) => a.data != b;
        public static bool operator ==(List<UnityObject> a, ObjectList b) => a == b.data;
        public static bool operator !=(List<UnityObject> a, ObjectList b) => a != b.data;
        public override bool Equals(object obj) => (obj is ObjectList wrap) ? data == wrap.data : data == obj;
        public bool Equals(ObjectList other) => data == other.data;
        public bool Equals(List<UnityObject> other) => data == other;
        public override int GetHashCode() => data.GetHashCode();
    }

}
