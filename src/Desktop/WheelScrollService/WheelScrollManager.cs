using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WheelScrollService;

/// <summary>
/// 鼠标滚轮速度管理器
/// </summary>
public sealed class WheelScrollManager
{
    private readonly int _originalSpeed;
    private readonly int _expectedSpeed;
    private readonly Timer _timer;
    private int? _currentSpeed;

    public WheelScrollManager(int originalSpeed, int expectedSpeed)
    {
        _originalSpeed = originalSpeed;
        _expectedSpeed = expectedSpeed;
        _timer = new Timer(CheckAndAdjust, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
    }

    /// <summary>
    /// 检查并调整滚动速度
    /// </summary>
    private void CheckAndAdjust(object? state)
    {
        try
        {
            var rodelProcesses = Process.GetProcesses()
                .Where(p => p.ProcessName.Contains("BiliCopilot", StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (rodelProcesses.Count == 0)
            {
                // 没有 BiliCopilot 进程，恢复到初始速度后退出
                EnsureSpeed(_originalSpeed);
                Environment.Exit(0);
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
