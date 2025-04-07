// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Richasy.MpvKernel.Core;
using Richasy.MpvKernel.Core.Enums;
using Richasy.MpvKernel.WinUI;

namespace BiliCopilot.UI.ViewModels.Core;

public sealed partial class MpvPlayerWindowViewModel
{
    private const int WindowMinWidth = 640;
    private const int WindowMinHeight = 480;

    private readonly ILogger<MpvPlayerWindowViewModel> _logger;
    private readonly DispatcherQueue _queue;
    private readonly IMediaSourceResolver _sourceResolver;
    private readonly IMediaUIProvider? _uiProvider;
    private DispatcherQueueTimer? _tipTimer;

    /// <summary>
    /// 播放客户端.
    /// </summary>
    internal MpvClient? Client { get; private set; }

    /// <summary>
    /// 播放窗口.
    /// </summary>
    internal MpvPlayerWindow? Window { get; private set; }

    /// <summary>
    /// 正在加载媒体源信息.
    /// </summary>
    [ObservableProperty]
    public partial bool IsSourceLoading { get; set; }

    [ObservableProperty]
    public partial bool IsFileLoading { get; set; }

    /// <summary>
    /// 表示媒体是否正在加载，比如 seeking 或者 buffering.
    /// </summary>
    [ObservableProperty]
    public partial bool IsMediaLoading { get; set; }

    [ObservableProperty]
    public partial bool IsPlaying { get; set; }

    /// <summary>
    /// 该状态表示没有任何媒体加载.
    /// </summary>
    [ObservableProperty]
    public partial bool IsIdle { get; set; }

    /// <summary>
    /// 是否显示重载按钮.
    /// </summary>
    [ObservableProperty]
    public partial bool IsRestartVisible { get; set; }

    [ObservableProperty]
    public partial MpvPlayerState LastState { get; set; }

    [ObservableProperty]
    public partial double Duration { get; set; }

    [ObservableProperty]
    public partial double CurrentPosition { get; set; }

    [ObservableProperty]
    public partial double Volume { get; set; }

    [ObservableProperty]
    public partial double Speed { get; set; }

    [ObservableProperty]
    public partial bool IsFullScreen { get; set; }

    [ObservableProperty]
    public partial bool IsCompactOverlay { get; set; }

    [ObservableProperty]
    public partial bool IsProgressChanging { get; set; }

    [ObservableProperty]
    public partial double PreviewPosition { get; set; }

    [ObservableProperty]
    public partial bool IsVolumeChanging { get; set; }
}
