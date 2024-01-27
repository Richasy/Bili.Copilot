// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.ObjectModel;
using Bili.Copilot.Models.Data.Video;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 视频收藏夹详情视图模型.
/// </summary>
public sealed partial class VideoFavoriteDetailViewModel
{
    private static readonly Lazy<VideoFavoriteDetailViewModel> _lazyInstance = new(() => new VideoFavoriteDetailViewModel());
    private bool _isEnd;
    private VideoFavoriteView _view;

    [ObservableProperty]
    private bool _isMine;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private VideoFavoriteFolderSelectableViewModel _currentFolder;

    /// <summary>
    /// 实例.
    /// </summary>
    public static VideoFavoriteDetailViewModel Instance => _lazyInstance.Value;

    /// <summary>
    /// 收藏夹集合.
    /// </summary>
    public ObservableCollection<VideoFavoriteFolderSelectableViewModel> Folders { get; }
}
