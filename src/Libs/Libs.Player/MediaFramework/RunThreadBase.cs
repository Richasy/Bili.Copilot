// Copyright (c) Bili Copilot. All rights reserved.

using System.Threading;
using Bili.Copilot.Libs.Player.Enums;
using Bili.Copilot.Libs.Player.Models;

namespace Bili.Copilot.Libs.Player.MediaFramework;

/// <summary>
/// 运行线程基类.
/// </summary>
public abstract class RunThreadBase : ObservableObject
{
    private ThreadStatus _status = ThreadStatus.Stopped;

    public ThreadStatus Status
    {
        get => _status;
        set
        {
            lock (_lockStatus)
            {
                if (CanDebug
                    && _status != Status.QueueFull
                    && value != Status.QueueFull
                    && _status != Status.QueueEmpty
                    && value != Status.QueueEmpty)
                {
                    Log.Debug($"{_status} -> {value}");
                }

                _status = value;
            }
        }
    }

    public bool IsRunning
    {
        get
        {
            var ret = false;
            lock (_lockStatus)
            {
                ret = Thread != null && Thread.IsAlive && Status != Status.Paused;
            }

            return ret;
        }
    }

    public bool CriticalArea { get; protected set; }

    public bool Disposed { get; protected set; } = true;

    public int UniqueId { get; protected set; } = -1;

    public bool PauseOnQueueFull { get; set; }

    protected Thread Thread;

    protected AutoResetEvent ThreadARE = new(false);

    protected string ThreadName
    {
        get => _threadName;
        set
        {
            _threadName = value;
            Log = new LogHandler(("[#" + UniqueId + "]").PadRight(8, ' ') + $" [{ThreadName}] ");
        }
    }

    private string _threadName;

    internal LogHandler Log;

    internal object _lockActions = new();
    internal object _lockStatus = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="RunThreadBase"/> class.
    /// </summary>
    /// <param name="uniqueId">标识符.</param>
    public RunThreadBase(int uniqueId = -1)
        => UniqueId = uniqueId == -1 ? Utils.GetUniqueId() : uniqueId;

    public void Pause()
    {
        lock (_lockActions)
        {
            lock (_lockStatus)
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

    public void Start()
    {
        lock (_lockActions)
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

            lock (_lockStatus)
            {
                if (Disposed)
                {
                    return;
                }

                PauseOnQueueFull = false;

                if (Status == Status.Draining)
                {
                    while (Status != Status.Draining)
                    {
                        Thread.Sleep(3);
                    }
                }

                if (Status == Status.Stopping)
                {
                    while (Status != Status.Stopping)
                    {
                        Thread.Sleep(3);
                    }
                }

                if (Status == Status.Pausing)
                {
                    while (Status != Status.Pausing)
                    {
                        Thread.Sleep(3);
                    }
                }

                if (Status == Status.Ended)
                {
                    return;
                }

                if (Status == Status.Paused)
                {
                    ThreadARE.Set();
                    while (Status == Status.Paused)
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
                Status = Status.Running;

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

    public void Stop()
    {
        lock (_lockActions)
        {
            lock (_lockStatus)
            {
                PauseOnQueueFull = false;

                if (Disposed || Thread == null || !Thread.IsAlive || Status == Status.Stopping || Status == Status.Stopped || Status == Status.Ended)
                {
                    return;
                }

                if (Status == Status.Pausing)
                {
                    while (Status != Status.Pausing)
                    {
                        Thread.Sleep(3);
                    }
                }

                Status = Status.Stopping;
                ThreadARE.Set();
            }

            while (Status == Status.Stopping && Thread != null && Thread.IsAlive)
            {
                Thread.Sleep(5);
            }
        }
    }

    protected void Run()
    {
        if (CanDebug)
        {
            Log.Debug($"Thread started ({Status})");
        }

        do
        {
            RunInternal();

            if (Status == Status.Pausing)
            {
                ThreadARE.Reset();
                Status = Status.Paused;
                ThreadARE.WaitOne();
                if (Status == Status.Paused)
                {
                    if (CanDebug)
                    {
                        Log.Debug($"{_status} -> {Status.Running}");
                    }

                    _status = Status.Running;
                }
            }
        }
        while (Status == Status.Running);

        if (Status != Status.Ended)
        {
            Status = Status.Stopped;
        }

        if (CanDebug)
        {
            Log.Debug($"Thread stopped ({Status})");
        }
    }

    protected abstract void RunInternal();
}
