// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Pages.Overlay;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Search;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 搜索页面视图模型.
/// </summary>
public sealed partial class SearchPageViewModel : LayoutPageViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchPageViewModel"/> class.
    /// </summary>
    public SearchPageViewModel(
        ISearchService service,
        ILogger<SearchPageViewModel> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <inheritdoc/>
    protected override string GetPageKey()
        => nameof(SearchPage);

    [RelayCommand]
    private async Task SearchAsync(string keyword)
    {
        if (string.IsNullOrEmpty(keyword))
        {
            throw new ArgumentNullException(nameof(keyword));
        }

        Keyword = keyword;
        try
        {
            var (videos, partitions, nextVideoOffset) = await _service.GetComprehensiveSearchResultAsync(keyword);
            if (partitions is not null)
            {
            }

            if (videos is not null)
            {
                _initialVideos = [.. videos];
            }

            _initialOffset = nextVideoOffset;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试获取搜索结果时失败.");
        }
    }
}
