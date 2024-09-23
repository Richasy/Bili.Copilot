// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Forms;
using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Pages.Overlay;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Models.Media;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;
using Windows.System;
using WinUIEx;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// PGC 播放器页面视图模型.
/// </summary>
public sealed partial class PgcPlayerPageViewModel
{
    [RelayCommand]
    private async Task ToggleLikeAsync()
    {
        try
        {
            var state = !IsLiked;
            var aid = _episode.GetExtensionIfNotNull<long>(EpisodeExtensionDataId.Aid).ToString();
            await _service.ToggleVideoLikeAsync(aid, state);
            IsLiked = state;
            ReloadEpisodeOpeartionCommand.Execute(default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试点赞/取消点赞视频时失败.");
        }
    }

    [RelayCommand]
    private async Task CoinAsync(int count = 1)
    {
        try
        {
            var aid = _episode.GetExtensionIfNotNull<long>(EpisodeExtensionDataId.Aid).ToString();
            await _service.CoinVideoAsync(aid, count, IsCoinAlsoLike);
            IsCoined = true;
            if (IsCoinAlsoLike && !IsLiked)
            {
                IsLiked = true;
            }

            ReloadEpisodeOpeartionCommand.Execute(default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试投币/取消投币视频时失败.");
        }
    }

    [RelayCommand]
    private async Task TripleAsync()
    {
        try
        {
            var aid = _episode.GetExtensionIfNotNull<long>(EpisodeExtensionDataId.Aid).ToString();
            await _service.TripleVideoAsync(aid);
            IsLiked = true;
            IsCoined = true;
            IsFavorited = true;
            ReloadEpisodeOpeartionCommand.Execute(default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试一键三连视频时失败.");
        }
    }

    [RelayCommand]
    private async Task InitializeFavoritesAsync()
    {
        if (FavoriteFolders is not null)
        {
            return;
        }

        try
        {
            var aid = _episode.GetExtensionIfNotNull<long>(EpisodeExtensionDataId.Aid).ToString();
            var (folders, ids) = await _favoriteService.GetPlayingVideoFavoriteFoldersAsync(aid);
            FavoriteFolders = folders.Select(p => new PlayerFavoriteFolderViewModel(p, ids.Contains(p.Id))).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试获取收藏夹列表时失败.");
        }
    }

    [RelayCommand]
    private async Task FavoriteAsync()
    {
        if (FavoriteFolders is null)
        {
            return;
        }

        var selectedFolders = FavoriteFolders?.Where(p => p.IsSelected).Select(p => p.Data.Id).ToList();
        var unselectedFolders = FavoriteFolders?.Where(p => !p.IsSelected).Select(p => p.Data.Id).ToList();
        try
        {
            var aid = _episode.GetExtensionIfNotNull<long>(EpisodeExtensionDataId.Aid).ToString();
            await _service.FavoriteVideoAsync(aid, selectedFolders, unselectedFolders);
            IsFavorited = selectedFolders.Any();
            FavoriteFolders = default;
            ReloadEpisodeOpeartionCommand.Execute(default);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试收藏视频时失败.");
        }
    }

    [RelayCommand]
    private void CopyEpisodeUrl()
    {
        var url = GetEpisodeUrl();
        var dp = new DataPackage();
        dp.SetText(url);
        dp.SetWebLink(new Uri(url));
        Clipboard.SetContent(dp);
        this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.Copied), InfoType.Success));
    }

    [RelayCommand]
    private void CopySeasonUrl()
    {
        var url = GetSeasonUrl();
        var dp = new DataPackage();
        dp.SetText(url);
        dp.SetWebLink(new Uri(url));
        Clipboard.SetContent(dp);
        this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.Copied), InfoType.Success));
    }

    [RelayCommand]
    private void ShareEpisodeUrl()
    {
        var handle = this.Get<AppViewModel>().ActivatedWindow.GetWindowHandle();
        var transferManager = DataTransferManagerInterop.GetForWindow(handle);
        transferManager.DataRequested += OnTransferDataRequested;
        DataTransferManagerInterop.ShowShareUIForWindow(handle);

        void OnTransferDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var url = GetEpisodeUrl();
            var dp = new DataPackage();
            dp.SetText(url);
            dp.SetWebLink(new Uri(url));
            dp.Properties.Title = EpisodeTitle;
            dp.Properties.Description = Description;
            dp.Properties.Thumbnail = RandomAccessStreamReference.CreateFromUri(_view.Information.Identifier.Cover.Uri);
            args.Request.Data = dp;
            sender.DataRequested -= OnTransferDataRequested;
        }
    }

    [RelayCommand]
    private async Task OpenInBroswerAsync()
    {
        var url = GetEpisodeUrl();
        await Launcher.LaunchUriAsync(new Uri(url)).AsTask();
    }

    [RelayCommand]
    private void OpenInNewWindow()
    {
        if (Player.Position > 0)
        {
            ReportProgressCommand.Execute(Player.Position);
        }

        if (!Player.IsPaused)
        {
            Player.TogglePlayPauseCommand.Execute(default);
        }

        var identifier = new MediaIdentifier("ep_" + _episode.Identifier.Id, _episode.Identifier.Title, _episode.Identifier.Cover);
        new PlayerWindow().OpenPgc(identifier);
    }

    [RelayCommand]
    private void PlayNextEpisode()
    {
        if (IsPageLoading || Player.IsPlayerDataLoading)
        {
            return;
        }

        var nextPart = FindNextEpisode();
        if (nextPart is null)
        {
            Player.BackToDefaultModeCommand.Execute(default);
            return;
        }

        if (nextPart is EpisodeInformation part)
        {
            ChangeEpisode(part);
        }
        else if (nextPart is VideoInformation video)
        {
            this.Get<NavigationViewModel>().NavigateToOver(typeof(VideoPlayerPage), new VideoSnapshot(video));
        }
        else
        {
            Player.BackToDefaultModeCommand.Execute(default);
        }
    }

    [RelayCommand]
    private async Task ReloadEpisodeOpeartionAsync()
    {
        if (_episode is null)
        {
            return;
        }

        try
        {
            var operation = await _service.GetEpisodeOperationInformationAsync(_episode.Identifier.Id);
            IsLiked = operation.IsLiked;
            IsCoined = operation.IsCoined;
            IsFavorited = operation.IsFavorited;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试获取剧集操作信息时失败.");
        }
    }

    [RelayCommand]
    private async Task ToggleFollowAsync()
    {
        try
        {
            var state = !IsFollow;
            if (state)
            {
                await _discoveryService.FollowAsync(_view.Information.Identifier.Id);
            }
            else
            {
                await _discoveryService.UnfollowAsync(_view.Information.Identifier.Id);
            }

            IsFollow = state;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"尝试追番/追剧失败 ({!IsFollow})");
        }
    }

    private void ChangeEpisode(EpisodeInformation episode)
    {
        if (_initialProgress == 0)
        {
            _initialProgress = -1;
        }

        var isFirstSet = _episode is null;
        _episode = episode;
        if (isFirstSet)
        {
            LoadInitialProgress();
        }

        if (_injectedProgress is not null)
        {
            _initialProgress = _injectedProgress.Value;
            _injectedProgress = null;
        }

        Player.CancelNotification();
        EpisodeId = episode.Identifier.Id;
        EpisodeTitle = SeasonTitle + " - " + episode.Identifier.Title;
        Player.Title = EpisodeTitle;
        var aid = episode.GetExtensionIfNotNull<long>(EpisodeExtensionDataId.Aid).ToString();
        var cid = episode.GetExtensionIfNotNull<long>(EpisodeExtensionDataId.Cid).ToString();
        _comments.Initialize(aid, Richasy.BiliKernel.Models.CommentTargetType.Video, Richasy.BiliKernel.Models.CommentSortType.Hot);
        Subtitle?.ResetData(aid, cid);
        CalcPlayerHeight();
        ReloadEpisodeOpeartionCommand.Execute(default);
        InitializeDashMediaCommand.Execute(episode);
        Subtitle.InitializeCommand.Execute(default);
        InitializeNextEpisode();
    }

    [RelayCommand]
    private void Pin()
    {
        var pinItem = new PinItem($"ss_{_view.Information.Identifier.Id}", _view.Information.Identifier.Title, _view.Information.Identifier.Cover.Uri.ToString(), PinContentType.Pgc);
        this.Get<PinnerViewModel>().AddItemCommand.Execute(pinItem);
    }

    private void PlayerProgressChanged(int progress, int duration)
    {
        if (progress < duration && progress > 1 && (Danmaku?.IsEmpty() ?? false))
        {
            var aid = _episode.GetExtensionIfNotNull<long>(EpisodeExtensionDataId.Aid).ToString();
            var cid = _episode.GetExtensionIfNotNull<long>(EpisodeExtensionDataId.Cid).ToString();
            Danmaku.ResetData(aid, cid);
            Danmaku.Redraw(true);
        }

        Danmaku?.UpdatePosition(progress);
        Subtitle?.UpdatePosition(progress);
    }

    private void PlayerStateChanged(PlayerState state)
    {
        if (_view is null)
        {
            return;
        }

        if (state == PlayerState.Playing)
        {
            if (!Danmaku.IsEmpty())
            {
                Danmaku?.Resume();
            }
        }
        else
        {
            if (state == PlayerState.Paused)
            {
                // 记录播放进度.
                ReportProgressCommand.Execute(Player.Position);
            }

            Danmaku?.Pause();
        }
    }

    private void PlayerSpeedChanged(double speed)
    {
        if (Danmaku is not null)
        {
            Danmaku.ExtraSpeed = speed;
        }
    }

    private void PlayerMediaEnded()
    {
        this.Get<Microsoft.UI.Dispatching.DispatcherQueue>().TryEnqueue(() =>
        {
            // 清除弹幕.
            Danmaku.ClearDanmaku();
            Subtitle.ClearSubttile();

            ReportProgressCommand.Execute(Player.Duration);

            var autoNext = SettingsToolkit.ReadLocalSetting(SettingNames.AutoPlayNext, true);
            if ((!autoNext || !HasNextEpisode) && !_isFormatChanging)
            {
                Player.BackToDefaultModeCommand.Execute(default);
                return;
            }

            if (IsPageLoading || Player.IsPlayerDataLoading)
            {
                return;
            }

            var next = FindNextEpisode();
            if (next is null)
            {
                return;
            }

            var withoutTip = SettingsToolkit.ReadLocalSetting(SettingNames.PlayNextWithoutTip, false);
            if (withoutTip)
            {
                PlayNextEpisode();
            }
            else
            {
                string tip = default;
                if (next is EpisodeInformation episode)
                {
                    tip = string.Format(ResourceToolkit.GetLocalizedString(StringNames.NextEpisodeNotificationTemplate), episode.Identifier.Title);
                }
                else if (next is VideoInformation video)
                {
                    tip = string.Format(ResourceToolkit.GetLocalizedString(StringNames.NextVideoNotificationTemplate), video.Identifier.Title);
                }

                var notification = new PlayerNotification(PlayNextEpisode, tip, 5);
                Player.ShowNotification(notification);
            }
        });
    }

    private string GetSeasonUrl()
        => $"https://www.bilibili.com/bangumi/play/ss{_view.Information.Identifier.Id}";

    private string GetEpisodeUrl()
        => $"https://www.bilibili.com/bangumi/play/ep{_episode.Identifier.Id}";
}
