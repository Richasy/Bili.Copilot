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
/// 直播播放页视图模型.
/// </summary>
public sealed partial class LivePlayerPageViewModel
{
    private readonly IPlayerService _service;
    private readonly ILogger<LivePlayerPageViewModel> _logger;

    private CancellationTokenSource _pageLoadCancellationTokenSource;
    private CancellationTokenSource _playLoadCancellationTokenSource;

    private LivePlayerView? _view;

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
    private PlayerFormatItemViewModel? _selectedFormat;

    [ObservableProperty]
    private LiveLineInformation? _selectedLine;

    [ObservableProperty]
    private IReadOnlyCollection<PlayerFormatItemViewModel>? _formats;

    [ObservableProperty]
    private IReadOnlyCollection<LiveLineInformation>? _lines;

    /// <summary>
    /// 播放器视图模型.
    /// </summary>
    public PlayerViewModel Player { get; }
}
