// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.App.Controls.Base;
using Bili.Copilot.Models.Data.Video;
using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Modules;

/// <summary>
/// 默认视频收藏夹模块.
/// </summary>
public sealed partial class VideoFavoriteModule : VideoFavoriteModuleBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoFavoriteModule"/> class.
    /// </summary>
    public VideoFavoriteModule()
    {
        InitializeComponent();
        ViewModel = VideoFavoriteDetailViewModel.Instance;
    }

    private void OnVideoViewIncrementalTriggered(object sender, System.EventArgs e)
        => ViewModel.IncrementalCommand.Execute(default);

    private void OnFolderComboBoxSelectionChanged(object sender, Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e)
    {
        var item = (sender as Microsoft.UI.Xaml.Controls.ComboBox)?.SelectedItem as VideoFavoriteFolder;
        ViewModel.SelectFolderCommand.Execute(item);
    }
}

/// <summary>
/// <see cref="VideoFavoriteModule"/> 的基类.
/// </summary>
public abstract class VideoFavoriteModuleBase : ReactiveUserControl<VideoFavoriteDetailViewModel>
{
}
