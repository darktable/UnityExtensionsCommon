using System;
using System.Threading;
using System.Threading.Tasks;

namespace UnityExtensions
{
    /// <summary>
    /// 队列化任务
    /// </summary>
    /// <typeparam name="TTask"></typeparam>
    public interface IQueuedTask<TTask> where TTask : IQueuedTask<TTask>
    {
        /// <summary>
        /// 在添加任务之前触发，可以用来标记或删除过期任务
        /// current 可能为 -1，表示任务都处理完了
        /// </summary>
        bool BeforeEnqueue(QuickLinkedList<TTask> tasks, int current);


        /// <summary>
        /// 处理任务，注意这可能是并行的
        /// </summary>
        void Process();


        /// <summary>
        /// 任务完成时触发
        /// </summary>
        void AfterComplete();
    }


    /// <summary>
    /// 泛型默认实现
    /// </summary>
    public abstract class QueuedTask<TTask> : IQueuedTask<TTask> where TTask : QueuedTask<TTask>
    {
        /// <summary>
        /// 在添加任务之前触发，可以用来标记或删除过期任务
        /// current 可能为 -1，表示任务都处理完了
        /// </summary>
        public virtual bool BeforeEnqueue(QuickLinkedList<TTask> tasks, int current)
        {
            return true;
        }


        /// <summary>
        /// 处理任务，注意这可能是并行的
        /// </summary>
        public virtual void Process()
        {
        }


        /// <summary>
        /// 任务完成时触发
        /// </summary>
        public virtual void AfterComplete()
        {
        }
    }


    /// <summary>
    /// 默认实现
    /// </summary>
    public abstract class QueuedTask : QueuedTask<QueuedTask>
    {
    }


    /// <summary>
    /// 异步任务队列. 支持 Editor 环境
    /// 添加任务时自动激活，完成任务后如果超过一定时间没有新的任务会自动停止
    /// 也可以通过调用 Dispose 以手动停止
    /// </summary>
    public class AsyncTaskQueue<TTask> : IDisposable where TTask : IQueuedTask<TTask>
    {
        volatile bool _working;

        QuickLinkedList<TTask> _tasks;
        volatile int _current;      // 正在进行的任务, -1 表示做完了任务

        AutoResetEvent _event;
        Task _taskThread;

        float _waitToDispose;      // 完成任务后到自动停止的时间, 负值: 永不停止；0：立即停止；正值：等待停止的时间
        float _timer;              // -1：尚未开始计时

        int _taskCount;


        public bool hasTask => _taskCount > 0;


        /// <summary>
        /// 添加一个新任务
        /// </summary>
        /// <param name="task"></param>
        /// <param name="waitToDispose"> 完成任务后到自动停止的时间, 负值永不停止；0立即停止；正值等待停止的秒数 </param>
        public void Enqueue(TTask task, float waitToDispose)
        {
            // 初始化
            if (!_working)
            {
                _working = true;

                _tasks = new QuickLinkedList<TTask>();
                _current = -1;

                _event = new AutoResetEvent(false);
                _taskThread = Task.Run(TaskThread);

                RuntimeUtilities.unitedUpdate += Update;
            }

            _waitToDispose = waitToDispose;
            _timer = -1;

            // 添加任务
            lock (_tasks)
            {
                if (!task.BeforeEnqueue(_tasks, _current))
                {
                    return;
                }

                if (_current >= 0) _tasks.AddLast(task);
                else _current = _tasks.AddLast(task);

                _taskCount++;
            }

            _event.Set();
        }


        public void ForEach(Action<TTask> action)
        {
            if (_working)
            {
                lock (_tasks)
                {
                    foreach (var task in _tasks)
                    {
                        action.Invoke(task);
                    }
                }
            }
        }


        /// <summary>
        /// 删除所有未处理的任务
        /// </summary>
        public void ClearUnprocessed()
        {
            if (_working)
            {
                lock (_tasks)
                {
                    if (_current >= 0)
                    {
                        int id = _tasks.GetNext(_current);
                        while (id >= 0)
                        {
                            int next = _tasks.GetNext(id);

                            _tasks.Remove(id);
                            _taskCount--;

                            id = next;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 释放之前会等待所有任务完成。如果希望尽快结束，可以先调用 ClearUnprocessed
        /// </summary>
        public void Dispose()
        {
            if (_working)
            {
                _working = false;

                _event.Set();
                _taskThread.Wait();

                if (_tasks.count > 0) Update();

                _taskThread.Dispose();
                _taskThread = null;

                _event.Dispose();
                _event = null;

                _tasks = null;
                _current = -1;

                RuntimeUtilities.unitedUpdate -= Update;
            }
        }


        void Update()
        {
            if (_timer < 0)
            {
                lock (_tasks)
                {
                    int id = _tasks.first;
                    while (id != _current)
                    {
                        int next = _tasks.GetNext(id);
                        var task = _tasks[id];

                        _tasks.Remove(id);
                        _taskCount--;
                        task.AfterComplete();

                        id = next;
                    }

                    if (_taskCount == 0)
                    {
                        // 完成所有任务开始计时
                        if (_waitToDispose == 0) Dispose();
                        else _timer = 0;
                    }
                }
            }
            else if (_waitToDispose > 0)
            {
                _timer += RuntimeUtilities.unitedUnscaledDeltaTime;
                if (_timer > _waitToDispose) Dispose();
            }
        }


        void TaskThread()
        {
            while (true)
            {
                if (_current >= 0)
                {
                    TTask task;
                    lock (_tasks)
                    {
                        task = _tasks[_current];
                    }

                    task.Process();

                    lock (_tasks)
                    {
                        _current = _tasks.GetNext(_current);
                    }
                }
                else if (!_working)
                {
                    return;
                }
                else
                {
                    _event.WaitOne();
                }
            }
        }

    } // struct AsyncTaskQueue<TTask>

} // namespace UnityExtensions