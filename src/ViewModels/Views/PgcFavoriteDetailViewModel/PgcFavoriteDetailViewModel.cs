// Copyright (c) Bili Copilot. All rights reserved.

using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 追番/追剧详情视图模型.
/// </summary>
public partial class PgcFavoriteDetailViewModel : InformationFlowViewModel<SeasonItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PgcFavoriteDetailViewModel"/> class.
    /// </summary>
    /// <param name="type">收藏类型.</param>
    internal PgcFavoriteDetailViewModel(FavoriteType type)
    {
        _type = type;
        Status = 1;
    }

    /// <inheritdoc/>
    protected override void BeforeReload()
    {
        _isEnd = false;
        IsEmpty = false;
        if (_type == FavoriteType.Anime)
        {
            FavoriteProvider.Instance.ResetAnimeStatus();
        }
        else if (_type == FavoriteType.Film)
        {
            FavoriteProvider.Instance.ResetCinemaStatus();
        }
    }

    /// <inheritdoc/>
    protected override string FormatException(string errorMsg)
    {
        var prefix = _type == FavoriteType.Anime
            ? StringNames.RequestAnimeFavoriteFailed
            : StringNames.RequestCinemaFavoriteFailed;
        return $"{ResourceToolkit.GetLocalizedString(prefix)}\n{errorMsg}";
    }

    /// <inheritdoc/>
    protected override async Task GetDataAsync()
    {
        if (_isEnd)
        {
            return;
        }

        var data = _type == FavoriteType.Anime
            ? await FavoriteProvider.Instance.GetFavoriteAnimeListAsync(Status + 1)
            : await FavoriteProvider.Instance.GetFavoriteCinemaListAsync(Status + 1);

        foreach (var item in data.Items)
        {
            var seasonVM = new SeasonItemViewModel(item, RemoveItem);
            Items.Add(seasonVM);
        }

        IsEmpty = Items.Count == 0;
        _isEnd = Items.Count >= data.TotalCount;
    }

    [RelayCommand]
    private void SetStatus(int status)
    {
        Status = status;
        ReloadCommand.Execute(default);
    }

    private void RemoveItem(SeasonItemViewModel vm)
    {
        Items.Remove(vm);
        IsEmpty = Items.Count == 0;
    }
}
