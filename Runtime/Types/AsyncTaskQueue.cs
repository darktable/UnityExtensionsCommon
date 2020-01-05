using System;
using System.Threading;
using System.Threading.Tasks;

namespace UnityExtensions
{
    /// <summary>
    /// Queued task
    /// </summary>
    public interface IQueuedTask<TTask> where TTask : IQueuedTask<TTask>
    {
        /// <summary>
        /// Called before adding the task, can be used to delete overdue tasks.
        /// </summary>
        /// <param name="current">The task processing currently, -1 means all tasks are done.</param>
        /// <returns>True means this task can be added to the queue. </returns>
        bool BeforeEnqueue(LinkedList<TTask> tasks, int current);

        /// <summary>
        /// Process this task. Note this is called asynchronously.
        /// </summary>
        void Process();

        /// <summary>
        /// Called after this task completed.
        /// </summary>
        void AfterComplete();
    }

    public abstract class QueuedTask<TTask> : IQueuedTask<TTask> where TTask : QueuedTask<TTask>
    {
        public virtual bool BeforeEnqueue(LinkedList<TTask> tasks, int current) => true;

        public virtual void Process() { }

        public virtual void AfterComplete() { }
    }

    /// <summary>
    /// Async task queue, support edit-mode.
    /// </summary>
    public class AsyncTaskQueue<TTask> : IDisposable where TTask : IQueuedTask<TTask>
    {
        volatile bool _working;

        LinkedList<TTask> _tasks;
        volatile int _current = -1; // -1 means all tasks are done

        AutoResetEvent _event;
        Task _taskThread;

        float _waitToDispose;       // time to auto-dispose after all task completed, negative: never, zero: immediately, positive: seconds to wait
        float _timer;               // -1: does not start

        int _taskCount = 0;


        public bool hasTask => _taskCount > 0;


        /// <summary>
        /// Enqueue a new task.
        /// </summary>
        /// <param name="waitToDispose"> time to auto-dispose after all task completed, negative: never, zero: immediately, positive: seconds to wait </param>
        public void Enqueue(TTask task, float waitToDispose)
        {
            // Initialize
            if (!_working)
            {
                _working = true;

                _tasks = new LinkedList<TTask>();
                _current = -1;

                _event = new AutoResetEvent(false);
                _taskThread = Task.Run(TaskThread);

                RuntimeUtilities.unitedUpdate += Update;
            }

            _waitToDispose = waitToDispose;
            _timer = -1;

            // add task
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
        /// Delete all unprocessed task.
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
        /// Dispose the task queue. It waits all tasks to be completed.
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