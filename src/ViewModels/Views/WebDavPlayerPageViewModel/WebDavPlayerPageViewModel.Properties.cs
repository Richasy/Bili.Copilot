// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.ObjectModel;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.Models.Constants.Player;
using Bili.Copilot.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// WebDav 播放器页面视图模型.
/// </summary>
public sealed partial class WebDavPlayerPageViewModel
{
    private readonly DispatcherQueue _dispatcherQueue;
    private WebDavConfig _config;
    private Action _playNextVideoAction;
    private Action _playPreviousVideoAction;
    private bool _isStatsUpdated;
    private Window _attachedWindow;

    [ObservableProperty]
    private string _fileName;

    [ObservableProperty]
    private string _publishTime;

    [ObservableProperty]
    private bool _isError;

    [ObservableProperty]
    private string _errorText;

    [ObservableProperty]
    private PlayerSectionHeader _currentSection;

    [ObservableProperty]
    private WebDavStorageItemViewModel _currentItem;

    [ObservableProperty]
    private bool _isShowInformation;

    [ObservableProperty]
    private bool _isShowPlaylist;

    [ObservableProperty]
    private PlayerDetailViewModel _playerDetail;

    [ObservableProperty]
    private VideoMediaStats _stats;

    [ObservableProperty]
    private bool _isReloading;

    [ObservableProperty]
    private bool _isContinuePlay;

    /// <summary>
    /// 播放时的关联区块集合.
    /// </summary>
    public ObservableCollection<PlayerSectionHeader> Sections { get; }

    /// <summary>
    /// 播放列表.
    /// </summary>
    public ObservableCollection<WebDavStorageItemViewModel> Playlist { get; }
}
