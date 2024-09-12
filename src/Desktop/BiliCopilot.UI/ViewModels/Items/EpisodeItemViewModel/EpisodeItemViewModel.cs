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

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 单集条目视图模型.
/// </summary>
public sealed partial class EpisodeItemViewModel : ViewModelBase<EpisodeInformation>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EpisodeItemViewModel"/> class.
    /// </summary>
    public EpisodeItemViewModel(EpisodeInformation data)
        : base(data)
    {
        Title = data.Identifier.Title;
        Cover = data.Identifier.Cover?.Uri;
        PlayCount = data.CommunityInformation?.PlayCount;
        DanmakuCount = data.CommunityInformation?.DanmakuCount;
        CommentCount = data.CommunityInformation?.CommentCount;
        CoinCount = data.CommunityInformation?.CoinCount;
        LikeCount = data.CommunityInformation?.LikeCount;
        Duration = AppToolkit.FormatDuration(TimeSpan.FromSeconds(data.Duration ?? 0));
        IsPreview = data.GetExtensionIfNotNull<bool>(EpisodeExtensionDataId.IsPreview);
        HighlightText = data.GetExtensionIfNotNull<string>(EpisodeExtensionDataId.RecommendReason);
        Index = data.Index;
    }

    [RelayCommand]
    private void Play()
    {
        var id = new MediaIdentifier("ep_" + Data.Identifier.Id, default, default);
        this.Get<NavigationViewModel>().NavigateToOver(typeof(PgcPlayerPage).FullName, id);
    }

    [RelayCommand]
    private async Task OpenInBroswerAsync()
    {
        var url = $"https://www.bilibili.com/bangumi/play/ep{Data.Identifier.Id}";
        await Launcher.LaunchUriAsync(new Uri(url)).AsTask();
    }

    [RelayCommand]
    private void OpenInNewWindow()
        => new PlayerWindow().OpenPgc(new MediaIdentifier("ep_" + Data.Identifier.Id, default, default));

    [RelayCommand]
    private void Pin()
    {
        var pinItem = new PinItem($"ep_{Data.Identifier.Id}", Data.Identifier.Title, Data.Identifier.Cover.Uri.ToString(), PinContentType.Pgc);
        this.Get<PinnerViewModel>().AddItemCommand.Execute(pinItem);
    }
}
