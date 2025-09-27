// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.WinUIKernel.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 应用视图模型.
/// </summary>
public sealed partial class AppViewModel : ViewModelBase
{
    private readonly ILogger<AppViewModel> _logger;
    private readonly IBiliTokenResolver _tokenResolver;
    internal static int? _originalWheelLines;
    internal static bool _isWheelLinesChanged;

    [ObservableProperty]
    private Window _activatedWindow;

    [ObservableProperty]
    private bool _isInitialLoading;

    [ObservableProperty]
    private bool _isUpdateShown;

    /// <summary>
    /// 已创建的窗口列表.
    /// </summary>
    public List<Window> Windows { get; } = new();

    /// <summary>
    /// BBDown 路径.
    /// </summary>
    public string BBDownPath { get; private set; }

    /// <summary>
    /// FFmpeg 路径.
    /// </summary>
    public string FFmpegPath { get; private set; }

    /// <summary>
    /// 是否为 AMD 显卡.
    /// </summary>
    public bool? IsAmdGpu { get; private set; }

    /// <summary>
    /// 是否已关闭.
    /// </summary>
    public bool IsClosed { get; set; }
}
