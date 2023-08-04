// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Constants.Community;
using Bili.Copilot.Models.Data.Appearance;
using Bili.Copilot.Models.Data.Article;
using Bili.Copilot.Models.Data.Dynamic;
using Bili.Copilot.Models.Data.Local;
using Bili.Copilot.Models.Data.Pgc;
using Bili.Copilot.Models.Data.Video;
using CommunityToolkit.Mvvm.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 动态条目的视图模型.
/// </summary>
public sealed partial class DynamicItemViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicItemViewModel"/> class.
    /// </summary>
    /// <param name="data">动态信息.</param>
    public DynamicItemViewModel(DynamicInformation data)
    {
        Data = data;
        InitializeData();
    }

    private static void ActiveData(object data)
    {
        if (data is null or IEnumerable<Image>)
        {
            return;
        }

        if (data is VideoInformation video)
        {
            var playSnapshot = new PlaySnapshot(video.Identifier.Id, "0", VideoType.Video);
            AppViewModel.Instance.OpenPlayerCommand.Execute(playSnapshot);
        }
        else if (data is EpisodeInformation episode)
        {
            var needBiliPlus = episode.Identifier.Id == "0";
            var id = needBiliPlus
                ? episode.VideoId
                : episode.Identifier.Id;

            var playSnapshot = new PlaySnapshot(id, episode.SeasonId, VideoType.Pgc)
            {
                Title = episode.Identifier.Title,
                NeedBiliPlus = needBiliPlus,
            };
            AppViewModel.Instance.OpenPlayerCommand.Execute(playSnapshot);
        }
        else if (data is ArticleInformation article)
        {
            AppViewModel.Instance.OpenReaderCommand.Execute(article);
        }
        else if (data is DynamicInformation dynamic)
        {
            ActiveData(dynamic.Data);
        }
    }

    [RelayCommand]
    private async Task ToggleLikeAsync()
    {
        var isLike = !IsLiked;
        var result = await CommunityProvider.LikeDynamicAsync(Data.Id, isLike, Publisher.User.Id, Data.CommentId);
        if (result)
        {
            IsLiked = isLike;
            if (isLike)
            {
                Data.CommunityInformation.LikeCount += 1;
            }
            else
            {
                Data.CommunityInformation.LikeCount -= 1;
            }

            LikeCountText = NumberToolkit.GetCountText(Data.CommunityInformation.LikeCount);
        }
        else
        {
            AppViewModel.Instance.ShowTip(ResourceToolkit.GetLocalizedString(StringNames.SetFailed), InfoType.Error);
        }
    }

    [RelayCommand]
    private void Active()
        => ActiveData(Data.Data);

    [RelayCommand]
    private void AddToViewLater()
    {
        if (Data.Data is VideoInformation videoInfo)
        {
            var vm = new VideoItemViewModel(videoInfo);
            vm.AddToViewLaterCommand.Execute(default);
        }
    }

    [RelayCommand]
    private void ShowUserDetail()
    {
        if (Data.User != null)
        {
            AppViewModel.Instance.ShowUserDetailCommand.Execute(Data.User);
        }
    }

    [RelayCommand]
    private void Share()
    {
        // TODO: 转换成 WinUI3 代码.
        var dataTransferManager = DataTransferManager.GetForCurrentView();
        dataTransferManager.DataRequested += OnShareDataRequested;
        DataTransferManager.ShowShareUI();
    }

    private void OnShareDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
    {
        var request = args.Request;
        var url = string.Empty;
        Uri coverUri = null;
        var title = string.Empty;

        request.Data.SetText(Data.Description?.Text ?? string.Empty);
        if (Data.DynamicType == DynamicItemType.Video)
        {
            var videoInfo = Data.Data as VideoInformation;
            title = videoInfo.Identifier.Title;
            coverUri = videoInfo.Identifier.Cover.GetSourceUri();
            url = $"https://www.bilibili.com/video/{videoInfo.AlternateId}";
        }
        else if (Data.DynamicType == DynamicItemType.Pgc)
        {
            var episodeInfo = Data.Data as EpisodeInformation;
            title = episodeInfo.Identifier.Title;
            coverUri = episodeInfo.Identifier.Cover.GetSourceUri();
            url = $"https://www.bilibili.com/bangumi/play/ss{episodeInfo.SeasonId}";
        }

        request.Data.Properties.Title = title;
        if (!string.IsNullOrEmpty(url))
        {
            request.Data.SetWebLink(new Uri(url));
        }

        if (coverUri != null)
        {
            request.Data.SetBitmap(RandomAccessStreamReference.CreateFromUri(coverUri));
        }
    }

    private void InitializeData()
    {
        IsShowCommunity = Data.CommunityInformation != null;
        if (IsShowCommunity)
        {
            IsLiked = Data.CommunityInformation.IsLiked;
            LikeCountText = NumberToolkit.GetCountText(Data.CommunityInformation.LikeCount);
            CommentCountText = NumberToolkit.GetCountText(Data.CommunityInformation.CommentCount);
        }

        if (Data.User != null)
        {
            var userVM = new UserItemViewModel(Data.User);
            Publisher = userVM;
        }

        CanAddViewLater = Data.Data is VideoInformation;
    }
}
