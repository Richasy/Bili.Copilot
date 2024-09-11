// Copyright (c) Bili Copilot. All rights reserved.

using System.Threading;
using BiliAgent.Interfaces;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Models.Media;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// AI 视图模型.
/// </summary>
public sealed partial class AIViewModel
{
    private readonly ILogger<AIViewModel> _logger;
    private readonly IAgentClient _client;

    private VideoPlayerView? _videoView;
    private VideoPart? _videoPart;

    private AIQuickItemViewModel? _currentPrompt;

    private CancellationTokenSource? _generateCancellationTokenSource;

    [ObservableProperty]
    private IReadOnlyCollection<AIServiceItemViewModel> _services;

    [ObservableProperty]
    private IReadOnlyCollection<ChatModelItemViewModel> _models;

    [ObservableProperty]
    private IReadOnlyCollection<AIQuickItemViewModel> _quickItems;

    [ObservableProperty]
    private IReadOnlyCollection<AIQuickItemViewModel> _morePrompts;

    [ObservableProperty]
    private AIServiceItemViewModel _selectedService;

    [ObservableProperty]
    private ChatModelItemViewModel _selectedModel;

    [ObservableProperty]
    private bool _isNoService;

    [ObservableProperty]
    private bool _isNoModel;

    [ObservableProperty]
    private bool _isQuickItemsVisible;

    [ObservableProperty]
    private bool _isGenerating;

    [ObservableProperty]
    private string _requestText;

    [ObservableProperty]
    private string _tempResult;

    [ObservableProperty]
    private string _finalResult;

    [ObservableProperty]
    private string _errorMessage;

    [ObservableProperty]
    private string _progressTip;

    [ObservableProperty]
    private string? _sourceTitle;

    [ObservableProperty]
    private string? _sourceSubtitle;

    [ObservableProperty]
    private Uri? _sourceCover;
}
