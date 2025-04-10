// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Models.Media;

namespace BiliCopilot.UI.ViewModels.Core;

/// <summary>
/// 直播源视图模型.
/// </summary>
public sealed partial class LiveSourceViewModel
{
    private const string LiveUserAgent = "Mozilla/5.0 BiliDroid/1.12.0 (bbcallen@gmail.com)";
    private const string LiveReferer = "https://live.bilibili.com";

    private readonly IPlayerService _service;
    private readonly IRelationshipService _relationshipService;
    private readonly IDanmakuService _danmakuService;
    private readonly ILogger<LiveSourceViewModel> _logger;

    private MediaIdentifier? _cachedMedia;
    private LivePlayerView? _view;
    private string? _liveUrl;

    public string Id { get; set; }

    [ObservableProperty]
    public partial Uri? Cover { get; set; }

    [ObservableProperty]
    public partial string? Title { get; set; }

    [ObservableProperty]
    public partial string? Description { get; set; }

    [ObservableProperty]
    public partial string? UpName { get; set; }

    [ObservableProperty]
    public partial Uri? UpAvatar { get; set; }

    [ObservableProperty]
    public partial string? StartRelativeTime { get; set; }

    [ObservableProperty]
    public partial bool IsFollow { get; set; }

    [ObservableProperty]
    public partial double ViewerCount { get; set; }

    [ObservableProperty]
    public partial string? TagName { get; set; }

    [ObservableProperty]
    public partial string? RoomId { get; set; }

    [ObservableProperty]
    public partial double Duration { get; set; }

    [ObservableProperty]
    public partial DateTimeOffset StartTime { get; set; }

    [ObservableProperty]
    public partial PlayerFormatItemViewModel? SelectedFormat { get; set; }

    [ObservableProperty]
    public partial List<PlayerFormatItemViewModel>? Formats { get; set; }

    [ObservableProperty]
    public partial List<LiveLineInformation>? Lines { get; set; }

    [ObservableProperty]
    public partial LiveLineInformation? SelectedLine { get; set; }

    [ObservableProperty]
    public partial bool IsInfoSectionPanelVisible { get; set; }

    [ObservableProperty]
    public partial bool IsChatSectionPanelVisible { get; set; }

    [ObservableProperty]
    public partial bool IsChatSectionPanelLoaded { get; set; }

    [ObservableProperty]
    public partial string? ErrorMessage { get; set; }

    /// <summary>
    /// 弹幕视图模型.
    /// </summary>
    public DanmakuViewModel Danmaku { get; }

    /// <summary>
    /// 聊天视图模型.
    /// </summary>
    public LiveChatSectionDetailViewModel Chat { get; }
}
