// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Mpv.Core;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 播放器视图模型.
/// </summary>
public sealed partial class PlayerViewModel
{
    private const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36 Edg/91.0.864.67";

    private readonly ILogger<PlayerViewModel> _logger;
    private readonly DispatcherQueue _dispatcherQueue;

    private string? _videoUrl;
    private string? _audioUrl;
    private bool _autoPlay;

    private bool _isInitialized;

    [ObservableProperty]
    private bool _isPlayerInitializing;

    [ObservableProperty]
    private bool _isPlayerDataLoading;

    /// <summary>
    /// 播放器内核.
    /// </summary>
    public Player? Player { get; private set; }
}
