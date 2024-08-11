// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Models;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 追番分区详情视图模型.
/// </summary>
public sealed partial class PgcFavoriteSectionDetailViewModel : ViewModelBase, IFavoriteSectionDetailViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PgcFavoriteSectionDetailViewModel"/> class.
    /// </summary>
    public PgcFavoriteSectionDetailViewModel(
        PgcFavoriteType type,
        IFavoriteService service)
    {
        _service = service;
        _logger = this.Get<ILogger<PgcFavoriteSectionDetailViewModel>>();
        StatusList = Enum.GetValues<PgcFavoriteStatus>().ToList();
        Type = type;
    }

    [RelayCommand]
    private async Task TryFirstLoadAsync()
    {
        if (Items.Count > 0 || _cache.Count > 0)
        {
            return;
        }

        CurrentStatus = PgcFavoriteStatus.Watching;
        await LoadItemsAsync();
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsEmpty = false;
        Items.Clear();
        _offsetCache.Clear();
        _cache.Clear();
        await LoadItemsAsync();
    }

    [RelayCommand]
    private async Task ChangeStatusAsync(PgcFavoriteStatus status)
    {
        if (CurrentStatus == status)
        {
            return;
        }

        _cache[CurrentStatus] = Items.ToList();
        CurrentStatus = status;
        if (_cache.TryGetValue(status, out var list))
        {
            Items.Clear();
            foreach (var item in list)
            {
                Items.Add(item);
            }

            IsEmpty = Items.Count == 0;
            ListUpdated?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Items.Clear();
            await LoadItemsAsync();
        }
    }

    [RelayCommand]
    private async Task LoadItemsAsync()
    {
        var (pn, isPrevent) = _offsetCache.GetValueOrDefault(CurrentStatus, (0, false));
        if (IsEmpty || IsLoading || isPrevent)
        {
            return;
        }

        try
        {
            IsLoading = true;
            var (seasons, totalCount, nextPage) = await _service.GetPgcFavoritesAsync(Type, CurrentStatus, pn);
            var isPreventLoadMore = seasons is null || nextPage == 0 || (seasons.Count + Items.Count >= totalCount && totalCount > 0);
            _offsetCache[CurrentStatus] = (nextPage ?? 0, isPreventLoadMore);
            if (seasons is not null)
            {
                foreach (var item in seasons)
                {
                    Items.Add(new SeasonItemViewModel(item));
                }
            }

            ListUpdated?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _offsetCache[CurrentStatus] = (pn, true);
            _logger.LogError(ex, $"获取{Type}列表时出错.");
        }
        finally
        {
            IsEmpty = Items.Count == 0;
            IsLoading = false;
        }
    }
}
