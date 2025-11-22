namespace BiliCopilot.Visor;

/// <summary>
/// 单例锁，确保只有一个服务实例运行
/// </summary>
public sealed class SingleInstanceLock : IDisposable
{
    private readonly Mutex _mutex;
    private readonly bool _hasHandle;

    public SingleInstanceLock(string mutexName)
    {
        _mutex = new Mutex(true, mutexName, out _hasHandle);
    }

    public bool IsFirstInstance => _hasHandle;

    public void Dispose()
    {
        if (_hasHandle)
        {
            _mutex.ReleaseMutex();
        }
        _mutex.Dispose();
    }
}
