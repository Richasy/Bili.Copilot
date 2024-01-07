// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Bili.Copilot.Models.App.Args;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Article;
using Bili.Copilot.Models.Data.Local;
using Bili.Copilot.Models.Data.User;
using Bili.Copilot.Models.Data.Video;
using Bili.Copilot.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 应用视图模型的属性定义.
/// </summary>
public sealed partial class AppViewModel
{
    private readonly DispatcherQueue _dispatcherQueue;

    [ObservableProperty]
    private bool _isBackButtonShown;

    [ObservableProperty]
    private PageType _currentPage;

    [ObservableProperty]
    private bool _isSigningIn;

    [ObservableProperty]
    private bool _isEngineStarted;

    [ObservableProperty]
    private bool _isDownloadSupported;

    [ObservableProperty]
    private Window _activatedWindow;

    [ObservableProperty]
    private NavigateItemViewModel _settingsItem;

    [ObservableProperty]
    private bool _isOverlayShown;

    [ObservableProperty]
    private MessageDetailViewModel _message;

    [ObservableProperty]
    private FansDetailViewModel _fans;

    [ObservableProperty]
    private MyFollowsDetailViewModel _follows;

    /// <summary>
    /// 在请求后退时触发.
    /// </summary>
    public event EventHandler BackRequest;

    /// <summary>
    /// 在有新的导航请求时触发.
    /// </summary>
    public event EventHandler<AppNavigationEventArgs> NavigateRequest;

    /// <summary>
    /// 在有新的提示请求时触发.
    /// </summary>
    public event EventHandler<AppTipNotification> RequestShowTip;

    /// <summary>
    /// 有新的播放请求时触发.
    /// </summary>
    public event EventHandler<PlaySnapshot> RequestPlay;

    /// <summary>
    /// 有新的播放请求时触发.
    /// </summary>
    public event EventHandler<List<VideoInformation>> RequestPlaylist;

    /// <summary>
    /// 在有新的消息请求时触发.
    /// </summary>
    public event EventHandler<string> RequestShowMessage;

    /// <summary>
    /// 请求显示升级提示.
    /// </summary>
    public event EventHandler<UpdateEventArgs> RequestShowUpdateDialog;

    /// <summary>
    /// 在有搜索请求时触发.
    /// </summary>
    public event EventHandler<string> RequestSearch;

    /// <summary>
    /// 在有新的粉丝请求时触发.
    /// </summary>
    public event EventHandler<UserProfile> RequestShowFans;

    /// <summary>
    /// 在有新的关注请求时触发.
    /// </summary>
    public event EventHandler RequestShowFollows;

    /// <summary>
    /// 在有消息页面请求时触发.
    /// </summary>
    public event EventHandler RequestShowMyMessages;

    /// <summary>
    /// 请求显示稍后再看.
    /// </summary>
    public event EventHandler RequestShowViewLater;

    /// <summary>
    /// 请求显示历史记录.
    /// </summary>
    public event EventHandler RequestShowHistory;

    /// <summary>
    /// 请求显示收藏夹.
    /// </summary>
    public event EventHandler<FavoriteType> RequestShowFavorites;

    /// <summary>
    /// 在有新的用户空间请求时触发.
    /// </summary>
    public event EventHandler<UserProfile> RequestShowUserSpace;

    /// <summary>
    /// 在有新的视频动态评论请求时触发.
    /// </summary>
    public event EventHandler<ShowCommentEventArgs> RequestShowCommentWindow;

    /// <summary>
    /// 在有新的阅读请求时触发.
    /// </summary>
    public event EventHandler<ArticleInformation> RequestRead;

    /// <summary>
    /// 请求显示图片.
    /// </summary>
    public event EventHandler<ShowImageEventArgs> RequestShowImages;

    /// <summary>
    /// 激活主窗口.
    /// </summary>
    public event EventHandler ActiveMainWindow;

    /// <summary>
    /// 实例.
    /// </summary>
    public static AppViewModel Instance { get; } = new();

    /// <summary>
    /// 导航条目列表.
    /// </summary>
    public ObservableCollection<NavigateItemViewModel> NavigateItems { get; }

    /// <summary>
    /// 显示的窗口列表.
    /// </summary>
    public List<Window> DisplayWindows { get; }
}
