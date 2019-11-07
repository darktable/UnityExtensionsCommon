using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnityExtensions
{
    /// <summary>
    /// 异步任务监视器, 支持 Editor
    /// </summary>
    public struct TaskMonitor
    {
        struct Item
        {
            public IAsyncResult task;
            public Action<IAsyncResult> update;
            public Action<IAsyncResult> completed;
        }


        static Dictionary<IAsyncResult, int> _ids = new Dictionary<IAsyncResult, int>();
        static QuickLinkedList<Item> _items = new QuickLinkedList<Item>(4);
        
        static int _current = -1;   // 用于遍历


        /// <summary>
        /// 让 TaskMonitor 监视一个 Task, 直到其完成之前，每帧触发 update, 最后一次 update 之后还会触发 completed
        /// </summary>
        /// <param name="task"></param>
        /// <param name="completed"></param>
        /// <param name="update"></param>
        /// <returns> 是否添加成功. false 表示 Task 无效或已添加过了 </returns>
        public static bool Add(IAsyncResult task, Action<IAsyncResult> completed = null, Action<IAsyncResult> update = null)
        {
            if (task == null || _ids.ContainsKey(task)) return false;

            if (_items.count == 0) RuntimeUtilities.unitedUpdate += Update;

            int id = _items.AddFirst(new Item { task = task, completed = completed, update = update });
            _ids.Add(task, id);

            return true;
        }


        public static Task Add(Action asyncAction, Action<Task> completed = null, Action<Task> update = null)
        {
            var task = Task.Run(asyncAction);
            Add(task, t=>completed?.Invoke(t as Task), t=>update?.Invoke(t as Task));
            return task;
        }


        public static Task<TResult> Add<TResult>(Func<TResult> asyncFunc, Action<Task<TResult>> completed = null, Action<Task<TResult>> update = null)
        {
            var task = Task.Run(asyncFunc);
            Add(task, t => completed?.Invoke(t as Task<TResult>), t => update?.Invoke(t as Task<TResult>));
            return task;
        }


        /// <summary>
        /// 让 TaskMonitor 移除对 Task 的监视, 凡是被移除监视的 Task 不会再触发 update 或 completed
        /// </summary>
        /// <param name="task"></param>
        /// <returns> 是否成功移除. false 表示该 Task 没有被监视 </returns>
        public static bool Remove(IAsyncResult task)
        {
            if (!_ids.TryGetValue(task, out int id)) return false;

            if (_current == id) _current = _items.GetNext(id);

            _ids.Remove(task);
            _items.Remove(id);

            if (_items.count == 0) RuntimeUtilities.unitedUpdate -= Update;

            return true;
        }


        static void Update()
        {
            Item item;
            while (true)
            {
                // 通过这种方式遍历，以保证可在 update 和 completed 中调用 Add 或 Remove

                if (_current == -1) _current = _items.last;
                else _current = _items.GetPrevious(_current);

                if (_current >= 0)
                {
                    item = _items.GetNode(_current).value;

                    item.update?.Invoke(item.task);
                    if (item.task.IsCompleted)
                    {
                        if (Remove(item.task))
                            item.completed?.Invoke(item.task);
                    }
                }
                else return;
            }
        }

    } // struct TaskMonitor

} // namespace UnityExtensions
