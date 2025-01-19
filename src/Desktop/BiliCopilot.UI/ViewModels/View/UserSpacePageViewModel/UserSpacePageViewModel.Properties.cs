// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Search;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Models.User;
using System.Collections.ObjectModel;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 用户动态页视图模型.
/// </summary>
public sealed partial class UserSpacePageViewModel
{
    private readonly IRelationshipService _relationshipService;
    private readonly IUserService _userService;
    private readonly ISearchService _searchService;
    private readonly ILogger<UserSpacePageViewModel> _logger;
    private UserProfile _profile;
    private int _searchPn;
    private bool _preventLoadMoreSearch;
    private CancellationTokenSource _searchCancellationTokenSource;

    [ObservableProperty]
    private List<UserMomentDetailViewModel> _sections;

    [ObservableProperty]
    private UserMomentDetailViewModel _selectedSection;

    [ObservableProperty]
    private bool _isCommentsOpened;

    [ObservableProperty]
    private string _userName;

    [ObservableProperty]
    private bool _isFollowed;

    [ObservableProperty]
    private bool _isSearching;

    [ObservableProperty]
    private bool _isSearchMode;

    [ObservableProperty]
    private string _searchKeyword;

    [ObservableProperty]
    private bool _isSearchEmpty;

    [ObservableProperty]
    private UserCard _card;

    /// <summary>
    /// 区块初始化完成.
    /// </summary>
    public event EventHandler Initialized;

    /// <summary>
    /// 搜索更新.
    /// </summary>
    public event EventHandler SearchUpdated;

    /// <summary>
    /// 评论模块.
    /// </summary>
    public CommentMainViewModel CommentModule { get; }

    /// <summary>
    /// 搜索视频结果.
    /// </summary>
    public ObservableCollection<VideoItemViewModel> SearchVideos { get; } = new();
}
