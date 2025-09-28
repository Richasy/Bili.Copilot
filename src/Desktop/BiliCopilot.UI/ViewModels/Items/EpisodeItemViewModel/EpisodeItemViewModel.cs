﻿// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using CommunityToolkit.Mvvm.Input;
using Richasy.BiliKernel.Models.Media;
using Richasy.WinUIKernel.Share.ViewModels;
using Windows.System;
using WinRT;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 单集条目视图模型.
/// </summary>
[GeneratedBindableCustomProperty]
public sealed partial class EpisodeItemViewModel : ViewModelBase<EpisodeInformation>
{
    private readonly Action<EpisodeItemViewModel>? _playAction;

    /// <summary>
    /// Initializes a new instance of the <see cref="EpisodeItemViewModel"/> class.
    /// </summary>
    public EpisodeItemViewModel(EpisodeInformation data, EpisodeCardStyle style, Action<EpisodeItemViewModel>? playAction = default)
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
        _playAction = playAction;
    }

    [RelayCommand]
    private void Play()
    {
        if (_playAction is not null)
        {
            _playAction(this);
            return;
        }

        var snapshot = new MediaSnapshot(default, Data);
        this.Get<AppViewModel>().OpenPlayerCommand.Execute(snapshot);
    }

    [RelayCommand]
    private Task OpenInBroswerAsync()
        => Launcher.LaunchUriAsync(new Uri(GetWebUrl())).AsTask();

    [RelayCommand]
    private void Pin()
    {
        var pinItem = new PinItem($"ep_{Data.Identifier.Id}", Data.Identifier.Title, Data.Identifier.Cover.Uri.ToString(), PinContentType.Pgc);
        this.Get<PinnerViewModel>().AddItemCommand.Execute(pinItem);
    }

    private string GetWebUrl()
        => $"https://www.bilibili.com/bangumi/play/ep{Data.Identifier.Id}";
}
