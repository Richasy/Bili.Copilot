// Copyright (c) Bili Copilot. All rights reserved.

using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 视频收藏夹详情视图模型.
/// </summary>
public sealed partial class DefaultVideoFavoriteDetailViewModel : InformationFlowViewModel<VideoItemViewModel>
{
    /// <inheritdoc/>
    protected override void BeforeReload()
    {
        FavoriteProvider.Instance.ResetVideoFolderDetailStatus();
        IsEmpty = false;
        _isEnd = false;
        _defaultFolder = default;
    }

    /// <inheritdoc/>
    protected override async Task GetDataAsync()
    {
        if (_isEnd)
        {
            return;
        }

        if (_defaultFolder == null)
        {
            var folders = await FavoriteProvider.Instance.GetVideoFavoriteViewAsync(AuthorizeProvider.Instance.CurrentUserId);
            _defaultFolder = folders.DefaultFolder.Folder;
        }

        IsEmpty = _defaultFolder.TotalCount == 0;
        if (IsEmpty)
        {
            return;
        }

        var data = await FavoriteProvider.Instance.GetVideoFavoriteFolderDetailAsync(_defaultFolder.Id);
        foreach (var item in data.VideoSet.Items)
        {
            var videoVM = new VideoItemViewModel(item, RemoveVideo, _defaultFolder.Id);
            Items.Add(videoVM);
        }

        _isEnd = Items.Count == data.VideoSet.TotalCount;
        IsEmpty = Items.Count == 0;
    }

    /// <inheritdoc/>
    protected override string FormatException(string errorMsg)
        => $"{ResourceToolkit.GetLocalizedString(Models.Constants.App.StringNames.RequestVideoFavoriteFailed)}\n{errorMsg}";

    private void RemoveVideo(VideoItemViewModel vm)
    {
        Items.Remove(vm);
        IsEmpty = Items.Count == 0;
    }
}
