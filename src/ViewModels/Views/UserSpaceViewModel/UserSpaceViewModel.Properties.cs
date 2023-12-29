// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using Bili.Copilot.Models.Data.User;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 用户空间视图模型.
/// </summary>
public sealed partial class UserSpaceViewModel
{
    private UserProfile _userProfile;
    private bool _isSpaceVideoFinished;
    private bool _isSearchVideoFinished;
    private string _requestKeyword;

    [ObservableProperty]
    private UserItemViewModel _userViewModel;

    [ObservableProperty]
    private bool _isMe;

    [ObservableProperty]
    private bool _isSearchMode;

    [ObservableProperty]
    private string _keyword;

    [ObservableProperty]
    private bool _isSpaceVideoEmpty;

    [ObservableProperty]
    private bool _isSearchVideoEmpty;

    [ObservableProperty]
    private bool _isFixed;

    [ObservableProperty]
    private bool _isSearching;

    [ObservableProperty]
    private bool _canSearch;

    [ObservableProperty]
    private bool _isMainShown;

    [ObservableProperty]
    private bool _isInFans;

    [ObservableProperty]
    private bool _isInFollows;

    [ObservableProperty]
    private FansDetailViewModel _fans;

    [ObservableProperty]
    private FollowsDetailViewModel _follows;

    /// <summary>
    /// 搜索的视频结果.
    /// </summary>
    public ObservableCollection<VideoItemViewModel> SearchVideos { get; }
}
