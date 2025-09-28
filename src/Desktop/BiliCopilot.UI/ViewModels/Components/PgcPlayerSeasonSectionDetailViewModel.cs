// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Richasy.BiliKernel.Models.Media;
using Richasy.WinUIKernel.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// PGC 播放器页面剧集视图模型.
/// </summary>
public sealed partial class PgcPlayerSeasonSectionDetailViewModel : ViewModelBase, IPlayerSectionDetailViewModel
{
    private readonly Action<SeasonItemViewModel> _action;

    [ObservableProperty]
    private List<SeasonItemViewModel>? _items;

    /// <summary>
    /// Initializes a new instance of the <see cref="PgcPlayerSeasonSectionDetailViewModel"/> class.
    /// </summary>
    public PgcPlayerSeasonSectionDetailViewModel(
        IList<SeasonInformation> items,
        string seasonId,
        Action<SeasonItemViewModel> action)
    {
        Items = [.. items.Select(p => new SeasonItemViewModel(p, SeasonCardStyle.Player, playAction: OpenSeason))];
        foreach (var item in Items)
        {
            item.IsSelected = item.Data.Identifier.Id == seasonId;
        }

        _action = action;
    }

    /// <inheritdoc/>
    public string Title { get; } = ResourceToolkit.GetLocalizedString(StringNames.Seasons);

    [RelayCommand]
    private static Task TryFirstLoadAsync()
        => Task.CompletedTask;

    private void OpenSeason(SeasonItemViewModel item)
    {
        if (item.IsSelected)
        {
            return;
        }

        _action?.Invoke(item);
    }
}
