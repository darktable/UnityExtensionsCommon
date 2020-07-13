using System;
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
        [SerializeReference] T _prevSibling;        // the previous of first child is last child
        [SerializeReference] T _nextSibling;
        [SerializeReference] T _firstChild;


        /// <summary>
        /// Parent node, return null if it does not exist.
        /// </summary>
        public T parent
        {
            get => _parent;
            set
            {
                if (value != _parent)
                {
                    if (_parent != null) DetachParent();
                    if (value != null) AsLastChild(value);
                }
            }
        }

        /// <summary>
        /// Next node in the same hierarchy, return null if this node is the last one.
        /// </summary>
        public T nextSibling => _nextSibling;

        /// <summary>
        /// Previous node in the same hierarchy, return null if this node is the first one.
        /// </summary>
        public T previousSibling => (_parent != null && _parent._firstChild == this) ? null : _prevSibling;

        /// <summary>
        /// First child node, return null if no child.
        /// </summary>
        public T firstChild => _firstChild;

        /// <summary>
        /// Last child node, return null if no child.
        /// </summary>
        public T lastChild => _firstChild?._prevSibling;

        /// <summary>
        /// Is this node a root node?
        /// </summary>
        public bool isRoot => _parent == null;

        /// <summary>
        /// Is this node a leaf node?
        /// </summary>
        public bool isLeaf => _firstChild == null;

        /// <summary>
        /// Does this node have parent?
        /// </summary>
        public bool hasParent => _parent != null;

        /// <summary>
        /// Does this node have children?
        /// </summary>
        public bool hasChildren => _firstChild != null;

        /// <summary>
        /// Number of children. Time complexity: O(n) - n is number of children.
        /// </summary>
        public int childCount
        {
            get
            {
                int n = 0;
                var node = _firstChild;
                while (node != null)
                {
                    n++;
                    node = node._nextSibling;
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
        /// Get a enumerable instance to foreach all descendants (include this node).
        /// Note: can not change the structure of this tree inside the foreach.
        /// </summary>
        public DescendantsEnumerable descendants => new DescendantsEnumerable((T)this);

        /// <summary>
        /// Get a enumerable instance to foreach all ancestors (include this node).
        /// Note: can not change the structure of this tree inside the foreach.
        /// </summary>
        public AncestorsEnumerable ancestors => new AncestorsEnumerable((T)this);

        /// <summary>
        /// Get a enumerable instance to foreach all children.
        /// Note: can not change the structure of this tree inside the foreach.
        /// </summary>
        public ChildrenEnumerable children => new ChildrenEnumerable((T)this);

        /// <summary>
        /// Attach to a specified node as the first child.
        /// </summary>
        public void AsFirstChild(T newParent)
        {
            ValidateNewParent(newParent);
            if (newParent._firstChild == this) return;
            DetachParent();

            var self = (T)this;

            _parent = newParent;

            if (newParent._firstChild != null)
            {
                _prevSibling = newParent._firstChild._prevSibling;
                _nextSibling = newParent._firstChild;
                newParent._firstChild._prevSibling = self;
            }
            else
            {
                _prevSibling = self;
            }

            newParent._firstChild = self;
        }

        /// <summary>
        /// Attach to a specified node as the last child.
        /// </summary>
        public void AsLastChild(T newParent)
        {
            ValidateNewParent(newParent);
            if (newParent.lastChild == this) return;
            DetachParent();

            var self = (T)this;

            _parent = newParent;

            if (newParent._firstChild != null)
            {
                _prevSibling = newParent._firstChild._prevSibling;
                _prevSibling._nextSibling = self;
                newParent._firstChild._prevSibling = self;
            }
            else
            {
                _prevSibling = self;
                newParent._firstChild = self;
            }
        }

        /// <summary>
        /// Attach to a parent before a specified child of it.
        /// </summary>
        public void AsSiblingBefore(T newNext)
        {
            ValidateNewSibling(newNext);
            if (this == newNext || this._nextSibling == newNext) return;

            ValidateNewParent(newNext._parent);
            DetachParent();

            var self = (T)this;

            _parent = newNext._parent;
            _prevSibling = newNext._prevSibling;
            _nextSibling = newNext;

            newNext._prevSibling = self;

            if (_parent._firstChild == newNext)
            {
                _parent._firstChild = self;
            }
            else
            {
                _prevSibling._nextSibling = self;
            }
        }

        /// <summary>
        /// Attach to a parent after a specified child of it.
        /// </summary>
        public void AsSiblingAfter(T newPrevious)
        {
            ValidateNewSibling(newPrevious);
            if (this == newPrevious || this.previousSibling == newPrevious) return;

            ValidateNewParent(newPrevious._parent);
            DetachParent();

            var self = (T)this;

            _parent = newPrevious._parent;
            _prevSibling = newPrevious;
            _nextSibling = newPrevious._nextSibling;

            newPrevious._nextSibling = self;

            if (_nextSibling != null)
            {
                _nextSibling._prevSibling = self;
            }
            else
            {
                _parent._firstChild._prevSibling = self;
            }
        }

        /// <summary>
        /// Detach from parent node.
        /// </summary>
        public void DetachParent()
        {
            if (_parent != null)
            {
                if (_nextSibling != null)
                {
                    _nextSibling._prevSibling = _prevSibling;
                }
                else
                {
                    _parent._firstChild._prevSibling = _prevSibling;
                }

                if (_parent._firstChild == this)
                {
                    _parent._firstChild = _nextSibling;
                }
                else
                {
                    _prevSibling._nextSibling = _nextSibling;
                }

                _parent = null;
                _nextSibling = null;
                _prevSibling = null;
            }
        }

        /// <summary>
        /// Detach from all children.
        /// </summary>
        public void DetachChildren()
        {
            T child = _firstChild;
            while (child != null)
            {
                child._parent = null;
                child._nextSibling = null;
                child._prevSibling = null;

                child = child._nextSibling;
            }

            _firstChild = null;
        }

        /// <summary>
        /// Is this node a descendant of a specified node?
        /// </summary>
        public bool IsDescendantOf(T root)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root is null");
            }

            var node = this;
            while (node != root)
            {
                node = node._parent;
                if (node == null) return false;
            }
            return true;
        }


        #region Internal Validate

        void ValidateNewParent(T newParent)
        {
            if (newParent == null)
            {
                throw new ArgumentNullException("new parent is null");
            }
#if DEBUG
            if (newParent.IsDescendantOf((T)this))
            {
                throw new InvalidOperationException("new parent is a descendant of this node");
            }
#endif
        }

        static void ValidateNewSibling(T newSibling)
        {
            if (newSibling == null)
            {
                throw new ArgumentNullException("new sibling is null");
            }
            if (newSibling._parent == null)
            {
                throw new InvalidOperationException("new sibling has no parent");
            }
        }

        #endregion


        #region Enumerable & Enumerator

        public struct DescendantsEnumerable
        {
            T _node;

            internal DescendantsEnumerable(T node)
            {
                _node = node;
            }

            public DescendantsEnumerator GetEnumerator()
            {
                return new DescendantsEnumerator(_node);
            }
        }

        public struct DescendantsEnumerator
        {
            T _root;

            internal DescendantsEnumerator(T node)
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
                            if (Current.nextSibling != null)
                            {
                                Current = Current._nextSibling;
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

        public struct AncestorsEnumerable
        {
            T _node;

            internal AncestorsEnumerable(T node)
            {
                _node = node;
            }

            public AncestorsEnumerator GetEnumerator()
            {
                return new AncestorsEnumerator(_node);
            }
        }

        public struct AncestorsEnumerator
        {
            T _node;

            internal AncestorsEnumerator(T node)
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
            T _node;

            internal ChildrenEnumerator(T node)
            {
                _node = node;
                Current = null;
            }

            public T Current { get; private set; }

            public bool MoveNext()
            {
                if (Current != null)
                {
                    Current = Current.nextSibling;
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