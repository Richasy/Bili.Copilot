// Copyright (c) Bili Copilot. All rights reserved.

using System.Threading;
using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Models.Media;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 视频播放器页面视图模型.
/// </summary>
public sealed partial class VideoPlayerPageViewModel
{
    private readonly IPlayerService _service;
    private readonly ILogger<VideoPlayerPageViewModel> _logger;

    private CancellationTokenSource _pageLoadCancellationTokenSource;
    private CancellationTokenSource _dashLoadCancellationTokenSource;

    private VideoPlayerView? _view;
    private IList<DashSegmentInformation>? _videoSegments;
    private IList<DashSegmentInformation>? _audioSegments;

    [ObservableProperty]
    private bool _isPageLoading;

    [ObservableProperty]
    private bool _isPageLoadFailed;

    [ObservableProperty]
    private bool _isMediaLoading;

    [ObservableProperty]
    private bool _isMediaLoadFailed;

    [ObservableProperty]
    private Uri? _cover;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private bool _isMyVideo;

    [ObservableProperty]
    private double _playerWidth;

    [ObservableProperty]
    private double _playerHeight;

    [ObservableProperty]
    private PlayerFormatItemViewModel? _selectedFormat;

    [ObservableProperty]
    private IReadOnlyCollection<PlayerFormatItemViewModel>? _formats;

    /// <summary>
    /// 播放器视图模型.
    /// </summary>
    public PlayerViewModel Player { get; }
}
