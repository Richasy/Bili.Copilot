// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Models.User;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 用户动态页视图模型.
/// </summary>
public sealed partial class UserSpacePageViewModel
{
    private readonly IRelationshipService _relationshipService;
    private readonly ILogger<UserSpacePageViewModel> _logger;
    private UserProfile _profile;

    [ObservableProperty]
    private IReadOnlyCollection<UserMomentDetailViewModel> _sections;

    [ObservableProperty]
    private UserMomentDetailViewModel _selectedSection;

    [ObservableProperty]
    private bool _isCommentsOpened;

    [ObservableProperty]
    private string _userName;

    [ObservableProperty]
    private bool _isFollowed;

    /// <summary>
    /// 区块初始化完成.
    /// </summary>
    public event EventHandler Initialized;

    /// <summary>
    /// 评论模块.
    /// </summary>
    public CommentMainViewModel CommentModule { get; }
}
