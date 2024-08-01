// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Media;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 动漫分区详情视图模型.
/// </summary>
public sealed partial class AnimeSectionDetailViewModel
{
    private readonly IEntertainmentDiscoveryService _service;
    private readonly ILogger<AnimeSectionDetailViewModel> _logger;

    /// <inheritdoc/>
    public AnimeSectionType SectionType { get; }
}
