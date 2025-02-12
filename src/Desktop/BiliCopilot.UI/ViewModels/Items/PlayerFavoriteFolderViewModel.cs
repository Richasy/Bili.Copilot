// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using Richasy.BiliKernel.Models.Media;
using Richasy.WinUIKernel.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 用户收藏夹视图模型.
/// </summary>
public sealed partial class PlayerFavoriteFolderViewModel : ViewModelBase<VideoFavoriteFolder>
{
    [ObservableProperty]
    private bool _isSelected;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerFavoriteFolderViewModel"/> class.
    /// </summary>
    public PlayerFavoriteFolderViewModel(VideoFavoriteFolder data, bool isSelected)
        : base(data)
    {
        IsSelected = isSelected;
    }
}
