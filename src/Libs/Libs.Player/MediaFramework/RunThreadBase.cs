// Copyright (c) Bili Copilot. All rights reserved.

using System.Threading;
using Bili.Copilot.Libs.Player.Enums;
using Bili.Copilot.Libs.Player.Misc;
using CommunityToolkit.Mvvm.ComponentModel;

using static Bili.Copilot.Libs.Player.Misc.Logger;

namespace Bili.Copilot.Libs.Player.MediaFramework;

/// <summary>
/// 运行线程基类.
/// </summary>
public abstract class RunThreadBase : ObservableObject
{
    private ThreadStatus _status = ThreadStatus.Stopped;
    private string _threadName;

    /// <summary>
    /// 初始化 <see cref="RunThreadBase"/> 类的新实例.
    /// </summary>
    /// <param name="uniqueId">标识符.</param>
    public RunThreadBase(int uniqueId = -1)
        => UniqueId = uniqueId == -1 ? Utils.GetUniqueId() : uniqueId;

    /// <summary>
    /// 线程状态.
    /// </summary>
    public ThreadStatus Status
    {
        get => _status;
        set
        {
            lock (LockStatus)
            {
                if (CanDebug
                    && _status != ThreadStatus.QueueFull
                    && value != ThreadStatus.QueueFull
                    && _status != ThreadStatus.QueueEmpty
                    && value != ThreadStatus.QueueEmpty)
                {
                    Log.Debug($"{_status} -> {value}");
                }

                _status = value;
            }
        }
    }

    /// <summary>
    /// 是否正在运行.
    /// </summary>
    public bool IsRunning
    {
        get
        {
            var ret = false;
            lock (LockStatus)
            {
                ret = Thread != null && Thread.IsAlive && Status != ThreadStatus.Paused;
            }

            return ret;
        }
    }

    /// <summary>
    /// 是否为关键区域.
    /// </summary>
    public bool CriticalArea { get; protected set; }

    /// <summary>
    /// 是否已释放.
    /// </summary>
    public bool Disposed { get; protected set; } = true;

    /// <summary>
    /// 唯一标识符.
    /// </summary>
    public int UniqueId { get; protected set; } = -1;

    /// <summary>
    /// 当队列满时是否暂停.
    /// </summary>
    public bool PauseOnQueueFull { get; set; }

    internal object LockActions { get; } = new();

    internal object LockStatus { get; } = new();

    internal LogHandler Log { get; set; }

    /// <summary>
    /// 线程名称.
    /// </summary>
    protected string ThreadName
    {
        get => _threadName;
        set
        {
            _threadName = value;
            Log = new LogHandler(("[#" + UniqueId + "]").PadRight(8, ' ') + $" [{ThreadName}] ");
        }
    }

    /// <summary>
    /// 线程实例.
    /// </summary>
    protected Thread Thread { get; set; }

    /// <summary>
    /// 线程的自动重置事件对象.
    /// </summary>
    protected AutoResetEvent ThreadARE { get; set; } = new(false);

    /// <summary>
    /// 暂停线程.
    /// </summary>
    public void Pause()
    {
        lock (LockActions)
        {
            lock (LockStatus)
            {
                PauseOnQueueFull = false;

                if (Disposed
                    || Thread == null
                    || !Thread.IsAlive
                    || Status == ThreadStatus.Stopping
                    || Status == ThreadStatus.Stopped
                    || Status == ThreadStatus.Ended
                    || Status == ThreadStatus.Pausing
                    || Status == ThreadStatus.Paused)
                {
                    return;
                }

                Status = ThreadStatus.Pausing;
            }

            while (Status == ThreadStatus.Pausing)
            {
                Thread.Sleep(5);
            }
        }
    }

    /// <summary>
    /// 启动线程.
    /// </summary>
    public void Start()
    {
        lock (LockActions)
        {
            var retries = 1;
            while (Thread != null && Thread.IsAlive && CriticalArea)
            {
                if (CanTrace)
                {
                    Log.Trace($"Start Retry {retries}/5");
                }

                Thread.Sleep(20);
                retries++;
                if (retries > 5)
                {
                    return;
                }
            }

            lock (LockStatus)
            {
                if (Disposed)
                {
                    return;
                }

                PauseOnQueueFull = false;

                if (Status == ThreadStatus.Draining)
                {
                    while (Status != ThreadStatus.Draining)
                    {
                        Thread.Sleep(3);
                    }
                }

                if (Status == ThreadStatus.Stopping)
                {
                    while (Status != ThreadStatus.Stopping)
                    {
                        Thread.Sleep(3);
                    }
                }

                if (Status == ThreadStatus.Pausing)
                {
                    while (Status != ThreadStatus.Pausing)
                    {
                        Thread.Sleep(3);
                    }
                }

                if (Status == ThreadStatus.Ended)
                {
                    return;
                }

                if (Status == ThreadStatus.Paused)
                {
                    ThreadARE.Set();
                    while (Status == ThreadStatus.Paused)
                    {
                        Thread.Sleep(3);
                    }

                    return;
                }

                if (Thread != null && Thread.IsAlive)
                {
                    return; // might re-check CriticalArea
                }

                Thread = new Thread(() => Run());
                Status = ThreadStatus.Running;

                Thread.Name = $"[#{UniqueId}] [{ThreadName}]";
                Thread.IsBackground = true;
                Thread.Start();
                while (!Thread.IsAlive)
                {
                    if (CanTrace)
                    {
                        Log.Trace("Waiting thread to come up");
                    }

                    Thread.Sleep(3);
                }
            }
        }
    }

    /// <summary>
    /// 停止线程.
    /// </summary>
    public void Stop()
    {
        lock (LockActions)
        {
            lock (LockStatus)
            {
                PauseOnQueueFull = false;

                if (Disposed || Thread == null || !Thread.IsAlive || Status == ThreadStatus.Stopping || Status == ThreadStatus.Stopped || Status == ThreadStatus.Ended)
                {
                    return;
                }

                if (Status == ThreadStatus.Pausing)
                {
                    while (Status != ThreadStatus.Pausing)
                    {
                        Thread.Sleep(3);
                    }
                }

                Status = ThreadStatus.Stopping;
                ThreadARE.Set();
            }

            while (Status == ThreadStatus.Stopping && Thread != null && Thread.IsAlive)
            {
                Thread.Sleep(5);
            }
        }
    }

    /// <summary>
    /// 运行线程.
    /// </summary>
    protected void Run()
    {
        if (CanDebug)
        {
            Log.Debug($"Thread started ({Status})");
        }

        do
        {
            RunInternal();

            if (Status == ThreadStatus.Pausing)
            {
                ThreadARE.Reset();
                Status = ThreadStatus.Paused;
                ThreadARE.WaitOne();
                if (Status == ThreadStatus.Paused)
                {
                    if (CanDebug)
                    {
                        Log.Debug($"{_status} -> {ThreadStatus.Running}");
                    }

                    _status = ThreadStatus.Running;
                }
            }
        }
        while (Status == ThreadStatus.Running);

        if (Status != ThreadStatus.Ended)
        {
            Status = ThreadStatus.Stopped;
        }

        if (CanDebug)
        {
            Log.Debug($"Thread stopped ({Status})");
        }
    }

    /// <summary>
    /// 内部运行方法.
    /// </summary>
    protected abstract void RunInternal();
}
