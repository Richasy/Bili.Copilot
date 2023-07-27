// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Data.Live;
using Bili.Copilot.Models.Data.Local;
using Bili.Copilot.Models.Data.Player;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 弹幕视图模型.
/// </summary>
public sealed partial class DanmakuModuleViewModel
{
    private string _mainId;
    private string _partId;
    private int _segmentIndex;
    private double _currentSeconds;
    private VideoType _videoType;

    [ObservableProperty]
    private bool _isShowDanmaku;

    [ObservableProperty]
    private bool _canShowDanmaku;

    [ObservableProperty]
    private double _danmakuOpacity;

    [ObservableProperty]
    private double _danmakuFontSize;

    [ObservableProperty]
    private double _danmakuArea;

    [ObservableProperty]
    private double _danmakuSpeed;

    [ObservableProperty]
    private string _danmakuFont;

    [ObservableProperty]
    private bool _isDanmakuLimit;

    [ObservableProperty]
    private bool _isDanmakuMerge;

    [ObservableProperty]
    private bool _isDanmakuBold;

    [ObservableProperty]
    private double _danmakuZoom;

    [ObservableProperty]
    private bool _useCloudShieldSettings;

    [ObservableProperty]
    private bool _isStandardSize;

    [ObservableProperty]
    private DanmakuLocation _location;

    [ObservableProperty]
    private string _color;

    [ObservableProperty]
    private bool _isReloading;

    [ObservableProperty]
    private bool _isDanmakuLoading;

    /// <summary>
    /// 当弹幕列表添加时触发的事件.
    /// </summary>
    public event EventHandler<IEnumerable<DanmakuInformation>> DanmakuListAdded;

    /// <summary>
    /// 当发送弹幕成功时触发的事件.
    /// </summary>
    public event EventHandler<string> SendDanmakuSucceeded;

    /// <summary>
    /// 当请求清除弹幕时触发的事件.
    /// </summary>
    public event EventHandler RequestClearDanmaku;

    /// <summary>
    /// 当直播弹幕添加时触发的事件.
    /// </summary>
    public event EventHandler<LiveDanmakuInformation> LiveDanmakuAdded;

    /// <summary>
    /// 弹幕位置的可观察集合.
    /// </summary>
    public ObservableCollection<DanmakuLocation> LocationCollection { get; }

    /// <summary>
    /// 弹幕颜色的可观察集合.
    /// </summary>
    public ObservableCollection<KeyValue<string>> ColorCollection { get; }

    /// <summary>
    /// 弹幕字体的可观察集合.
    /// </summary>
    public ObservableCollection<string> FontCollection { get; }
}
