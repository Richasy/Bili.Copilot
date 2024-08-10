// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Richasy.BiliKernel.Bili.Moment;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 动态页面视图模型.
/// </summary>
public sealed partial class MomentPageViewModel
{
    private readonly IMomentDiscoveryService _momentDiscoveryService;

    [ObservableProperty]
    private IReadOnlyCollection<IMomentSectionDetailViewModel> _sections;

    [ObservableProperty]
    private IMomentSectionDetailViewModel _selectedSection;

    [ObservableProperty]
    private bool _isCommentsOpened;

    /// <summary>
    /// 区块初始化完成.
    /// </summary>
    public event EventHandler Initialized;

    /// <summary>
    /// 评论模块.
    /// </summary>
    public CommentMainViewModel CommentModule { get; }
}
