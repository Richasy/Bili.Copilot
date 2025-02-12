// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Forms;
using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Pages.Overlay;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Models.Media;
using Richasy.WinUIKernel.Share.Base;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;
using Windows.System;
using WinUIEx;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 视频播放器页面视图模型.
/// </summary>
public sealed partial class VideoPlayerPageViewModel
{
    [RelayCommand]
    private async Task ToggleFollowAsync()
    {
        try
        {
            if (IsFollow)
            {
                await _relationshipService.UnfollowUserAsync(_view.Information.Publisher.User.Id);
                IsFollow = false;
            }
            else
            {
                await _relationshipService.FollowUserAsync(_view.Information.Publisher.User.Id);
                IsFollow = true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试关注/取消关注 UP 主时失败.");
        }
    }

    [RelayCommand]
    private async Task ToggleLikeAsync()
    {
        try
        {
            var state = !IsLiked;
            await _service.ToggleVideoLikeAsync(_view.Information.Identifier.Id, state);
            IsLiked = state;
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
            await _service.CoinVideoAsync(_view.Information.Identifier.Id, count, IsCoinAlsoLike);
            IsCoined = true;
            if (IsCoinAlsoLike && !IsLiked)
            {
                IsLiked = true;
            }
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
            await _service.TripleVideoAsync(_view.Information.Identifier.Id);
            IsLiked = true;
            IsCoined = true;
            IsFavorited = true;
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
            var (folders, ids) = await _favoriteService.GetPlayingVideoFavoriteFoldersAsync(_view.Information.Identifier.Id);
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
            await _service.FavoriteVideoAsync(_view.Information.Identifier.Id, selectedFolders, unselectedFolders);
            IsFavorited = selectedFolders.Count != 0;
            FavoriteFolders = default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试收藏视频时失败.");
        }
    }

    [RelayCommand]
    private void CopyVideoUrl()
    {
        var url = GetWebLink();
        var dp = new DataPackage();
        dp.SetText(url);
        dp.SetWebLink(new Uri(url));
        Clipboard.SetContent(dp);
        this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.Copied), InfoType.Success));
    }

    [RelayCommand]
    private async Task AddToViewLaterAsync()
    {
        try
        {
            await this.Get<IViewLaterService>().AddAsync(_view.Information.Identifier.Id);
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.AddViewLaterSucceed), InfoType.Success));
        }
        catch (Exception ex)
        {
            this.Get<ILogger<VideoItemViewModel>>().LogError(ex, "添加稍后再看失败");
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.AddViewLaterFailed), InfoType.Error));
        }
    }

    [RelayCommand]
    private void ShareVideoUrl()
    {
        var handle = this.Get<AppViewModel>().ActivatedWindow.GetWindowHandle();
        var transferManager = DataTransferManagerInterop.GetForWindow(handle);
        transferManager.DataRequested += OnTransferDataRequested;
        DataTransferManagerInterop.ShowShareUIForWindow(handle);

        void OnTransferDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var url = GetWebLink();
            var dp = new DataPackage();
            dp.SetText(url);
            dp.SetWebLink(new Uri(url));
            dp.Properties.Title = Title;
            dp.Properties.Description = Description;
            dp.Properties.Thumbnail = RandomAccessStreamReference.CreateFromUri(_view.Information.Identifier.Cover.Uri);
            args.Request.Data = dp;
            sender.DataRequested -= OnTransferDataRequested;
        }
    }

    [RelayCommand]
    private Task OpenInBroswerAsync()
        => Launcher.LaunchUriAsync(new Uri(GetWebLink())).AsTask();

    [RelayCommand]
    private async Task PlayNextVideoAsync()
    {
        if (IsPageLoading || Player.IsPlayerDataLoading)
        {
            return;
        }

        var nextPart = FindNextVideo();
        if (nextPart is null)
        {
            Player.BackToDefaultModeCommand.Execute(default);
            return;
        }

        _initialProgress = 0;
        if (Player is IslandPlayerViewModel)
        {
            if (!Player.IsPaused)
            {
                Player.TogglePlayPauseCommand.Execute(default);
            }

            // 留出时间给 MPV 清理资源.
            await Task.Delay(1500);
        }

        if (nextPart is VideoPart part)
        {
            ChangePart(part);
        }
        else if (nextPart is VideoInformation video)
        {
            InitializePageCommand.Execute(new VideoSnapshot(video, IsPrivatePlay));
        }
        else
        {
            Player.BackToDefaultModeCommand.Execute(default);
        }
    }

    [RelayCommand]
    private void OpenUserSpace()
    {
        var profile = _view.Information.Publisher.User;
        this.Get<NavigationViewModel>().NavigateToOver(typeof(UserSpacePage), profile);
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

        new PlayerWindow().OpenVideo(new VideoSnapshot(_view.Information, IsPrivatePlay));
    }

    [RelayCommand]
    private void Pin()
    {
        var pinItem = new PinItem(_view.Information.Identifier.Id, _view.Information.Identifier.Title, _view.Information.Identifier.Cover.Uri.ToString(), PinContentType.Video);
        this.Get<PinnerViewModel>().AddItemCommand.Execute(pinItem);
    }

    [RelayCommand]
    private void CopyTitle()
    {
        var dp = new DataPackage();
        dp.SetText(Title);
        Clipboard.SetContent(dp);
        this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.Copied), InfoType.Success));
    }

    private string GetWebLink()
    => $"https://www.bilibili.com/video/av{_view.Information.Identifier.Id}";

    private void ChangePart(VideoPart part)
    {
        if (_initialProgress == 0)
        {
            _initialProgress = -1;
        }

        _part = part;
        AI.InjectVideoAsync(_view, part);
        Player.CancelNotification();
        Danmaku?.ResetData(_view.Information.Identifier.Id, part.Identifier.Id);
        Subtitle?.ResetData(_view.Information.Identifier.Id, part.Identifier.Id);
        InitializeDashMediaCommand.Execute(part);
        Subtitle.InitializeCommand.Execute(default);
        InitializeNextVideo();
        var partSection = Sections.FirstOrDefault(p => p is VideoPlayerPartSectionDetailViewModel) as VideoPlayerPartSectionDetailViewModel;
        partSection?.UpdateSelectedPartCommand.Execute(part);
    }

    private void PlayerProgressChanged(int progress, int duration)
    {
        if (progress < duration && progress > 1 && (Danmaku?.IsEmpty() ?? false))
        {
            Danmaku?.ResetData(_view?.Information.Identifier.Id, _part?.Identifier.Id);
            Danmaku?.LoadDanmakusCommand.Execute(duration);
        }

        Danmaku?.UpdatePosition(progress);
        Subtitle?.UpdatePosition(progress);
    }

    private void PlayerStateChanged(PlayerState state)
    {
        if (_view == null)
        {
            return;
        }

        if (state == PlayerState.Playing)
        {
            Danmaku?.Resume();
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
            Subtitle.ClearSubtitle();

            ReportProgressCommand.Execute(Player.Duration);

            // 单视频循环.
            if (CurrentLoop == VideoLoopType.Single)
            {
                Player.RestartCommand.Execute(default);
                return;
            }

            var autoNext = SettingsToolkit.ReadLocalSetting(SettingNames.AutoPlayNext, true);
            if ((!autoNext || !HasNextVideo) && !_isFormatChanging)
            {
                Player.BackToDefaultModeCommand.Execute(default);
                return;
            }

            if (IsPageLoading || Player.IsPlayerDataLoading)
            {
                return;
            }

            var next = FindNextVideo();
            if (next is null)
            {
                return;
            }

            var withoutTip = SettingsToolkit.ReadLocalSetting(SettingNames.PlayNextWithoutTip, false);
            if (withoutTip)
            {
                PlayNextVideoCommand.Execute(default);
            }
            else
            {
                string tip = default;
                if (next is VideoPart part)
                {
                    tip = string.Format(ResourceToolkit.GetLocalizedString(StringNames.NextVideoNotificationTemplate), part.Identifier.Title);
                }
                else if (next is VideoInformation video)
                {
                    tip = string.Format(ResourceToolkit.GetLocalizedString(StringNames.NextVideoNotificationTemplate), video.Identifier.Title);
                }

                var notification = new PlayerNotification(() => PlayNextVideoCommand.Execute(default), tip, 5);
                Player.ShowNotification(notification);
            }
        });
    }
}
