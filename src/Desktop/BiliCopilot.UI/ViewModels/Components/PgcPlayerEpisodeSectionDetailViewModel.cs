// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Richasy.BiliKernel.Models.Media;
using Richasy.WinUI.Share.ViewModels;

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
    private EpisodeItemViewModel? _selectedEpisode;

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
        Episodes = episodes.Select(p => new EpisodeItemViewModel(p, EpisodeCardStyle.Player)).ToList();
        SelectedEpisode = Episodes.FirstOrDefault(p => p.Data.Identifier.Id == epid);
        OnlyIndex = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.IsPgcPlayerPartsOnlyIndex, false);
    }

    /// <inheritdoc/>
    public string Title { get; } = ResourceToolkit.GetLocalizedString(StringNames.Episodes);

    [RelayCommand]
    private static Task TryFirstLoadAsync()
        => Task.CompletedTask;

    [RelayCommand]
    private void SelectEpisode(EpisodeItemViewModel episode)
    {
        SelectedEpisode = episode;
        _episodeSelectedAction?.Invoke(episode.Data);
    }

    partial void OnOnlyIndexChanged(bool value)
        => SettingsToolkit.WriteLocalSetting(SettingNames.IsPgcPlayerPartsOnlyIndex, value);
}
