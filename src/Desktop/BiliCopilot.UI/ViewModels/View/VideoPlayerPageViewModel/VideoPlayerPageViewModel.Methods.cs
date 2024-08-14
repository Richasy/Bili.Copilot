﻿// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Items;
using Humanizer;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Models.Media;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 视频播放器页面视图模型.
/// </summary>
public sealed partial class VideoPlayerPageViewModel
{
    private void InitializeView(VideoPlayerView view)
    {
        _view = view;
        Cover = view.Information.Identifier.Cover.SourceUri;
        Title = view.Information.Identifier.Title;
        AvId = view.Information.Identifier.Id;
        BvId = view.Information.BvId;
        IsMyVideo = _view.Information.Publisher.User.Id == this.Get<IBiliTokenResolver>().GetToken().UserId.ToString();
        UpAvatar = view.Information.Publisher.User.Avatar.Uri;
        UpName = view.Information.Publisher.User.Name;
        PublishRelativeTime = string.Format(ResourceToolkit.GetLocalizedString(StringNames.AuthorPublishTime), view.Information.PublishTime.Humanize(default, new System.Globalization.CultureInfo("zh-CN")));
        Description = view.Information.GetExtensionIfNotNull<string>(VideoExtensionDataId.Description);
        IsFollow = view.OwnerCommunity.Relation != Richasy.BiliKernel.Models.User.UserRelationStatus.Unfollow && view.OwnerCommunity.Relation != Richasy.BiliKernel.Models.User.UserRelationStatus.Unknown;
        Tags = view.Tags.ToList();
        PlayCount = view.Information.CommunityInformation.PlayCount ?? 0;
        DanmakuCount = view.Information.CommunityInformation.DanmakuCount ?? 0;
        CommentCount = view.Information.CommunityInformation.CommentCount ?? 0;
        LikeCount = view.Information.CommunityInformation.LikeCount ?? 0;
        CoinCount = view.Information.CommunityInformation.CoinCount ?? 0;
        FavoriteCount = view.Information.CommunityInformation.FavoriteCount ?? 0;
        IsLiked = view.Operation.IsLiked;
        IsCoined = view.Operation.IsCoined;
        IsFavorited = view.Operation.IsFavorited;
        IsCoinAlsoLike = true;

        CalcPlayerHeight();
    }

    private void InitializeDash(DashMediaInformation info)
    {
        _videoSegments = info.Videos;
        _audioSegments = info.Audios;
        Formats = info.Formats.Select(p => new PlayerFormatItemViewModel(p)).ToList();

        // 用户个人视频无需会员即可观看最高画质.
        if (IsMyVideo)
        {
            foreach (var format in Formats)
            {
                format.IsEnabled = true;
            }
        }

        var preferFormatSetting = SettingsToolkit.ReadLocalSetting(SettingNames.PreferQuality, PreferQualityType.Auto);
        var availableFormats = Formats.Where(p => p.IsEnabled).ToList();
        PlayerFormatItemViewModel? selectedFormat = default;
        if (preferFormatSetting == PreferQualityType.Auto)
        {
            var lastSelectedFormat = SettingsToolkit.ReadLocalSetting(SettingNames.LastSelectedVideoQuality, 0);
            selectedFormat = availableFormats.Find(p => p.Data.Quality == lastSelectedFormat);
        }
        else if (preferFormatSetting == PreferQualityType.FourK)
        {
            selectedFormat = availableFormats.Find(p => p.Data.Quality == 120);
        }
        else if (preferFormatSetting == PreferQualityType.HD)
        {
            selectedFormat = availableFormats.Find(p => p.Data.Quality == 80);
        }

        if (selectedFormat is null)
        {
            var maxQuality = availableFormats.Max(p => p.Data.Quality);
            selectedFormat = availableFormats.Find(p => p.Data.Quality == maxQuality);
        }

        ChangeFormatCommand.Execute(selectedFormat);
    }

    private void ClearView()
    {
        IsPageLoadFailed = false;
        _view = default;
        _videoSegments = default;
        _audioSegments = default;
        Cover = default;
        Title = default;
        Tags = default;
        Description = default;
        UpAvatar = default;
        UpName = default;
        IsFollow = false;
        IsMyVideo = false;
        PlayCount = 0;
        DanmakuCount = 0;
        CommentCount = 0;
        LikeCount = 0;
        CoinCount = 0;
        FavoriteCount = 0;
        IsLiked = false;
        IsCoined = false;
        IsFavorited = false;
        IsCoinAlsoLike = true;
        OnlineCountText = default;
        AvId = default;
        BvId = default;

        Formats = default;
        SelectedFormat = default;
    }

    private void CalcPlayerHeight()
    {
        if (PlayerWidth <= 0 || _view?.AspectRatio is null)
        {
            return;
        }

        PlayerHeight = (double)(PlayerWidth * _view.AspectRatio.Value.Height / _view.AspectRatio.Value.Width);
    }
}
