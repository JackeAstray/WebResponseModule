using GameLogic.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace GameLogic.Coroutiner
{
    /// <summary>
    /// Collaborative object pool
    /// 协程对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CoroutinerPool<T> where T : class, new()
    {
        private readonly Stack<T> poolStack = new Stack<T>();
        private readonly object lockObj = new object();
        private readonly int maxCapacity;

        public CoroutinerPool(int maxCapacity)
        {
            this.maxCapacity = maxCapacity;
        }

        /// <summary>
        /// Get Object
        /// 获取对象
        /// </summary>
        /// <returns></returns>
        public T Get()
        {
            lock (lockObj)
            {
                if (poolStack.Count > 0)
                {
                    return poolStack.Pop();
                }
                else
                {
                    return new T();
                }
            }
        }

        /// <summary>
        /// Release Object
        /// 释放对象
        /// </summary>
        /// <param name="obj"></param>
        public void Release(T obj)
        {
            lock (lockObj)
            {
                if (poolStack.Count < maxCapacity)
                {
                    poolStack.Push(obj);
                }
            }
        }
    }

    /// <summary>
    /// Coroutiner Manager
    /// 协程管理器
    /// </summary>
    public class CoroutinerMgr : SingletonMgr<CoroutinerMgr>
    {
        public long taskIndex = 0;
        private readonly object taskLock = new object();
        private readonly CoroutinerPool<CoroutineTask> taskPool = new CoroutinerPool<CoroutineTask>(100);
        /// <summary>
        /// Collaborative task
        /// 协程任务
        /// </summary>
        public class CoroutineTask : IReference
        {
            public IEnumerator Enumerator;
            public Action<Coroutine> Callback;
            public long TaskId;

            public void Clear()
            {
                TaskId = -1;
                Enumerator = null;
                Callback = null;
            }
        }

        // Collaborative task
        // 协作任务
        private List<CoroutineTask> coroutineTasks = new List<CoroutineTask>();
        private List<CoroutineTask> tasksToProcess = new List<CoroutineTask>();

        void Update()
        {
            // Update task
            // 更新事件
            RefreshRoutineTasks();
        }

        /// <summary>
        /// Add routine
        /// 添加协程
        /// </summary>
        /// <param name="routine"></param>
        /// <param name="callback"></param>
        public void AddRoutine(IEnumerator routine, Action<Coroutine> callback)
        {
            var task = taskPool.Get();
            task.Enumerator = routine;
            task.Callback = callback;
            task.TaskId = Interlocked.Increment(ref taskIndex);

            lock (taskLock)
            {
                coroutineTasks.Add(task);
            }
        }

        /// <summary>
        /// Initiate the collaborative process
        /// 启动协程
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public Coroutine StartRoutine(Action handler)
        {
            return StartCoroutine(ExecuteRoutine(handler));
        }

        /// <summary>
        /// Initiate the collaborative process
        /// 启动协程
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public Coroutine StartRoutine(Action handler, Action callback)
        {
            return StartCoroutine(ExecuteRoutine(handler, callback));
        }

        /// <summary>
        /// Stop all collaborative processes
        /// 停止所有协程
        /// </summary>
        public void StopAllRoutine()
        {
            base.StopAllCoroutines();
        }

        /// <summary>
        /// Stop target collaboration
        /// 停止目标协程
        /// </summary>
        /// <param name="enumerator"></param>
        public void StopTargetRoutine(IEnumerator enumerator)
        {
            base.StopCoroutine(enumerator);
        }

        /// <summary>
        /// Stop target collaboration
        /// 停止目标协程
        /// </summary>
        /// <param name="coroutine"></param>
        public void StopTargetRoutine(Coroutine coroutine)
        {
            base.StopCoroutine(coroutine);
        }

        /// <summary>
        /// Execute routine
        /// 执行例程
        /// </summary>
        /// <param name="routine"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        IEnumerator ExecuteRoutine(Coroutine routine, Action callBack)
        {
            yield return routine;
            callBack?.Invoke();
        }

        /// <summary>
        /// Execute routine
        /// 执行例程
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        IEnumerator ExecuteRoutine(Action handler)
        {
            handler?.Invoke();
            yield return null;
        }

        /// <summary>
        /// Execute routine
        /// 执行例程
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="callack"></param>
        /// <returns></returns>
        IEnumerator ExecuteRoutine(Action handler, Action callack)
        {
            yield return StartRoutine(handler);
            callack?.Invoke();
        }

        /// <summary>
        /// Enumeration assertion coroutine
        /// 枚举断言协程
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        IEnumerator ExecutePredicateRoutine(Func<bool> handler, Action callBack)
        {
            yield return new WaitUntil(handler);
            callBack();
        }

        /// <summary>
        /// Collaborative task
        /// 协程任务
        /// </summary>
        void RefreshRoutineTasks()
        {
            lock (taskLock)
            {
                tasksToProcess.AddRange(coroutineTasks);
                coroutineTasks.Clear();
            }

            // Processing tasks
            // 处理任务
            foreach (var task in tasksToProcess)
            {
                try
                {
                    var coroutine = StartCoroutine(task.Enumerator);
                    task.Callback?.Invoke(coroutine);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Coroutine task {task.TaskId} threw an exception: {ex}");
                }
                finally
                {
                    // Cleaning task
                    // 清理任务
                    task.Clear();
                    taskPool.Release(task);
                }
            }

            tasksToProcess.Clear();
        }

        /// <summary>
        /// Cancel the collaborative task
        /// 取消协程任务
        /// </summary>
        /// <param name="taskId"></param>
        public void CancelRoutineTask(long taskId)
        {
            lock (taskLock)
            {
                var task = coroutineTasks.Find(t => t.TaskId == taskId);
                if (task != null)
                {
                    coroutineTasks.Remove(task);
                    task.Clear();
                    taskPool.Release(task);
                }
            }
        }
    }
}
