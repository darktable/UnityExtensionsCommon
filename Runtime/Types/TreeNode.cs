﻿using System;
using UnityEngine;

namespace UnityExtensions
{
    /// <summary>
    /// TreeNode
    /// </summary>
    [Serializable]
    public class TreeNode<T> where T : TreeNode<T>
    {
        [SerializeReference] T _parent;
        [SerializeReference] T _next;
        [SerializeReference] T _previous;        // the previous of first child references the last child
        [SerializeReference] T _firstChild;


        /// <summary>
        /// Parent node, return null if it does not exist.
        /// </summary>
        public T parent => _parent;


        /// <summary>
        /// Next node in the same hierarchy, return null if this node is the last one.
        /// </summary>
        public T next => _next;


        /// <summary>
        /// Previous node in the same hierarchy, return null if this node is the first one.
        /// </summary>
        public T previous => (_parent != null && _parent._firstChild == this) ? null : _previous;


        /// <summary>
        /// First child node, return null if no child.
        /// </summary>
        public T firstChild => _firstChild;


        /// <summary>
        /// Last child node, return null if no child.
        /// </summary>
        public T lastChild => _firstChild?._previous;


        /// <summary>
        /// Is this node a root node?
        /// </summary>
        public bool isRoot => _parent == null;


        /// <summary>
        /// Is this node a leaf node?
        /// </summary>
        public bool isLeaf => _firstChild == null;


        /// <summary>
        /// Number of direct children. Time complexity: O(n) - n is number of direct children.
        /// </summary>
        public int directChildCount
        {
            get
            {
                int n = 0;
                var node = _firstChild;
                while (node != null)
                {
                    n++;
                    node = node._next;
                }
                return n;
            }
        }


        /// <summary>
        /// Depth of this node. Depth of a root node is zero. Time complexity: O(n) - n is depth of this node.
        /// </summary>
        public int depth
        {
            get
            {
                int n = 0;
                var node = _parent;
                while (node != null)
                {
                    n++;
                    node = node._parent;
                }
                return n;
            }
        }


        /// <summary>
        /// Root node of this tree. Time complexity: O(n) - n is depth of this node.
        /// </summary>
        public T root
        {
            get
            {
                var node = (T)this;
                while (node._parent != null)
                {
                    node = node._parent;
                }
                return node;
            }
        }


        /// <summary>
        /// Get a enumerable instance to foreach all children (include this node).
        /// Note: can not change the structure of this tree inside the foreach.
        /// </summary>
        public ChildrenEnumerable children => new ChildrenEnumerable((T)this);


        /// <summary>
        /// Get a enumerable instance to foreach all parents (include this node).
        /// Note: can not change the structure of this tree inside the foreach.
        /// </summary>
        public ParentsEnumerable parents => new ParentsEnumerable((T)this);


        /// <summary>
        /// Get a enumerable instance to foreach all direct children.
        /// Note: can not change the structure of this tree inside the foreach.
        /// </summary>
        public DirectChildrenEnumerable directChildren => new DirectChildrenEnumerable((T)this);


        /// <summary>
        /// Attach to a specified node as the first child.
        /// Note: Use TREE_NODE_STRICT to check if the specified node is a child of this node.
        /// </summary>
        public void AttachAsFirst(T parent)
        {
            InternalValidateAttaching(parent);

            _parent = parent;
            var self = (T)this;

            if (parent._firstChild != null)
            {
                _previous = parent._firstChild._previous;
                _next = parent._firstChild;
                parent._firstChild._previous = self;
            }
            else
            {
                _previous = self;
            }

            parent._firstChild = self;
        }


        /// <summary>
        /// Attach to a specified node as the last child.
        /// Note: Use TREE_NODE_STRICT to check if the specified node is a child of this node.
        /// </summary>
        public void AttachAsLast(T parent)
        {
            InternalValidateAttaching(parent);

            _parent = parent;
            var self = (T)this;

            if (parent._firstChild != null)
            {
                _previous = parent._firstChild._previous;
                _previous._next = self;
                parent._firstChild._previous = self;
            }
            else
            {
                _previous = self;
                parent._firstChild = self;
            }
        }


        /// <summary>
        /// Attach to a specified node before a child of it.
        /// Note: Use TREE_NODE_STRICT to check if the specified node is a child of this node.
        /// </summary>
        public void AttachBefore(T parent, T next)
        {
            InternalValidateAttaching(parent);
            parent.InternalValidateChild(next);

            _parent = parent;
            var self = (T)this;

            _previous = next._previous;
            _next = next;
            next._previous = self;

            if (parent._firstChild == next)
            {
                parent._firstChild = self;
            }
        }


        /// <summary>
        /// Attach to a specified node after a child of it.
        /// Note: Use TREE_NODE_STRICT to check if the specified node is a child of this node.
        /// </summary>
        public void AttachAfter(T parent, T previous)
        {
            InternalValidateAttaching(parent);
            parent.InternalValidateChild(previous);

            _parent = parent;
            var self = (T)this;

            _previous = previous;
            _next = previous._next;
            previous._next = self;
        }


        /// <summary>
        /// Detach from parent node.
        /// </summary>
        public void DetachParent()
        {
            if (_parent != null)
            {
                if (_parent._firstChild == this)
                {
                    _parent._firstChild = _next;
                }
                else
                {
                    _previous._next = _next;
                }

                if (_next != null) _next._previous = _previous;

                _parent = null;
                _next = null;
                _previous = null;
            }
        }


        /// <summary>
        /// Detach from all direct children.
        /// </summary>
        public void DetachChildren()
        {
            T child;

            child = _firstChild;
            while (child != null)
            {
                child._parent = null;
                child._next = null;
                child._previous = null;

                child = child._next;
            }

            _firstChild = null;
        }


        /// <summary>
        /// Is this node a child of a specified node?
        /// </summary>
        public bool IsChildOf(T parent)
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }

            var node = this;
            while (node != parent)
            {
                node = node._parent;
                if (node == null) return false;
            }
            return true;
        }


        #region Internal

        void InternalValidateAttaching(T parent)
        {
            if (_parent != null)
            {
                DetachParent();
            }
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }
#if DEBUG
            if (parent.IsChildOf((T)this))
            {
                throw new InvalidOperationException("new parent is a child of this node");
            }
#endif
        }


        void InternalValidateChild(T node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            if (node._parent != this)
            {
                throw new InvalidOperationException("node is not a child of parent");
            }
        }

        #endregion


        #region Enumerable & Enumerator

        public struct ChildrenEnumerable
        {
            T _node;

            internal ChildrenEnumerable(T node)
            {
                _node = node;
            }

            public ChildrenEnumerator GetEnumerator()
            {
                return new ChildrenEnumerator(_node);
            }
        }


        public struct ChildrenEnumerator
        {
            T _root;

            internal ChildrenEnumerator(T node)
            {
                _root = node;
                Current = null;
            }

            public T Current { get; private set; }

            public bool MoveNext()
            {
                if (Current != null)
                {
                    if (Current._firstChild != null)
                    {
                        Current = Current._firstChild;
                        return true;
                    }
                    else
                    {
                        while (Current != _root)
                        {
                            if (Current.next != null)
                            {
                                Current = Current._next;
                                return true;
                            }
                            else
                            {
                                Current = Current._parent;
                            }
                        }
                        Current = null;
                        return false;
                    }
                }
                else
                {
                    Current = _root;
                    return true;
                }
            }

            public void Reset()
            {
                Current = null;
            }
        }


        public struct ParentsEnumerable
        {
            T _node;

            internal ParentsEnumerable(T node)
            {
                _node = node;
            }

            public ParentsEnumerator GetEnumerator()
            {
                return new ParentsEnumerator(_node);
            }
        }


        public struct ParentsEnumerator
        {
            T _node;

            internal ParentsEnumerator(T node)
            {
                _node = node;
                Current = null;
            }

            public T Current { get; private set; }

            public bool MoveNext()
            {
                if (Current != null)
                {
                    Current = Current._parent;
                    return Current != null;
                }
                else
                {
                    Current = _node;
                    return true;
                }
            }

            public void Reset()
            {
                Current = null;
            }
        }


        public struct DirectChildrenEnumerable
        {
            T _node;

            internal DirectChildrenEnumerable(T node)
            {
                _node = node;
            }

            public DirectChildrenEnumerator GetEnumerator()
            {
                return new DirectChildrenEnumerator(_node);
            }
        }


        public struct DirectChildrenEnumerator
        {
            T _node;

            internal DirectChildrenEnumerator(T node)
            {
                _node = node;
                Current = null;
            }

            public T Current { get; private set; }

            public bool MoveNext()
            {
                if (Current != null)
                {
                    Current = Current.next;
                }
                else
                {
                    Current = _node._firstChild;
                }

                return Current != null;
            }

            public void Reset()
            {
                Current = null;
            }
        }

        #endregion

    } // class TreeNode<T>

} // namespace UnityExtensions