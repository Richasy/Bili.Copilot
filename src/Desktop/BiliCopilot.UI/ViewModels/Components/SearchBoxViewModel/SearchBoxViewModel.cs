// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.Items;
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

    /// <summary>
    /// 设置附加搜索区域（用于在特殊页面进行局部搜索）.
    /// </summary>
    public void SetExtraRegion(string regionId, string regionName)
    {
        _extraRegionId = regionId;
        _extraRegionName = regionName;
    }

    [RelayCommand]
    private async Task ReloadSuggestionsAsync()
    {
        TryCancelSuggest();

        if (string.IsNullOrEmpty(Keyword.Trim()))
        {
            if (string.IsNullOrEmpty(_extraRegionId))
            {
                var isRecommendEnabled = SettingsToolkit.ReadLocalSetting(Models.Constants.SettingNames.ShowSearchRecommend, true);
                if (!isRecommendEnabled)
                {
                    _recommendItems.Clear();
                    Suggestion.Clear();
                    return;
                }

                if (_recommendItems.Count == 0)
                {
                    var recommends = await _searchService.GetSearchRecommendsAsync();
                    _recommendItems.AddRange(recommends);
                }

                if (Suggestion.Count == _recommendItems.Count)
                {
                    return;
                }

                Suggestion.Clear();
                foreach (var item in _recommendItems)
                {
                    Suggestion.Add(new(new Richasy.BiliKernel.Models.Search.SearchSuggestItem(item.Text, item.Keyword)));
                }
            }
            else
            {
                Suggestion.Clear();
                Suggestion.Add(new("global", ResourceToolkit.GetLocalizedString(Models.Constants.StringNames.WholePartitions)));
                Suggestion.Add(new(_extraRegionId, _extraRegionName));
            }
        }
        else
        {
            if (string.IsNullOrEmpty(_extraRegionId))
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
                            Suggestion.Add(new(item));
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "尝试加载搜索建议时出错.");
                }
            }
            else
            {
                foreach (var item in Suggestion)
                {
                    item.SearchContent = Keyword;
                    item.Keyword = Keyword;
                }
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
    private void Search(SearchSuggestItemViewModel vm)
    {
        var keyword = vm.Keyword;
        if (string.IsNullOrEmpty(keyword))
        {
            return;
        }

        if (string.IsNullOrEmpty(vm.RegionId) || vm.RegionId == "global")
        {
            this.Get<NavigationViewModel>().Search(keyword);
        }
        else
        {
            this.Get<NavigationViewModel>().SearchInRegion(keyword);
        }
    }
}
