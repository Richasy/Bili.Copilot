// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using CommunityToolkit.Mvvm.ComponentModel;
using Richasy.BiliKernel.Models.Danmaku;
using System.Collections.ObjectModel;

namespace BiliCopilot.UI.ViewModels.Core;

public sealed partial class DanmakuRenderViewModel
{
    private int _position;
    private IList<DanmakuInformation>? _cachedDanmakus;

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
    public partial string? Title { get; set; }

    [ObservableProperty]
    public partial DanmakuRendererType Renderer { get; set; }

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

    /// <summary>
    /// 请求添加单条弹幕.
    /// </summary>
    public event EventHandler<string> RequestAddSingleDanmaku;
}
