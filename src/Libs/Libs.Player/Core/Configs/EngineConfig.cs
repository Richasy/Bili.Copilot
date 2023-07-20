// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Libs.Player.Core.Engines;
using Bili.Copilot.Libs.Player.Enums;
using Bili.Copilot.Libs.Player.Misc;

namespace Bili.Copilot.Libs.Player.Core.Configs;

/// <summary>
/// 引擎配置.
/// </summary>
public sealed class EngineConfig
{
    private static bool _uiRefresh;
    private FFmpegLogLevel _ffmpegLogLevel = FFmpegLogLevel.Quiet;
    private string _logOutput = string.Empty;

    /// <summary>
    /// 激活主线程以监视所有播放器并执行所需的更新.
    /// 在 Activity Mode、Stats 和 Buffered Duration on Pause 时需要.
    /// </summary>
    public static bool UIRefresh
    {
        get => _uiRefresh;
        set
        {
            _uiRefresh = value;
            if (value && Engine.IsLoaded)
            {
                Engine.StartThread();
            }
        }
    }

    /// <summary>
    /// 是否禁用音频并在全局范围内禁用.
    /// </summary>
    public bool DisableAudio { get; set; }

    /// <summary>
    /// 注册 FFmpeg 库所需的路径.根据项目选择 x86 或 x64.
    /// :&lt;path&gt; 表示相对于当前文件夹或其子文件夹的路径.
    /// &lt;path&gt; 表示绝对或相对路径.
    /// </summary>
    public string FFmpegPath { get; set; } = "FFmpeg";

    /// <summary>
    /// 是否注册 av 设备（gdigrab/dshow 等）.
    /// 启用后，可以使用以下格式的 URL：
    /// device://[device_name]?[FFmpeg_Url]
    /// device://gdigrab?desktop
    /// device://gdigrab?title=Command Prompt
    /// device://dshow?video=Lenovo Camera
    /// device://dshow?audio=Microphone (Relatek):video=Lenovo Camera.
    /// </summary>
    public bool FFmpegDevices { get; set; }

    /// <summary>
    /// 是否允许 HLS 直播定位（这可能会导致不兼容的 FFmpeg 版本与库的自定义结构导致的分段错误）.
    /// </summary>
    public bool FFmpegHLSLiveSeek { get; set; }

    /// <summary>
    /// 设置 FFmpeg 日志记录器的级别.
    /// </summary>
    public FFmpegLogLevel FFmpegLogLevel
    {
        get => _ffmpegLogLevel;
        set
        {
            _ffmpegLogLevel = value;
            if (Engine.IsLoaded)
            {
                FFmpegEngine.SetLogLevel();
            }
        }
    }

    /// <summary>
    /// 配置是否已从文件加载.
    /// </summary>
    public bool Loaded { get; private set; }

    /// <summary>
    /// 加载配置的路径.
    /// </summary>
    public string LoadedPath { get; private set; }

    /// <summary>
    /// 设置日志记录器的输出.
    /// :debug -> System.Diagnostics.Debug
    /// :console -> System.Console
    /// &lt;path&gt; -> 绝对或相对文件路径.
    /// </summary>
    public string LogOutput
    {
        get => _logOutput;
        set
        {
            _logOutput = value;
            if (Engine.IsLoaded)
            {
                Logger.SetOutput();
            }
        }
    }

    /// <summary>
    /// 设置日志记录器的级别.
    /// </summary>
    public LogLevel LogLevel { get; set; } = LogLevel.Quiet;

    /// <summary>
    /// 当输出为文件时，是否追加而不是覆盖.
    /// </summary>
    public bool LogAppend { get; set; }

    /// <summary>
    /// 写入文件之前缓存的行数.
    /// </summary>
    public int LogCachedLines { get; set; } = 20;

    /// <summary>
    /// 设置日志记录器的日期时间字符串格式.
    /// </summary>
    public string LogDateTimeFormat { get; set; } = "HH.mm.ss.fff";

    /// <summary>
    /// 注册插件所需的路径.根据项目选择 x86 或 x64，并使用相同的 .NET Framework.
    /// :&lt;path&gt; 表示相对于当前文件夹或其子文件夹的路径.
    /// &lt;path&gt; 表示绝对或相对路径.
    /// </summary>
    public string PluginsPath { get; set; } = "Plugins";

    /// <summary>
    /// 当秒数变化时更新 Player.CurTime，否则在每个 UIRefreshInterval 上更新.
    /// </summary>
    public bool UICurTimePerSecond { get; set; } = true;

    /// <summary>
    /// 每隔多少毫秒更新一次 UI（低值可能会导致性能问题）.
    /// </summary>
    public int UIRefreshInterval { get; set; } = 250;
}
