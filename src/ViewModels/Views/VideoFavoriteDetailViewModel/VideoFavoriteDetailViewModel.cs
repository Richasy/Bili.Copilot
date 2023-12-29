// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Data.Local;
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
        => Folders = new ObservableCollection<VideoFavoriteFolderSelectableViewModel>();

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
            var folderList = folders.Groups.SelectMany(p => p.FavoriteSet.Items).ToList();
            folderList.Insert(0, folders.DefaultFolder.Folder);
            foreach (var folder in folderList)
            {
                if (Folders.Any(p => p.Equals(folder)))
                {
                    return;
                }

                Folders.Add(new VideoFavoriteFolderSelectableViewModel(folder));
            }

            var canSelectFolder = SettingsToolkit.ReadLocalSetting(SettingNames.LastFavoriteType, FavoriteType.Video) == FavoriteType.Video;
            if (canSelectFolder)
            {
                var lastFolderId = SettingsToolkit.ReadLocalSetting(SettingNames.LastVideoFavoriteFolderId, string.Empty);
                if (!string.IsNullOrEmpty(lastFolderId))
                {
                    CurrentFolder = Folders.FirstOrDefault(p => p.Data.Id == lastFolderId);
                }

                CurrentFolder ??= Folders.First();
                CurrentFolder.IsSelected = true;
                CheckFolderIsMine();
            }
        }

        if (CurrentFolder != null)
        {
            IsEmpty = CurrentFolder.Data.TotalCount == 0;
            if (IsEmpty)
            {
                return;
            }

            var data = await FavoriteProvider.Instance.GetVideoFavoriteFolderDetailAsync(CurrentFolder.Data.Id);
            foreach (var item in data.VideoSet.Items)
            {
                if (Items.Any(p => p.Data.Identifier.Id == item.Identifier.Id))
                {
                    continue;
                }

                var videoVM = new VideoItemViewModel(item, additionalAction: RemoveVideo, additionalData: CurrentFolder.Data.Id)
                {
                    CanRemove = IsMine,
                };
                Items.Add(videoVM);
            }

            _isEnd = Items.Count == data.VideoSet.TotalCount;
        }

        IsEmpty = Items.Count == 0;
    }

    /// <inheritdoc/>
    protected override string FormatException(string errorMsg)
        => $"{ResourceToolkit.GetLocalizedString(StringNames.RequestVideoFavoriteFailed)}\n{errorMsg}";

    [RelayCommand]
    private async Task SelectFolderAsync(VideoFavoriteFolderSelectableViewModel folder)
    {
        if (folder == null)
        {
            foreach (var item in Folders)
            {
                item.IsSelected = false;
            }

            return;
        }

        CurrentFolder = folder;
        foreach (var item in Folders)
        {
            item.IsSelected = item.Equals(folder);
        }

        CheckFolderIsMine();
        _isEnd = false;
        FavoriteProvider.Instance.ResetVideoFolderDetailStatus();
        IsEmpty = false;
        _isEnd = false;
        TryClear(Items);
        await GetDataAsync();
    }

    [RelayCommand]
    private void PlayAll()
    {
        var filteredItems = Items.Where(x => x.Data.Identifier.Title != "已失效视频").Select(p => p.Data).ToList();
        if (filteredItems.Count > 1)
        {
            AppViewModel.Instance.OpenPlaylistCommand.Execute(filteredItems);
        }
        else if (filteredItems.Count > 0)
        {
            var info = filteredItems.First();
            AppViewModel.Instance.OpenPlayerCommand.Execute(new PlaySnapshot(info.Identifier.Id, "0", VideoType.Video));
        }
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
        IsMine = CurrentFolder.Data.Id == _view.DefaultFolder.Folder.Id || mineFolders.Any(p => p.Id == CurrentFolder.Data.Id);
    }

    partial void OnCurrentFolderChanged(VideoFavoriteFolderSelectableViewModel value)
    {
        if (value == null)
        {
            SettingsToolkit.DeleteLocalSetting(SettingNames.LastVideoFavoriteFolderId);
            return;
        }

        SettingsToolkit.WriteLocalSetting(SettingNames.LastVideoFavoriteFolderId, value.Data.Id);
    }
}
