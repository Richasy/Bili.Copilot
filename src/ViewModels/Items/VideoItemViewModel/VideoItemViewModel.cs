// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Authorize;
using Bili.Copilot.Models.Data.Local;
using Bili.Copilot.Models.Data.Video;
using CommunityToolkit.Mvvm.Input;
using Windows.System;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 视频项视图模型.
/// </summary>
public sealed partial class VideoItemViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoItemViewModel"/> class.
    /// </summary>
    /// <param name="information">视频信息.</param>
    public VideoItemViewModel(
        VideoInformation information,
        Action<VideoItemViewModel> playAction = default,
        Action<VideoItemViewModel> additionalAction = default,
        object additionalData = default)
    {
        Data = information;
        _playAction = playAction;
        _additionalAction = additionalAction;
        _additionalData = additionalData;
        CanRemove = true;
        InitializeData();
    }

    [RelayCommand]
    private void Play()
    {
        if (_playAction != null)
        {
            _playAction(this);
        }
        else
        {
            AppViewModel.Instance.OpenPlayerCommand.Execute(GetSnapshot());
        }
    }

    [RelayCommand]
    private void PlayInPrivate()
        => AppViewModel.Instance.OpenPlayerCommand.Execute(GetSnapshot(true));

    [RelayCommand]
    private async Task AddToViewLaterAsync()
    {
        if (AuthorizeProvider.Instance.State == AuthorizeState.SignedIn)
        {
            var result = await AccountProvider.AddVideoToViewLaterAsync(Data.Identifier.Id);
            if (result)
            {
                // 显示添加成功的消息.
                AppViewModel.Instance.ShowTip(
                    ResourceToolkit.GetLocalizedString(StringNames.AddViewLaterSucceseded),
                    InfoType.Success);
            }
            else
            {
                // 显示添加失败的消息.
                AppViewModel.Instance.ShowTip(
                    ResourceToolkit.GetLocalizedString(StringNames.AddViewLaterFailed),
                    InfoType.Error);
            }
        }
        else
        {
            // 显示需要登录的消息.
            AppViewModel.Instance.ShowTip(
                    ResourceToolkit.GetLocalizedString(StringNames.NeedLoginFirst),
                    InfoType.Warning);
        }
    }

    [RelayCommand]
    private async Task RemoveFromViewLaterAsync()
    {
        if (AuthorizeProvider.Instance.State == AuthorizeState.SignedIn)
        {
            var result = await AccountProvider.RemoveVideoFromViewLaterAsync(Data.Identifier.Id);
            if (!result)
            {
                // 显示移除失败的消息.
                AppViewModel.Instance.ShowTip(
                    ResourceToolkit.GetLocalizedString(StringNames.RemoveViewLaterFailed),
                    InfoType.Error);
            }
            else
            {
                _additionalAction?.Invoke(this);
            }
        }
        else
        {
            // 显示需要登录的消息.
            AppViewModel.Instance.ShowTip(
                    ResourceToolkit.GetLocalizedString(StringNames.NeedLoginFirst),
                    InfoType.Warning);
        }
    }

    [RelayCommand]
    private async Task RemoveFromHistoryAsync()
    {
        if (AuthorizeProvider.Instance.State == AuthorizeState.SignedIn)
        {
            var result = await AccountProvider.RemoveHistoryItemAsync(Data.Identifier.Id);
            if (!result)
            {
                // 显示移除失败的消息.
                AppViewModel.Instance.ShowTip(
                    ResourceToolkit.GetLocalizedString(StringNames.FailedToRemoveVideoFromHistory),
                    InfoType.Error);
            }
            else
            {
                _additionalAction?.Invoke(this);
            }
        }
        else
        {
            // 显示需要登录的消息.
            AppViewModel.Instance.ShowTip(
                    ResourceToolkit.GetLocalizedString(StringNames.NeedLoginFirst),
                    InfoType.Warning);
        }
    }

    [RelayCommand]
    private async Task RemoveFromFavoriteAsync()
    {
        if (AuthorizeProvider.Instance.State == AuthorizeState.SignedIn)
        {
            var folderId = _additionalData.ToString();
            var result = await FavoriteProvider.RemoveFavoriteVideoAsync(folderId, Data.Identifier.Id);
            if (!result)
            {
                // 显示移除失败的消息.
                AppViewModel.Instance.ShowTip(
                    ResourceToolkit.GetLocalizedString(StringNames.FailedToRemoveVideoFromFavorite),
                    InfoType.Error);
            }
            else
            {
                _additionalAction?.Invoke(this);
            }
        }
        else
        {
            // 显示需要登录的消息.
            AppViewModel.Instance.ShowTip(
                    ResourceToolkit.GetLocalizedString(StringNames.NeedLoginFirst),
                    InfoType.Warning);
        }
    }

    [RelayCommand]
    private async Task OpenInBrowserAsync()
    {
        var uri = $"https://www.bilibili.com/video/av{Data.Identifier.Id}";
        await Launcher.LaunchUriAsync(new Uri(uri));
    }

    private void InitializeData()
    {
        IsShowCommunity = Data.CommunityInformation != null;
        var userVM = new UserItemViewModel(Data.Publisher);
        Publisher = userVM;
        if (IsShowCommunity)
        {
            PlayCountText = NumberToolkit.GetCountText(Data.CommunityInformation.PlayCount);
            DanmakuCountText = NumberToolkit.GetCountText(Data.CommunityInformation.DanmakuCount);
            LikeCountText = NumberToolkit.GetCountText(Data.CommunityInformation.LikeCount);

            IsShowScore = Data.CommunityInformation?.Score > 0;
            ScoreText = IsShowScore ?
                Data.CommunityInformation.Score.ToString("0")
                : default;
        }

        if (Data.Identifier.Duration > 0)
        {
            DurationText = NumberToolkit.GetDurationText(TimeSpan.FromSeconds(Data.Identifier.Duration));
        }
    }

    private PlaySnapshot GetSnapshot(bool isInPrivate = false)
    {
        var id = string.IsNullOrEmpty(Data.AlternateId) ? Data.Identifier.Id : Data.AlternateId;
        var snapshot = new PlaySnapshot(id, "0", Models.Constants.Bili.VideoType.Video)
        {
            Title = Data.Identifier.Title,
            IsInPrivate = isInPrivate,
        };

        return snapshot;
    }
}
