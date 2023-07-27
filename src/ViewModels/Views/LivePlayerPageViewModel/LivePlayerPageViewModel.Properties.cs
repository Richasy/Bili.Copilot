// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.ObjectModel;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.Models.Data.Live;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 直播播放页面视图模型.
/// </summary>
public sealed partial class LivePlayerPageViewModel
{
    private readonly DispatcherQueue _dispatcherQueue;

    private DispatcherTimer _heartBeatTimer;
    private string _presetRoomId;
    private Window _attachedWindow;

    [ObservableProperty]
    private LivePlayerView _view;

    [ObservableProperty]
    private bool _isSignedIn;

    [ObservableProperty]
    private UserItemViewModel _user;

    [ObservableProperty]
    private string _watchingCountText;

    [ObservableProperty]
    private bool _isError;

    [ObservableProperty]
    private string _errorText;

    [ObservableProperty]
    private bool _isLiveFixed;

    [ObservableProperty]
    private PlayerSectionHeader _currentSection;

    [ObservableProperty]
    private bool _isDanmakusEmpty;

    [ObservableProperty]
    private bool _isDanmakusAutoScroll;

    [ObservableProperty]
    private bool _isReloading;

    [ObservableProperty]
    private bool _isShowChat;

    [ObservableProperty]
    private bool _isShowInformation;

    [ObservableProperty]
    private PlayerDetailViewModel _playerDetail;

    [ObservableProperty]
    private LiveMediaStats _stats;

    /// <summary>
    /// 请求弹幕滚动到底部.
    /// </summary>
    public event EventHandler RequestDanmakusScrollToBottom;

    /// <summary>
    /// 弹幕列表.
    /// </summary>
    public ObservableCollection<LiveDanmakuInformation> Danmakus { get; }

    /// <summary>
    /// 区块集合.
    /// </summary>
    public ObservableCollection<PlayerSectionHeader> Sections { get; }
}
