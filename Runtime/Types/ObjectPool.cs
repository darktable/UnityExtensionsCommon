using System;
using System.Collections.Generic;

namespace UnityExtensions
{
    public abstract class BaseObjectPool<T> where T : class
    {
        Stack<T> _objects;


        public int count => _objects.Count;


        public BaseObjectPool(int preallocateCount = 0)
        {
            _objects = new Stack<T>(preallocateCount > 16 ? preallocateCount : 16);
            AddObjects(preallocateCount);
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
            if (_objects.Count > 0) return _objects.Pop();
            return CreateInstance();
        }


        public void Despawn(T target)
        {
            _objects.Push(target);
        }


        public TempObject GetTemp()
        {
            return new TempObject(this);
        }


        public struct TempObject : IDisposable
        {
            public T item { get; private set; }
            BaseObjectPool<T> _pool;

            public TempObject(BaseObjectPool<T> objectPool)
            {
                item = objectPool.Spawn();
                _pool = objectPool;
            }

            public static implicit operator T(TempObject temp) => temp.item;

            void IDisposable.Dispose()
            {
                _pool.Despawn(item);
                item = null;
                _pool = null;
            }
        }

    } // class BaseObjectPool

    public class ObjectPool<T> : BaseObjectPool<T> where T : class, new()
    {
        protected override T CreateInstance() => new T();

        public ObjectPool(int preallocateCount = 0) : base(preallocateCount) { }

    } // class ObjectPool

} // namespace UnityExtensions