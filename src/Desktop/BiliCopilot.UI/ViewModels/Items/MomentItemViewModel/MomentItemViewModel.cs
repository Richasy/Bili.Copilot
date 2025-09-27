// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Forms;
using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Pages.Overlay;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Moment;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Models.Appearance;
using Richasy.BiliKernel.Models.Media;
using Richasy.BiliKernel.Models.Moment;
using Richasy.WinUIKernel.Share.Base;
using Richasy.WinUIKernel.Share.ViewModels;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;
using WinRT;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 动态条目视图模型.
/// </summary>
[GeneratedBindableCustomProperty]
public sealed partial class MomentItemViewModel : ViewModelBase<MomentInformation>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MomentItemViewModel"/> class.
    /// </summary>
    public MomentItemViewModel(MomentInformation data, MomentCardStyle style, Action<MomentItemViewModel> showCommentAction)
        : base(data)
    {
        _showMomentAction = showCommentAction;
        _operationService = this.Get<IMomentOperationService>();
        _logger = this.Get<ILogger<MomentItemViewModel>>();
        IsLiked = data.CommunityInformation?.IsLiked ?? false;
        LikeCount = data.CommunityInformation?.LikeCount;
        CommentCount = data.CommunityInformation?.CommentCount;
        Author = data.User?.Name;
        Avatar = data.User?.Avatar?.Uri;
        Tip = data.Tip;
        Description = data.Description;
        NoData = data.Data is null;
        Style = style;

        if (!NoData)
        {
            if (data.Data is MomentInformation forward)
            {
                InnerContent = new MomentItemViewModel(forward, Style, showCommentAction);
            }
            else if (data.Data is VideoInformation video)
            {
                InnerContent = new VideoItemViewModel(video, VideoCardStyle.Moment);
            }
            else if (data.Data is EpisodeInformation episode)
            {
                InnerContent = new EpisodeItemViewModel(episode, EpisodeCardStyle.Moment);
            }
            else if (data.Data is IEnumerable<BiliImage> images)
            {
                InnerContent = images;
            }

            if (FindInnerContent<VideoInformation>() is VideoInformation vinfo)
            {
                VideoTitle = vinfo.Identifier.Title ?? ResourceToolkit.GetLocalizedString(StringNames.NoTitleVideo);
                VideoCover = vinfo.Identifier.Cover.Uri;
                VideoDuration = AppToolkit.FormatDuration(TimeSpan.FromSeconds(vinfo.Duration!.Value));
            }
            else if (FindInnerContent<EpisodeInformation>() is EpisodeInformation einfo)
            {
                IsPgc = true;
                VideoTitle = einfo.Identifier.Title;
                VideoCover = einfo.Identifier.Cover.Uri;
                VideoDuration = AppToolkit.FormatDuration(TimeSpan.FromSeconds(einfo.Duration!.Value));
            }
        }
    }

    [RelayCommand]
    private async Task ToggleLikeAsync()
    {
        var state = !IsLiked;
        try
        {
            if (state)
            {
                await _operationService.LikeMomentAsync(Data);
            }
            else
            {
                await _operationService.DislikeMomentAsync(Data);
            }

            IsLiked = state;
            LikeCount += state ? 1 : -1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "切换评论点赞状态时失败");
        }
    }

    [RelayCommand]
    private void ShowComment()
        => _showMomentAction?.Invoke(this);

    [RelayCommand]
    private void Activate()
    {
        if (FindInnerContent<VideoInformation>() is VideoInformation vinfo)
        {
            if (!TryOpenInNewWindowIfPreferred() && !TryOpenInWebPlayerIfPreferred())
            {
                var snapshot = new MediaSnapshot(vinfo);
                this.Get<NavigationViewModel>().NavigateToOver(typeof(VideoPlayerPage), snapshot);
            }
        }
        else if (FindInnerContent<EpisodeInformation>() is EpisodeInformation einfo)
        {
            if (!TryOpenInNewWindowIfPreferred() && !TryOpenInWebPlayerIfPreferred())
            {
                var hasEpid = einfo.Identifier.Id != "0";
                if (hasEpid)
                {
                    var identifier = new MediaIdentifier("ep_" + einfo.Identifier.Id, default, default);
                    this.Get<NavigationViewModel>().NavigateToOver(typeof(PgcPlayerPage), identifier);
                }
                else
                {
                    // 出差番剧，使用网页打开.
                    OpenInBroswerCommand.Execute(default);
                }
            }
        }
        else if (FindInnerContent<LiveInformation>() is LiveInformation linfo && !TryOpenInNewWindowIfPreferred())
        {
            this.Get<NavigationViewModel>().NavigateToOver(typeof(LivePlayerPage), linfo.Identifier);
        }
        else if (FindInnerContent<IEnumerable<BiliImage>>() is not IEnumerable<BiliImage>)
        {
            OpenInBroswerCommand.Execute(default);
        }

        bool TryOpenInNewWindowIfPreferred()
        {
            var preferDisplayMode = SettingsToolkit.ReadLocalSetting(SettingNames.DefaultPlayerDisplayMode, PlayerDisplayMode.Default);
            if (preferDisplayMode == PlayerDisplayMode.NewWindow)
            {
                OpenInNewWindowCommand.Execute(default);
                return true;
            }

            return false;
        }

        bool TryOpenInWebPlayerIfPreferred()
        {
            var preferPlayer = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerType, PlayerType.Island);
            if (preferPlayer == PlayerType.Web)
            {
                var webUrl = GetMediaUrl();
                if (webUrl is not null)
                {
                    this.Get<NavigationViewModel>().NavigateToOver(typeof(WebPlayerPage), webUrl);
                }

                return true;
            }

            return false;
        }
    }

    [RelayCommand]
    private void ShowUserSpace()
        => this.Get<NavigationViewModel>().NavigateToOver(typeof(UserSpacePage), Data.User);

    [RelayCommand]
    private void PlayInPrivate()
    {
        var vinfo = FindInnerContent<VideoInformation>();
        if (vinfo is null)
        {
            return;
        }

        var snapshot = new MediaSnapshot(vinfo, true);
        this.Get<NavigationViewModel>().NavigateToOver(typeof(VideoPlayerPage), snapshot);
    }

    [RelayCommand]
    private async Task AddToViewLaterAsync()
    {
        var vinfo = FindInnerContent<VideoInformation>();
        var aid = string.Empty;
        if (vinfo is null)
        {
            var einfo = FindInnerContent<EpisodeInformation>();
            if (einfo is null)
            {
                return;
            }

            aid = einfo.GetExtensionIfNotNull<long>(EpisodeExtensionDataId.Aid).ToString();
        }
        else
        {
            aid = vinfo.Identifier.Id;
        }

        try
        {
            await this.Get<IViewLaterService>().AddAsync(aid);
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.AddViewLaterSucceed), InfoType.Success));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "添加稍后再看失败");
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.AddViewLaterFailed), InfoType.Error));
        }
    }

    [RelayCommand]
    private async Task OpenInBroswerAsync()
    {
        var url = GetMediaUrl() ?? $"https://t.bilibili.com/{Data.Id}";
        await Launcher.LaunchUriAsync(new Uri(url));
    }

    [RelayCommand]
    private void OpenInNewWindow()
    {
        if (FindInnerContent<VideoInformation>() is VideoInformation vinfo)
        {
            new OldPlayerWindow().OpenVideo(new MediaSnapshot(vinfo));
        }
        else if (FindInnerContent<EpisodeInformation>() is EpisodeInformation einfo)
        {
            if (einfo.Identifier.Id == "0")
            {
                // 出差番剧.
                new OldPlayerWindow().OpenPgc(new MediaIdentifier($"ss_{einfo.GetExtensionIfNotNull<long>(EpisodeExtensionDataId.SeasonId)}", default, default));
            }
            else
            {
                new OldPlayerWindow().OpenPgc(new MediaIdentifier($"ep_{einfo.Identifier.Id}", default, default));
            }
        }
        else if (FindInnerContent<LiveInformation>() is LiveInformation linfo)
        {
            new OldPlayerWindow().OpenLive(linfo.Identifier);
        }
    }

    [RelayCommand]
    private void CopyUrl()
    {
        var url = GetMediaUrl();
        if (url is not null)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(url);
            dataPackage.SetWebLink(new Uri(url));
            Clipboard.SetContent(dataPackage);
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.Copied), InfoType.Success));
        }
    }

    [RelayCommand]
    private void Pin()
    {
        PinItem pinItem = default;
        if (FindInnerContent<VideoInformation>() is VideoInformation vinfo)
        {
            pinItem = new PinItem(vinfo.Identifier.Id, vinfo.Identifier.Title, vinfo.Identifier.Cover.Uri.ToString(), PinContentType.Video);
        }
        else if (FindInnerContent<EpisodeInformation>() is EpisodeInformation einfo)
        {
            pinItem = einfo.Identifier.Id == "0"
                ? new PinItem($"ss_{einfo.GetExtensionIfNotNull<long>(EpisodeExtensionDataId.SeasonId)}", einfo.Identifier.Title, einfo.Identifier.Cover.Uri.ToString(), PinContentType.Pgc)
                : new PinItem($"ep_{einfo.Identifier.Id}", einfo.Identifier.Title, einfo.Identifier.Cover.Uri.ToString(), PinContentType.Pgc);
        }

        if (pinItem is not null)
        {
            this.Get<PinnerViewModel>().AddItemCommand.Execute(pinItem);
        }
    }

    private string? GetMediaUrl()
    {
        if (FindInnerContent<VideoInformation>() is VideoInformation vinfo)
        {
            return $"https://www.bilibili.com/av{vinfo.Identifier.Id}";
        }
        else if (FindInnerContent<EpisodeInformation>() is EpisodeInformation episodeInformation)
        {
            if (episodeInformation.Identifier.Id == "0")
            {
                // 出差番剧.
                var ssid = episodeInformation.GetExtensionIfNotNull<long>(EpisodeExtensionDataId.SeasonId);
                return $"https://www.bilibili.com/bangumi/play/ss{ssid}";
            }

            return $"https://www.bilibili.com/bangumi/play/ep{episodeInformation.Identifier.Id}";
        }

        return default;
    }

    private T? FindInnerContent<T>()
        where T : class
    {
        if (Data.Data is T info)
        {
            return info;
        }
        else if (Data.Data is MomentInformation minfo && minfo.Data is T info2)
        {
            return info2;
        }

        return default;
    }
}
