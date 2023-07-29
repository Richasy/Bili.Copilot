// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Bili.Copilot.Models.App.Args;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Local;
using Bili.Copilot.Models.Data.User;
using Bili.Copilot.Models.Data.Video;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 应用视图模型的属性定义.
/// </summary>
public sealed partial class AppViewModel
{
    [ObservableProperty]
    private bool _isBackButtonShown;

    [ObservableProperty]
    private bool _isNavigationMenuShown;

    [ObservableProperty]
    private NavigateItem _currentNavigateItem;

    [ObservableProperty]
    private PageType _currentPage;

    [ObservableProperty]
    private bool _isSigningIn;

    [ObservableProperty]
    private bool _isEngineStarted;

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
    public event EventHandler<AppTipNotificationEventArgs> RequestShowTip;

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
    /// 在有搜索请求时触发.
    /// </summary>
    public event EventHandler<string> RequestSearch;

    /// <summary>
    /// 在有新的用户空间请求时触发.
    /// </summary>
    public event EventHandler<UserProfile> RequestShowUserSpace;

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
    public ObservableCollection<NavigateItem> NavigateItems { get; }
}
