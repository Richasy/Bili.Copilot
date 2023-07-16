// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Data.Video;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 视频收藏夹详情视图模型.
/// </summary>
public sealed partial class DefaultVideoFavoriteDetailViewModel
{
    private VideoFavoriteFolder _defaultFolder;
    private bool _isEnd;

    [ObservableProperty]
    private bool _isEmpty;

    /// <summary>
    /// 实例.
    /// </summary>
    public static DefaultVideoFavoriteDetailViewModel Instance { get; } = new();
}
