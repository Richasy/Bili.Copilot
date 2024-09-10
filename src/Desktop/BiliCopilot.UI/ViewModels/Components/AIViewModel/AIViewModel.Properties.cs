// Copyright (c) Bili Copilot. All rights reserved.

using BiliAgent.Interfaces;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// AI 视图模型.
/// </summary>
public sealed partial class AIViewModel
{
    private readonly ILogger<AIViewModel> _logger;
    private readonly IAgentClient _client;

    [ObservableProperty]
    private IReadOnlyCollection<AIServiceItemViewModel> _services;

    [ObservableProperty]
    private IReadOnlyCollection<ChatModelItemViewModel> _models;

    [ObservableProperty]
    private IReadOnlyCollection<AIQuickItemViewModel> _quickItems;

    [ObservableProperty]
    private AIServiceItemViewModel _selectedService;

    [ObservableProperty]
    private ChatModelItemViewModel _selectedModel;

    [ObservableProperty]
    private bool _isNoService;

    [ObservableProperty]
    private bool _isNoModel;
}
