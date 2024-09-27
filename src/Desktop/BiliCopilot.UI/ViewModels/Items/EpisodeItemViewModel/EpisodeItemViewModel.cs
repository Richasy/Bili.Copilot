// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Forms;
using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Pages.Overlay;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using CommunityToolkit.Mvvm.Input;
using Richasy.BiliKernel.Models.Media;
using Richasy.WinUI.Share.ViewModels;
using Windows.System;
using WinRT;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 单集条目视图模型.
/// </summary>
[GeneratedBindableCustomProperty]
public sealed partial class EpisodeItemViewModel : ViewModelBase<EpisodeInformation>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EpisodeItemViewModel"/> class.
    /// </summary>
    public EpisodeItemViewModel(EpisodeInformation data, EpisodeCardStyle style)
        : base(data)
    {
        Title = data.Identifier.Title;
        Cover = data.Identifier.Cover?.Uri;
        PlayCount = data.CommunityInformation?.PlayCount;
        DanmakuCount = data.CommunityInformation?.DanmakuCount;
        CommentCount = data.CommunityInformation?.CommentCount;
        CoinCount = data.CommunityInformation?.CoinCount;
        LikeCount = data.CommunityInformation?.LikeCount;
        Style = style;
        Duration = AppToolkit.FormatDuration(TimeSpan.FromSeconds(data.Duration ?? 0));
        IsPreview = data.GetExtensionIfNotNull<bool>(EpisodeExtensionDataId.IsPreview);
        HighlightText = data.GetExtensionIfNotNull<string>(EpisodeExtensionDataId.RecommendReason);
        Index = data.Index;
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

        var preferPlayer = SettingsToolkit.ReadLocalSetting(SettingNames.PlayerType, PlayerType.Island);
        if (preferPlayer == PlayerType.Web)
        {
            this.Get<NavigationViewModel>().NavigateToOver(typeof(WebPlayerPage), GetWebUrl());
            return;
        }

        var id = new MediaIdentifier("ep_" + Data.Identifier.Id, default, default);
        this.Get<NavigationViewModel>().NavigateToOver(typeof(PgcPlayerPage), id);
    }

    [RelayCommand]
    private Task OpenInBroswerAsync()
        => Launcher.LaunchUriAsync(new Uri(GetWebUrl())).AsTask();

    [RelayCommand]
    private void OpenInNewWindow()
        => new PlayerWindow().OpenPgc(new MediaIdentifier("ep_" + Data.Identifier.Id, default, default));

    [RelayCommand]
    private void Pin()
    {
        var pinItem = new PinItem($"ep_{Data.Identifier.Id}", Data.Identifier.Title, Data.Identifier.Cover.Uri.ToString(), PinContentType.Pgc);
        this.Get<PinnerViewModel>().AddItemCommand.Execute(pinItem);
    }

    private string GetWebUrl()
        => $"https://www.bilibili.com/bangumi/play/ep{Data.Identifier.Id}";
}
