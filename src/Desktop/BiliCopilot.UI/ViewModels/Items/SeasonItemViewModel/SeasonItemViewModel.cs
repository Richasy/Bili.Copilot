// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Forms;
using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Pages.Overlay;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Media;
using Richasy.WinUI.Share.ViewModels;
using Windows.System;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 剧集条目视图模型.
/// </summary>
public sealed partial class SeasonItemViewModel : ViewModelBase<SeasonInformation>
{
    private readonly Action<SeasonItemViewModel>? _removeAction;

    /// <summary>
    /// Initializes a new instance of the <see cref="SeasonItemViewModel"/> class.
    /// </summary>
    public SeasonItemViewModel(SeasonInformation data, PgcFavoriteStatus? favStatus = default, Action<SeasonItemViewModel>? removeAction = default)
        : base(data)
    {
        Title = data.Identifier.Title;
        Cover = data.Identifier.Cover.Uri;
        Subtitle = data.GetExtensionIfNotNull<string>(SeasonExtensionDataId.Subtitle);
        Highlight = data.GetExtensionIfNotNull<string>(SeasonExtensionDataId.Highlight);
        Score = data.GetExtensionIfNotNull<double>(SeasonExtensionDataId.Score);
        InWantWatch = favStatus == PgcFavoriteStatus.Want;
        InWatching = favStatus == PgcFavoriteStatus.Watching;
        InWatched = favStatus == PgcFavoriteStatus.Watched;
        _removeAction = removeAction;
    }

    [RelayCommand]
    private void Play()
    {
        var preferDisplayMode = SettingsToolkit.ReadLocalSetting(SettingNames.DefaultPlayerDisplayMode, PlayerDisplayMode.Default);
        if (preferDisplayMode == PlayerDisplayMode.NewWindow)
        {
            OpenInNewWindowCommand.Execute(default);
            return;
        }

        var preferPlayer = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerType, PlayerType.Native);
        if (preferPlayer == PlayerType.Web)
        {
            this.Get<NavigationViewModel>().NavigateToOver(typeof(WebPlayerPage).FullName, GetWebUrl());
            return;
        }

        var identifier = new MediaIdentifier("ss_" + Data.Identifier.Id, default, default);
        this.Get<NavigationViewModel>().NavigateToOver(typeof(PgcPlayerPage).FullName, identifier);
    }

    [RelayCommand]
    private Task OpenInBroswerAsync()
        => Launcher.LaunchUriAsync(new Uri(GetWebUrl())).AsTask();

    [RelayCommand]
    private void OpenInNewWindow()
        => new PlayerWindow().OpenPgc(new MediaIdentifier("ss_" + Data.Identifier.Id, default, default));

    [RelayCommand]
    private async Task FollowAsync()
    {
        try
        {
            await this.Get<IEntertainmentDiscoveryService>().FollowAsync(Data.Identifier.Id);
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.Followed), InfoType.Success));
        }
        catch (Exception ex)
        {
            this.Get<ILogger<SeasonItemViewModel>>().LogError(ex, "追番/追剧失败");
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.FailedToFollowPgc), InfoType.Error));
        }
    }

    [RelayCommand]
    private async Task UnfollowAsync()
    {
        try
        {
            await this.Get<IEntertainmentDiscoveryService>().UnfollowAsync(Data.Identifier.Id);
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.Unfollowed), InfoType.Success));
        }
        catch (Exception ex)
        {
            this.Get<ILogger<SeasonItemViewModel>>().LogError(ex, "取消追番/追剧失败");
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.FailedToUnfollowPgc), InfoType.Error));
        }
    }

    [RelayCommand]
    private Task MarkWantWatchAsync()
        => MarkFavoriteStatusAsync(PgcFavoriteStatus.Want);

    [RelayCommand]
    private Task MarkWatchingAsync()
        => MarkFavoriteStatusAsync(PgcFavoriteStatus.Watching);

    [RelayCommand]
    private Task MarkWatchedAsync()
        => MarkFavoriteStatusAsync(PgcFavoriteStatus.Watched);

    [RelayCommand]
    private void Pin()
    {
        var pinItem = new PinItem($"ss_{Data.Identifier.Id}", Data.Identifier.Title, Data.Identifier.Cover.Uri.ToString(), PinContentType.Pgc);
        this.Get<PinnerViewModel>().AddItemCommand.Execute(pinItem);
    }

    private string GetWebUrl()
        => $"https://www.bilibili.com/bangumi/play/ss{Data.Identifier.Id}";

    private async Task MarkFavoriteStatusAsync(PgcFavoriteStatus status)
    {
        try
        {
            await this.Get<IFavoriteService>().MarkPgcAsync(Data.Identifier, status);
            _removeAction?.Invoke(this);
        }
        catch (Exception ex)
        {
            this.Get<ILogger<SeasonItemViewModel>>().LogError(ex, "标记想看失败");
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.FailedToMarkPgc), InfoType.Error));
        }
    }
}
