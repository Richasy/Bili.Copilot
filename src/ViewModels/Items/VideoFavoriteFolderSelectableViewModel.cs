// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Data.Video;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 可选择的视频收藏夹视图模型.
/// </summary>
public sealed class VideoFavoriteFolderSelectableViewModel : SelectableViewModel<VideoFavoriteFolder>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoFavoriteFolderSelectableViewModel"/> class.
    /// </summary>
    public VideoFavoriteFolderSelectableViewModel(VideoFavoriteFolder data)
        : base(data)
    {
    }
}
