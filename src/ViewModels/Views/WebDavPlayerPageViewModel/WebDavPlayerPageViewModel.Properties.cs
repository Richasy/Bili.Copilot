// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Windows.Media.Playback;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// WebDav 播放器页面视图模型.
/// </summary>
public sealed partial class WebDavPlayerPageViewModel
{
    private readonly DispatcherQueue _dispatcherQueue;
    private WebDavConfig _config;
    private HttpRandomAccessStream _stream;

    [ObservableProperty]
    private MediaPlayer _player;

    [ObservableProperty]
    private bool _onlyOne;

    [ObservableProperty]
    private WebDavStorageItemViewModel _selectedItem;

    /// <summary>
    /// 播放列表.
    /// </summary>
    public ObservableCollection<WebDavStorageItemViewModel> Playlist { get; }

    /// <summary>
    /// 关联窗口.
    /// </summary>
    public Window AttachedWindow { get; set; }
}
