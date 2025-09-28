// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Pages.Overlay;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Bili.User;
using Richasy.WinUIKernel.Share.Base;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;
using Windows.System;
using WinUIEx;

namespace BiliCopilot.UI.ViewModels.Core;

public sealed partial class VideoConnectorViewModel
{
    [RelayCommand]
    private async Task ToggleFollowAsync()
    {
        try
        {
            var service = this.Get<IRelationshipService>();
            if (IsFollow)
            {
                await service.UnfollowUserAsync(_view.Information.Publisher.User.Id);
                IsFollow = false;
            }
            else
            {
                await service.FollowUserAsync(_view.Information.Publisher.User.Id);
                IsFollow = true;
            }
        }
        catch (Exception ex)
        {
            this.Get<ILogger<VideoConnectorViewModel>>().LogError(ex, "尝试关注/取消关注 UP 主时失败.");
        }
    }

    [RelayCommand]
    private async Task ToggleLikeAsync()
    {
        try
        {
            var state = !IsLiked;
            await this.Get<IPlayerService>().ToggleVideoLikeAsync(_view.Information.Identifier.Id, state);
            IsLiked = state;
        }
        catch (Exception ex)
        {
            this.Get<ILogger<VideoConnectorViewModel>>().LogError(ex, "尝试点赞/取消点赞视频时失败.");
        }
    }

    [RelayCommand]
    private async Task CoinAsync(int count = 1)
    {
        try
        {
            await this.Get<IPlayerService>().CoinVideoAsync(_view.Information.Identifier.Id, count, IsCoinAlsoLike);
            IsCoined = true;
            if (IsCoinAlsoLike && !IsLiked)
            {
                IsLiked = true;
            }
        }
        catch (Exception ex)
        {
            this.Get<ILogger<VideoConnectorViewModel>>().LogError(ex, "尝试投币/取消投币视频时失败.");
        }
    }

    [RelayCommand]
    private async Task TripleAsync()
    {
        try
        {
            await this.Get<IPlayerService>().TripleVideoAsync(_view.Information.Identifier.Id);
            IsLiked = true;
            IsCoined = true;
            IsFavorited = true;
        }
        catch (Exception ex)
        {
            this.Get<ILogger<VideoConnectorViewModel>>().LogError(ex, "尝试一键三连视频时失败.");
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
            var (folders, ids) = await this.Get<IFavoriteService>().GetPlayingVideoFavoriteFoldersAsync(_view.Information.Identifier.Id);
            FavoriteFolders = folders.Select(p => new PlayerFavoriteFolderViewModel(p, ids.Contains(p.Id))).ToList();
        }
        catch (Exception ex)
        {
            this.Get<ILogger<VideoConnectorViewModel>>().LogError(ex, "尝试获取收藏夹列表时失败.");
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
            await this.Get<IPlayerService>().FavoriteVideoAsync(_view.Information.Identifier.Id, selectedFolders, unselectedFolders);
            IsFavorited = selectedFolders.Count != 0;
            FavoriteFolders = default;
        }
        catch (Exception ex)
        {
            this.Get<ILogger<VideoConnectorViewModel>>().LogError(ex, "尝试收藏视频时失败.");
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
    private Task OpenInBrowserAsync()
        => Launcher.LaunchUriAsync(new Uri(GetWebLink())).AsTask();

    [RelayCommand]
    private void OpenUserSpace()
    {
        var profile = _view.Information.Publisher.User;
        this.Get<NavigationViewModel>().NavigateToOver(typeof(UserSpacePage), profile);
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
}
