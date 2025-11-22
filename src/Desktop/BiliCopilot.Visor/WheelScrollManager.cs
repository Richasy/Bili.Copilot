using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BiliCopilot.Visor;

/// <summary>
/// 鼠标滚轮速度管理器
/// </summary>
public sealed class WheelScrollManager : IDisposable
{
    private int _originalSpeed;
    private int _expectedSpeed;
    private readonly Timer _timer;
    private int? _currentSpeed;
    private bool _isInitialized;
    private DateTime _lastProcessDetectionTime = DateTime.MinValue;
    private readonly TimeSpan _autoExitTimeout = TimeSpan.FromMinutes(5);

    /// <summary>
    /// 是否启用滚动加速
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    public WheelScrollManager()
    {
        _timer = new Timer(CheckAndAdjust, null, Timeout.Infinite, Timeout.Infinite);
    }

    /// <summary>
    /// 初始化管理器
    /// </summary>
    public void Initialize(int originalSpeed, int expectedSpeed)
    {
        _originalSpeed = originalSpeed;
        _expectedSpeed = expectedSpeed;
        _isInitialized = true;
        _timer.Change(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
    }

    /// <summary>
    /// 更新速度设置
    /// </summary>
    public void UpdateSpeed(int originalSpeed, int expectedSpeed)
    {
        _originalSpeed = originalSpeed;
        _expectedSpeed = expectedSpeed;
    }

    /// <summary>
    /// 检查并调整滚动速度
    /// </summary>
    private void CheckAndAdjust(object? state)
    {
        if (!_isInitialized)
        {
            return;
        }

        try
        {
            var rodelProcesses = Process.GetProcesses()
                .Where(p => p.ProcessName.Equals("BiliCopilot.UI", StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (rodelProcesses.Count == 0)
            {
                // 没有 BiliCopilot 进程
                EnsureSpeed(_originalSpeed);

                // 检查是否需要自动退出
                if (_lastProcessDetectionTime == DateTime.MinValue)
                {
                    // 首次检测到没有进程，开始计时
                    _lastProcessDetectionTime = DateTime.Now;
                    Console.WriteLine($"No BiliCopilot processes found. Will auto-exit in {_autoExitTimeout.TotalMinutes} minutes if not detected.");
                }
                else
                {
                    // 检查是否超过 5 分钟
                    var elapsed = DateTime.Now - _lastProcessDetectionTime;
                    if (elapsed >= _autoExitTimeout)
                    {
                        Console.WriteLine($"No BiliCopilot processes detected for {_autoExitTimeout.TotalMinutes} minutes. Exiting...");
                        Environment.Exit(0);
                    }
                    else
                    {
                        var remaining = _autoExitTimeout - elapsed;
                        Console.WriteLine($"No process. Time until auto-exit: {remaining.TotalSeconds:F0} seconds.");
                    }
                }

                return;
            }

            // 检测到进程，重置计时器
            if (_lastProcessDetectionTime != DateTime.MinValue)
            {
                Console.WriteLine("BiliCopilot process detected. Reset auto-exit timer.");
                _lastProcessDetectionTime = DateTime.MinValue;
            }

            // 如果未启用，始终保持原始速度
            if (!IsEnabled)
            {
                EnsureSpeed(_originalSpeed);
                return;
            }

            // 检查是否满足加速条件：窗口有焦点且鼠标在窗口内
            var shouldAccelerate = ShouldAccelerateScroll(rodelProcesses);

            if (shouldAccelerate)
            {
                // 窗口有焦点且鼠标在窗口内，设置为预期速度
                EnsureSpeed(_expectedSpeed);
            }
            else
            {
                // 不满足条件，恢复到初始速度
                EnsureSpeed(_originalSpeed);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CheckAndAdjust: {ex.Message}");
        }
    }

    /// <summary>
    /// 检查是否应该加速滚动：窗口有焦点且鼠标在窗口内
    /// </summary>
    private static bool ShouldAccelerateScroll(List<Process> processes)
    {
        // 获取当前前台窗口
        var foregroundWindow = GetForegroundWindow();
        if (foregroundWindow == IntPtr.Zero)
        {
            return false;
        }

        // 获取鼠标位置
        if (!GetCursorPos(out var cursorPos))
        {
            return false;
        }

        foreach (var process in processes)
        {
            try
            {
                if (process.MainWindowHandle != IntPtr.Zero)
                {
                    // 检查窗口是否可见
                    if (IsWindowVisible(process.MainWindowHandle))
                    {
                        // 检查窗口或其子窗口是否有焦点
                        if (IsWindowOrChildFocused(process.MainWindowHandle, foregroundWindow))
                        {
                            // 检查鼠标是否在窗口内
                            if (IsCursorInWindow(process.MainWindowHandle, cursorPos))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            catch
            {
                // 进程可能已经退出或窗口已关闭
            }
        }

        return false;
    }

    /// <summary>
    /// 检查窗口或其子窗口是否有焦点
    /// </summary>
    private static bool IsWindowOrChildFocused(IntPtr windowHandle, IntPtr foregroundWindow)
    {
        if (windowHandle == foregroundWindow)
        {
            return true;
        }

        // 检查前台窗口是否是该窗口的子窗口
        var parent = foregroundWindow;
        while (parent != IntPtr.Zero)
        {
            parent = GetParent(parent);
            if (parent == windowHandle)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 检查鼠标是否在窗口内
    /// </summary>
    private static bool IsCursorInWindow(IntPtr windowHandle, POINT cursorPos)
    {
        if (!GetWindowRect(windowHandle, out var rect))
        {
            return false;
        }

        return cursorPos.X >= rect.Left && cursorPos.X <= rect.Right &&
               cursorPos.Y >= rect.Top && cursorPos.Y <= rect.Bottom;
    }

    /// <summary>
    /// 确保滚动速度设置为指定值
    /// </summary>
    private void EnsureSpeed(int targetSpeed)
    {
        var currentSpeed = GetCurrentSpeed();
        if (currentSpeed == targetSpeed)
        {
            return;
        }

        SetWheelSpeed(targetSpeed);
        _currentSpeed = targetSpeed;
    }

    /// <summary>
    /// 获取当前滚动速度
    /// </summary>
    private int GetCurrentSpeed()
    {
        if (_currentSpeed.HasValue)
        {
            return _currentSpeed.Value;
        }

        unsafe
        {
            uint lines = 0;
            var result = SystemParametersInfo(SPI_GETWHEELSCROLLLINES, 0, &lines, 0);
            if (result)
            {
                _currentSpeed = (int)lines;
                return _currentSpeed.Value;
            }
        }

        return _originalSpeed;
    }

    /// <summary>
    /// 设置滚动速度
    /// </summary>
    private static void SetWheelSpeed(int lines)
    {
        unsafe
        {
            var result = SystemParametersInfo(
                SPI_SETWHEELSCROLLLINES,
                (uint)lines,
                null,
                SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);

            if (!result)
            {
                Console.WriteLine($"Failed to set wheel speed to {lines}");
            }
        }
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    #region P/Invoke

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    private const uint SPI_GETWHEELSCROLLLINES = 0x0068;
    private const uint SPI_SETWHEELSCROLLLINES = 0x0069;
    private const uint SPIF_UPDATEINIFILE = 0x01;
    private const uint SPIF_SENDCHANGE = 0x02;

    [DllImport("user32.dll", SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static extern unsafe bool SystemParametersInfo(
        uint uiAction,
        uint uiParam,
        void* pvParam,
        uint fWinIni);

    [DllImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static extern IntPtr GetParent(IntPtr hWnd);

    [DllImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    #endregion
}
