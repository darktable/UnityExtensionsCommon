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

        public static implicit operator T(TempObject<T> temp) => temp.item;

        public void Dispose()
        {
            if (_pool != null)
            {
                _pool.Despawn(item);
                _pool = null;
                item = null;
            }
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

    } // class ArrayPool


    public sealed class PoolSingleton<T> : ObjectPool<T> where T : class, new()
    {
        public static readonly PoolSingleton<T> instance = new PoolSingleton<T>();

        private PoolSingleton() : base() { }
    }

    public sealed class Array1PoolSingleton<T> : ArrayPool<T>
    {
        public static readonly Array1PoolSingleton<T> instance = new Array1PoolSingleton<T>();

        private Array1PoolSingleton() : base(1) { }
    }

    public sealed class Array4PoolSingleton<T> : ArrayPool<T>
    {
        public static readonly Array4PoolSingleton<T> instance = new Array4PoolSingleton<T>();

        private Array4PoolSingleton() : base(4) { }
    }

    public sealed class Array16PoolSingleton<T> : ArrayPool<T>
    {
        public static readonly Array16PoolSingleton<T> instance = new Array16PoolSingleton<T>();

        private Array16PoolSingleton() : base(16) { }
    }

    public sealed class Array32PoolSingleton<T> : ArrayPool<T>
    {
        public static readonly Array32PoolSingleton<T> instance = new Array32PoolSingleton<T>();

        private Array32PoolSingleton() : base(32) { }
    }

    public sealed class Array64PoolSingleton<T> : ArrayPool<T>
    {
        public static readonly Array64PoolSingleton<T> instance = new Array64PoolSingleton<T>();

        private Array64PoolSingleton() : base(64) { }
    }

    public sealed class Array128PoolSingleton<T> : ArrayPool<T>
    {
        public static readonly Array128PoolSingleton<T> instance = new Array128PoolSingleton<T>();

        private Array128PoolSingleton() : base(128) { }
    }

    public sealed class Array256PoolSingleton<T> : ArrayPool<T>
    {
        public static readonly Array256PoolSingleton<T> instance = new Array256PoolSingleton<T>();

        private Array256PoolSingleton() : base(256) { }
    }

    public sealed class Array512PoolSingleton<T> : ArrayPool<T>
    {
        public static readonly Array512PoolSingleton<T> instance = new Array512PoolSingleton<T>();

        private Array512PoolSingleton() : base(512) { }
    }

    public sealed class Array1024PoolSingleton<T> : ArrayPool<T>
    {
        public static readonly Array1024PoolSingleton<T> instance = new Array1024PoolSingleton<T>();

        private Array1024PoolSingleton() : base(1024) { }
    }

} // namespace UnityExtensions