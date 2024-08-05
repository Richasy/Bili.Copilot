// Copyright (c) Bili Copilot. All rights reserved.

using System.Threading;
using BiliCopilot.UI.ViewModels.Core;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Search;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 搜索视图模型.
/// </summary>
public sealed partial class SearchBoxViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchBoxViewModel"/> class.
    /// </summary>
    public SearchBoxViewModel(
        ISearchService searchService,
        ILogger<SearchBoxViewModel> logger)
    {
        _searchService = searchService;
        _logger = logger;
    }

    [RelayCommand]
    private async Task ReloadSuggestionsAsync()
    {
        TryCancelSuggest();
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
            foreach (var item in _recommendItems)
            {
                Suggestion.Add(new Richasy.BiliKernel.Models.Search.SearchSuggestItem(item.Text, item.Keyword));
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
                    foreach (var item in suggests)
                    {
                        Suggestion.Add(item);
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
            foreach (var item in hotSearches)
            {
                HotSearchItems.Add(item);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试加载热搜榜单时出错.");
        }

        IsHotSearchLoading = false;
    }

    [RelayCommand]
    private void TryCancelSuggest()
    {
        if (_cancellationTokenSource is not null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
    }

    [RelayCommand]
    private void Search(string keyword)
    {
        if (string.IsNullOrEmpty(keyword))
        {
            return;
        }

        this.Get<NavigationViewModel>().Search(keyword);
    }
}
