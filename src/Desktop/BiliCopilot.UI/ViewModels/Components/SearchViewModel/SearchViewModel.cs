﻿// Copyright (c) Bili Copilot. All rights reserved.

using System.Threading;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Search;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 搜索视图模型.
/// </summary>
public sealed partial class SearchViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchViewModel"/> class.
    /// </summary>
    public SearchViewModel(
        ISearchService searchService,
        ILogger<SearchViewModel> logger)
    {
        _searchService = searchService;
        _logger = logger;
    }

    [RelayCommand]
    private async Task ReloadSuggestionsAsync()
    {
        if (_cancellationTokenSource is not null)
        {
            await _cancellationTokenSource.CancelAsync();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }

        if (_recommendItems.Count == 0)
        {
            var recommends = await _searchService.GetSearchRecommendsAsync();
            _recommendItems.AddRange(recommends);
        }

        if (string.IsNullOrEmpty(Keyword.Trim()))
        {
            if (Suggestion.Count == _recommendItems.Count)
            {
                return;
            }

            Suggestion.Clear();
            using var delay = Suggestion.DelayNotifications();
            foreach (var item in _recommendItems)
            {
                delay.Add(new Richasy.BiliKernel.Models.Search.SearchSuggestItem(item.Text, item.Keyword));
            }
        }
        else
        {
            try
            {
                Suggestion.Clear();
                _cancellationTokenSource = new CancellationTokenSource();
                var suggests = await _searchService.GetSearchSuggestsAsync(Keyword, _cancellationTokenSource.Token);
                if (suggests is not null)
                {
                    using var delay = Suggestion.DelayNotifications();
                    foreach (var item in suggests)
                    {
                        delay.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "尝试加载搜索建议时出错.");
            }
        }
    }

    [RelayCommand]
    private async Task LoadHotSearchAsync()
    {
        if (HotSearchItems.Count > 0)
        {
            return;
        }

        IsHotSearchLoading = true;
        try
        {
            var hotSearches = await _searchService.GetTotalHotSearchAsync(30);
            using var delay = HotSearchItems.DelayNotifications();
            foreach (var item in hotSearches)
            {
                delay.Add(item);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试加载热搜榜单时出错.");
        }

        IsHotSearchLoading = false;
    }
}
