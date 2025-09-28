﻿// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Richasy.BiliKernel.Models.Media;
using Richasy.WinUIKernel.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// PGC 播放器分集详情视图模型.
/// </summary>
public sealed partial class PgcPlayerEpisodeSectionDetailViewModel : ViewModelBase, IPlayerSectionDetailViewModel
{
    private readonly Action<EpisodeInformation> _episodeSelectedAction;

    [ObservableProperty]
    private List<EpisodeItemViewModel>? _episodes;

    [ObservableProperty]
    private bool _onlyIndex;

    /// <summary>
    /// Initializes a new instance of the <see cref="PgcPlayerEpisodeSectionDetailViewModel"/> class.
    /// </summary>
    public PgcPlayerEpisodeSectionDetailViewModel(
        IList<EpisodeInformation> episodes,
        string epid,
        Action<EpisodeInformation> action)
    {
        _episodeSelectedAction = action;
        Episodes = [.. episodes.Select(p => new EpisodeItemViewModel(p, EpisodeCardStyle.Player, PlayEpisode))];
        foreach (var item in Episodes)
        {
            item.IsSelected = item.Data.Identifier.Id == epid;
        }

        OnlyIndex = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.IsPgcPlayerPartsOnlyIndex, false);
    }

    /// <inheritdoc/>
    public string Title { get; } = ResourceToolkit.GetLocalizedString(StringNames.Episodes);

    [RelayCommand]
    private static Task TryFirstLoadAsync()
        => Task.CompletedTask;

    private void PlayEpisode(EpisodeItemViewModel episode)
    {
        foreach (var item in Episodes ?? [])
        {
            item.IsSelected = item.Data.Identifier.Id == episode.Data.Identifier.Id;
        }

        _episodeSelectedAction?.Invoke(episode.Data);
    }

    partial void OnOnlyIndexChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.IsPgcPlayerPartsOnlyIndex, value);
}
