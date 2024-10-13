// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Danmaku;
using Windows.UI;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 弹幕视图模型.
/// </summary>
public sealed partial class DanmakuViewModel
{
    private readonly IDanmakuService _danmakuService;
    private readonly ILogger<DanmakuViewModel> _logger;

    private string _aid;
    private string _cid;
    private int _position;
    private int _duration;

    [ObservableProperty]
    private bool _isShowDanmaku;

    [ObservableProperty]
    private bool _canShowDanmaku;

    [ObservableProperty]
    private double _danmakuOpacity;

    [ObservableProperty]
    private double _danmakuFontSize;

    [ObservableProperty]
    private string _danmakuFontFamily;

    [ObservableProperty]
    private double _danmakuArea;

    [ObservableProperty]
    private double _danmakuSpeed;

    [ObservableProperty]
    private double _extraSpeed = 1;

    [ObservableProperty]
    private bool _isDanmakuLimit;

    [ObservableProperty]
    private bool _isDanmakuBold;

    [ObservableProperty]
    private bool _isStandardSize;

    [ObservableProperty]
    private DanmakuLocation _location;

    [ObservableProperty]
    private Color _color;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private List<DanmakuLocation> _locations;

    [ObservableProperty]
    private List<Color> _colors;

    [ObservableProperty]
    private List<string> _fonts;

    [ObservableProperty]
    private bool _isPaused;

    /// <summary>
    /// 当弹幕列表添加时触发的事件.
    /// </summary>
    public event EventHandler<IReadOnlyList<DanmakuInformation>> ListAdded;

    /// <summary>
    /// 速度加成改变时触发的事件.
    /// </summary>
    public event EventHandler ExtraSpeedChanged;

    /// <summary>
    /// 当请求清除弹幕时触发的事件.
    /// </summary>
    public event EventHandler RequestClearDanmaku;

    /// <summary>
    /// 当请求重绘弹幕时触发的事件.
    /// </summary>
    public event EventHandler RequestRedrawDanmaku;

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
    /// 请求添加单条弹幕.
    /// </summary>
    public event EventHandler<string> RequestAddSingleDanmaku;

    /// <summary>
    /// 请求重置样式.
    /// </summary>
    public event EventHandler RequestResetStyle;

    /// <summary>
    /// 是否为直播弹幕模块.
    /// </summary>
    public bool IsLive { get; private set; }
}
