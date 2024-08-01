// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Media;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 动漫分区详情视图模型.
/// </summary>
public sealed partial class AnimeSectionDetailViewModel : ViewModelBase, IAnimeSectionDetailViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AnimeSectionDetailViewModel"/> class.
    /// </summary>
    public AnimeSectionDetailViewModel(
        AnimeSectionType type,
        IEntertainmentDiscoveryService service)
    {
        SectionType = type;
        _logger = this.Get<ILogger<AnimeSectionDetailViewModel>>();
        _service = service;
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        _ = this;
        await Task.CompletedTask;
    }
}
