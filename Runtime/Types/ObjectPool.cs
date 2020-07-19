using System;
using System.Collections.Generic;

namespace UnityExtensions
{
    public struct TempObject<T> : IDisposable where T : class
    {
        public T item { get; private set; }
        BaseObjectPool<T> _pool;

        public TempObject(BaseObjectPool<T> objectPool)
        {
            item = objectPool.Spawn();
            _pool = objectPool;
        }

        public TempObject(T item)
        {
            this.item = item;
            _pool = null;
        }

        public static implicit operator T(TempObject<T> temp) => temp.item;

        public void Dispose()
        {
            if (_pool != null)
            {
                _pool.Despawn(item);
                _pool = null;
            }
            item = null;
        }
    }


    public abstract class BaseObjectPool<T> where T : class
    {
        Stack<T> _objects;

        public int count => _objects.Count;

        public BaseObjectPool(int initialQuantity = 0)
        {
            _objects = new Stack<T>(initialQuantity > 16 ? initialQuantity : 16);
            AddObjects(initialQuantity);
        }

        protected abstract T CreateInstance();

        public void AddObjects(int quantity)
        {
            while (quantity > 0)
            {
                _objects.Push(CreateInstance());
                quantity--;
            }
        }

        public T Spawn()
        {
            return _objects.Count > 0 ? _objects.Pop() : CreateInstance();
        }

        public void Despawn(T target)
        {
            _objects.Push(target);
        }

        public TempObject<T> GetTemp()
        {
            return new TempObject<T>(this);
        }

    } // class BaseObjectPool


    public class ObjectPool<T> : BaseObjectPool<T> where T : class, new()
    {
        protected override T CreateInstance() => new T();

        public ObjectPool(int initialQuantity = 0) : base(initialQuantity) { }

    } // class ObjectPool


    public class ArrayPool<T> : BaseObjectPool<T[]>
    {
        public int arrayLength { get; private set; }

        protected override T[] CreateInstance() => new T[arrayLength];

        public ArrayPool(int arrayLength, int initialQuantity = 0)
            : base(initialQuantity)
                => this.arrayLength = arrayLength;

    } // class ObjectPool

} // namespace UnityExtensions