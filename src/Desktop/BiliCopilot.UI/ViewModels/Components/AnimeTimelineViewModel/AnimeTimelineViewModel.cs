// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Media;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 动漫时间线视图模型.
/// </summary>
public sealed partial class AnimeTimelineViewModel : ViewModelBase, IAnimeSectionDetailViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AnimeTimelineViewModel"/> class.
    /// </summary>
    public AnimeTimelineViewModel(
        IEntertainmentDiscoveryService discoveryService)
    {
        _service = discoveryService;
        _logger = this.Get<ILogger<AnimeTimelineViewModel>>();
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        _ = this;
        await Task.CompletedTask;
    }
}
