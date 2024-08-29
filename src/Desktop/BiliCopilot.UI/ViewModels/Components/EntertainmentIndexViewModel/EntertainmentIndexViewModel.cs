// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Media;
using Richasy.BiliKernel.Models;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 娱乐索引视图模型.
/// </summary>
public sealed partial class EntertainmentIndexViewModel : ViewModelBase, IPgcSectionDetailViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EntertainmentIndexViewModel"/> class.
    /// </summary>
    public EntertainmentIndexViewModel(
        PgcSectionType type,
        IEntertainmentDiscoveryService service)
    {
        SectionType = type;
        _logger = this.Get<ILogger<EntertainmentIndexViewModel>>();
        _service = service;
    }

    private static EntertainmentType GetEntertainmentType(PgcSectionType type)
    {
        return type switch
        {
            PgcSectionType.Anime => EntertainmentType.Anime,
            PgcSectionType.Documentary => EntertainmentType.Documentary,
            PgcSectionType.Movie => EntertainmentType.Movie,
            PgcSectionType.TV => EntertainmentType.TV,
            _ => throw new NotSupportedException(),
        };
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (Filters?.Count > 0)
        {
            return;
        }

        IsFilterLoading = true;
        try
        {
            var filters = await _service.GetIndexFiltersAsync(GetEntertainmentType(SectionType));
            Filters = filters.Select(p => new IndexFilterViewModel(p, RequestForConditionChanged)).ToList();
            await RequestIndexAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试请求索引筛选器时失败");
        }
        finally
        {
            IsFilterLoading = false;
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        _pageNumer = 0;
        Items.Clear();
        Filters = default;
        await InitializeAsync();
    }

    [RelayCommand]
    private async Task RequestIndexAsync()
    {
        if (Filters is null || Filters.Count == 0 || IsSeasonLoading)
        {
            return;
        }

        IsSeasonLoading = true;
        try
        {
            var conditions = Filters.Select(p => (p.Data, p.CurrentCondition!)).ToDictionary();
            var (seasons, hasNext) = await _service.GetIndexSeasonsWithFiltersAsync(GetEntertainmentType(SectionType), conditions, _pageNumer);
            if (seasons is not null)
            {
                foreach (var item in seasons)
                {
                    Items.Add(new SeasonItemViewModel(item));
                }

                if (hasNext)
                {
                    _pageNumer++;
                }

                ItemsUpdated?.Invoke(this, EventArgs.Empty);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试加载索引时失败");
        }
        finally
        {
            IsSeasonLoading = false;
        }
    }

    private void RequestForConditionChanged()
    {
        _pageNumer = 0;
        Items.Clear();
        RequestIndexCommand.Execute(default);
    }
}
