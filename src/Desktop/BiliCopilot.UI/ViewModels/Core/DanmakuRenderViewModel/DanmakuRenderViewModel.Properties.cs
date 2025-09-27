// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using Danmaku.Core;
using System.Collections.ObjectModel;

namespace BiliCopilot.UI.ViewModels.Core;

public sealed partial class DanmakuRenderViewModel
{
    private int _position;
    private string? _localDanmakuPath;
    private string? _localDanmakuXml;
    private List<DanmakuItem>? _cachedDanmakus;
    private Func<double>? _getDelayFunc;
    private Action<double>? _writeDelayAction;

    [ObservableProperty]
    public partial bool IsShowDanmaku { get; set; }

    [ObservableProperty]
    public partial double DanmakuOpacity { get; set; }

    [ObservableProperty]
    public partial double DanmakuFontSize { get; set; }

    [ObservableProperty]
    public partial string DanmakuFontFamily { get; set; }

    [ObservableProperty]
    public partial double DanmakuArea { get; set; }

    [ObservableProperty]
    public partial double DanmakuSpeed { get; set; }

    [ObservableProperty]
    public partial bool IsDanmakuLimit { get; set; }

    [ObservableProperty]
    public partial bool IsDanmakuBold { get; set; }

    [ObservableProperty]
    public partial double OutlineSize { get; set; }

    [ObservableProperty]
    public partial bool IsPaused { get; set; }

    [ObservableProperty]
    public partial double ExtraSpeed { get; set; } = 1;

    [ObservableProperty]
    public partial double DanmakuDelay { get; set; }

    [ObservableProperty]
    public partial bool IsRollingEnabled { get; set; }

    [ObservableProperty]
    public partial bool IsTopEnabled { get; set; }

    [ObservableProperty]
    public partial bool IsBottomEnabled { get; set; }

    [ObservableProperty]
    public partial string? DanmakuPoolName { get; set; }

    [ObservableProperty]
    public partial string? DanmakuCountText { get; set; }

    [ObservableProperty]
    public partial int DanmakuRefreshRate { get; set; }

    [ObservableProperty]
    public partial bool ForceSoftwareRenderer { get; set; }

    [ObservableProperty]
    public partial string? Title { get; set; }

    public ObservableCollection<string> Fonts { get; } = [];

    /// <summary>
    /// 速度加成改变时触发的事件.
    /// </summary>
    public event EventHandler ExtraSpeedChanged;

    /// <summary>
    /// 当请求重绘弹幕时触发的事件.
    /// </summary>
    public event EventHandler RequestRedrawDanmaku;

    /// <summary>
    /// 当请求清除弹幕时触发的事件.
    /// </summary>
    public event EventHandler RequestClearDanmaku;

    /// <summary>
    /// 当进度更新时触发的事件.
    /// </summary>
    public event EventHandler<int> ProgressChanged;

    /// <summary>
    /// 暂停弹幕.
    /// </summary>
    public event EventHandler PauseDanmaku;

    /// <summary>
    /// 恢复弹幕.
    /// </summary>
    public event EventHandler ResumeDanmaku;

    /// <summary>
    /// 请求重置样式.
    /// </summary>
    public event EventHandler RequestResetStyle;
}
