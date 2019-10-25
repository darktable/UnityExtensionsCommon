using System;
using UnityEngine;

namespace UnityExtensions
{
    /// <summary>
    /// 树节点
    /// 在内部, 一个节点的子节点被组织为双向链表
    /// </summary>
    [Serializable]
    public class TreeNode<Node> where Node : TreeNode<Node>
    {
        [SerializeReference] Node _parent;
        [SerializeReference] Node _next;
        [SerializeReference] Node _previous;        // 第一个子节点的 previous 指向链表尾部, 以便访问最后一个子节点
        [SerializeReference] Node _firstChild;


        #region Enumerable & Enumerator

        public struct ChildrenEnumerable
        {
            Node _node;

            internal ChildrenEnumerable(Node node)
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
            Node _root;

            internal ChildrenEnumerator(Node node)
            {
                _root = node;
                Current = null;
            }

            public Node Current { get; private set; }

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
            Node _node;

            internal ParentsEnumerable(Node node)
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
            Node _node;

            internal ParentsEnumerator(Node node)
            {
                _node = node;
                Current = null;
            }

            public Node Current { get; private set; }

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
            Node _node;

            internal DirectChildrenEnumerable(Node node)
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
            Node _node;

            internal DirectChildrenEnumerator(Node node)
            {
                _node = node;
                Current = null;
            }

            public Node Current { get; private set; }

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


        /// <summary>
        /// 父节点. 如果没有父节点返回 null
        /// </summary>
        public Node parent => _parent;


        /// <summary>
        /// 同层级中后一个节点. 如果此节点是最后一个则返回 null
        /// </summary>
        public Node next => _next;


        /// <summary>
        /// 同层级中前一个节点. 如果此节点是第一个则返回 null
        /// </summary>
        public Node previous => (_parent != null && _parent._firstChild == this) ? null : _previous;


        /// <summary>
        /// 第一个直接子节点. 如果没有子节点返回 null
        /// </summary>
        public Node firstChild => _firstChild;


        /// <summary>
        /// 最后一个直接子节点. 如果没有子节点返回 null
        /// </summary>
        public Node lastChild => _firstChild?._previous;


        /// <summary>
        /// 是否为根节点
        /// </summary>
        public bool isRoot => _parent == null;


        /// <summary>
        /// 是否为叶子节点
        /// </summary>
        public bool isLeaf => _firstChild == null;


        /// <summary>
        /// 直接子节点总数. O(n), n 为直接子节点数量
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
        /// 深度. 一个根节点的深度是 0. O(n), n 为节点深度
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
        /// 根节点. O(n), n 为节点深度
        /// </summary>
        public Node root
        {
            get
            {
                var node = this as Node;
                while (node._parent != null)
                {
                    node = node._parent;
                }
                return node;
            }
        }


        /// <summary>
        /// 获取一个用以 foreach 所有子节点的 Enumerable 对象 (包括自身)
        /// 注意: 在遍历过程中修改树的结构可能造成错误
        /// </summary>
        public ChildrenEnumerable children => new ChildrenEnumerable(this as Node);


        /// <summary>
        /// 获取一个用以 foreach 所有父节点的 Enumerable 对象 (包括自身)
        /// 注意: 在遍历过程中修改树的结构可能造成错误
        /// </summary>
        public ParentsEnumerable parents => new ParentsEnumerable(this as Node);


        /// <summary>
        /// 获取一个用以 foreach 所有直接子节点的 Enumerable 对象
        /// 注意: 在遍历过程中修改树的结构可能造成错误
        /// </summary>
        public DirectChildrenEnumerable directChildren => new DirectChildrenEnumerable(this as Node);


        /// <summary>
        /// 作为第一个子节点附着到一个父节点下
        /// 注意: 为性能考虑, 未检查 parent 是否存在于当前节点为根的子树中. 要启用此检查, 请添加定义: TREE_NODE_STRICT
        /// </summary>
        public void AttachAsFirst(Node parent)
        {
            InternalValidateAttaching(parent);

            _parent = parent;
            var self = this as Node;

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
        /// 作为最后一个子节点附着到一个父节点下
        /// 注意: 为性能考虑, 未检查 parent 是否存在于当前节点为根的子树中. 要启用此检查, 请添加定义: TREE_NODE_STRICT
        /// </summary>
        public void AttachAsLast(Node parent)
        {
            InternalValidateAttaching(parent);

            _parent = parent;
            var self = this as Node;

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
        /// 附着到一个父节点下的某个子节点之前
        /// 注意: 为性能考虑, 未检查 parent 是否存在于当前节点为根的子树中. 要启用此检查, 请添加定义: TREE_NODE_STRICT
        /// </summary>
        public void AttachBefore(Node parent, Node next)
        {
            InternalValidateAttaching(parent);
            parent.InternalValidateChild(next);

            _parent = parent;
            var self = this as Node;

            _previous = next._previous;
            _next = next;
            next._previous = self;

            if (parent._firstChild == next)
            {
                parent._firstChild = self;
            }
        }


        /// <summary>
        /// 附着到一个父节点下的某个子节点之后
        /// 注意: 为性能考虑, 未检查 parent 是否存在于当前节点为根的子树中. 要启用此检查, 请添加定义: TREE_NODE_STRICT
        /// </summary>
        public void AttachAfter(Node parent, Node previous)
        {
            InternalValidateAttaching(parent);
            parent.InternalValidateChild(previous);

            _parent = parent;
            var self = this as Node;

            _previous = previous;
            _next = previous._next;
            previous._next = self;
        }


        /// <summary>
        /// 从父节点脱离
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
        /// 分离所有直接子节点
        /// </summary>
        public void DetachChildren()
        {
            Node child;

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
        /// 是否存在于某个节点为根的子树中
        /// </summary>
        public bool IsChildOf(Node parent)
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }

            var node = this;
            do
            {
                if (node == parent) return true;
                node = node._parent;
            }
            while (node != null);

            return false;
        }


        #region Internal

        // 验证 Attach 操作
        void InternalValidateAttaching(Node parent)
        {
            if (_parent != null)
            {
                DetachParent();
            }
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }
#if TREE_NODE_STRICT
            if (parent.IsChildOf(this))
            {
                throw new InvalidOperationException("new parent is a child of this node");
            }
#endif
        }


        // 验证一个节点是否为 parent 的子节点
        void InternalValidateChild(Node node)
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

    } // class TreeNode<Node>

} // namespace UnityExtensions