// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Video;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 视频收藏夹详情视图模型.
/// </summary>
public sealed partial class VideoFavoriteDetailViewModel : InformationFlowViewModel<VideoItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoFavoriteDetailViewModel"/> class.
    /// </summary>
    private VideoFavoriteDetailViewModel()
        => Folders = new ObservableCollection<VideoFavoriteFolder>();

    /// <inheritdoc/>
    protected override void BeforeReload()
    {
        FavoriteProvider.Instance.ResetVideoFolderDetailStatus();
        IsEmpty = false;
        _isEnd = false;
        _view = default;
        IsMine = false;
        TryClear(Folders);
        CurrentFolder = default;
    }

    /// <inheritdoc/>
    protected override async Task GetDataAsync()
    {
        if (_isEnd)
        {
            return;
        }

        if (CurrentFolder == null)
        {
            var folders = await FavoriteProvider.Instance.GetVideoFavoriteViewAsync(AuthorizeProvider.Instance.CurrentUserId);
            _view = folders;
            Folders.Add(folders.DefaultFolder.Folder);
            var folderList = folders.Groups.SelectMany(p => p.FavoriteSet.Items).ToList();
            foreach (var folder in folderList)
            {
                if (Folders.Any(p => p.Equals(folder)))
                {
                    return;
                }

                Folders.Add(folder);
            }

            CurrentFolder = Folders.First();
            CheckFolderIsMine();
        }

        IsEmpty = CurrentFolder.TotalCount == 0;
        if (IsEmpty)
        {
            return;
        }

        var data = await FavoriteProvider.Instance.GetVideoFavoriteFolderDetailAsync(CurrentFolder.Id);
        foreach (var item in data.VideoSet.Items)
        {
            if (Items.Any(p => p.Data.Identifier.Id == item.Identifier.Id))
            {
                continue;
            }

            var videoVM = new VideoItemViewModel(item, additionalAction: RemoveVideo, additionalData: CurrentFolder.Id)
            {
                CanRemove = IsMine,
            };
            Items.Add(videoVM);
        }

        _isEnd = Items.Count == data.VideoSet.TotalCount;
        IsEmpty = Items.Count == 0;
    }

    /// <inheritdoc/>
    protected override string FormatException(string errorMsg)
        => $"{ResourceToolkit.GetLocalizedString(StringNames.RequestVideoFavoriteFailed)}\n{errorMsg}";

    [RelayCommand]
    private async Task SelectFolderAsync(VideoFavoriteFolder folder)
    {
        CurrentFolder = folder;
        CheckFolderIsMine();
        _isEnd = false;
        FavoriteProvider.Instance.ResetVideoFolderDetailStatus();
        IsEmpty = false;
        _isEnd = false;
        TryClear(Items);
        await GetDataAsync();
    }

    private void RemoveVideo(VideoItemViewModel vm)
    {
        _ = Items.Remove(vm);
        IsEmpty = Items.Count == 0;
    }

    private void CheckFolderIsMine()
    {
        if (CurrentFolder == null || _view == null)
        {
            IsMine = false;
            return;
        }

        var mineFolders = _view.Groups.Where(p => p.IsMine).SelectMany(p => p.FavoriteSet.Items);
        IsMine = CurrentFolder.Id == _view.DefaultFolder.Folder.Id || mineFolders.Any(p => p.Id == CurrentFolder.Id);
    }
}
