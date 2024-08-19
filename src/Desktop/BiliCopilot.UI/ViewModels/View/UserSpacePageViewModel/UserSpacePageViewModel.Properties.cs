// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 用户动态页视图模型.
/// </summary>
public sealed partial class UserSpacePageViewModel
{
    [ObservableProperty]
    private IReadOnlyCollection<UserMomentDetailViewModel> _sections;

    [ObservableProperty]
    private UserMomentDetailViewModel _selectedSection;

    [ObservableProperty]
    private bool _isCommentsOpened;

    [ObservableProperty]
    private string _userName;

    /// <summary>
    /// 区块初始化完成.
    /// </summary>
    public event EventHandler Initialized;

    /// <summary>
    /// 评论模块.
    /// </summary>
    public CommentMainViewModel CommentModule { get; }
}
