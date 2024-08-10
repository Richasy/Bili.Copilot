// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Moment;
using Richasy.BiliKernel.Models.Appearance;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 动态条目视图模型.
/// </summary>
public sealed partial class MomentItemViewModel
{
    private readonly IMomentOperationService _operationService;
    private readonly ILogger<MomentItemViewModel> _logger;
    private readonly Action<MomentItemViewModel> _showMomentAction;

    [ObservableProperty]
    private double? _likeCount;

    [ObservableProperty]
    private double? _commentCount;

    [ObservableProperty]
    private bool _isLiked;

    /// <summary>
    /// 作者.
    /// </summary>
    public string? Author { get; init; }

    /// <summary>
    /// 头像.
    /// </summary>
    public Uri? Avatar { get; init; }

    /// <summary>
    /// 副标题.
    /// </summary>
    public string? Tip { get; init; }

    /// <summary>
    /// 视频封面.
    /// </summary>
    public Uri? VideoCover { get; init; }

    /// <summary>
    /// 说明.
    /// </summary>
    public EmoteText? Description { get; init; }

    /// <summary>
    /// 视频标题.
    /// </summary>
    public string VideoTitle { get; init; }

    /// <summary>
    /// 是否没有数据.
    /// </summary>
    public bool NoData { get; init; }

    /// <summary>
    /// 内部内容.
    /// </summary>
    public object? InnerContent { get; init; }
}
