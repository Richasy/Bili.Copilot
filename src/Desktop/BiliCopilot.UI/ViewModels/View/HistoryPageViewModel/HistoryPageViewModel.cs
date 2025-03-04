// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Core;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Richasy.BiliKernel.Bili.Search;
using Richasy.BiliKernel.Bili.User;
using Richasy.BiliKernel.Models;
using Richasy.WinUIKernel.Share.Base;
using Richasy.WinUIKernel.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 历史记录页面视图模型.
/// </summary>
public sealed partial class HistoryPageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HistoryPageViewModel"/> class.
    /// </summary>
    public HistoryPageViewModel(
        IViewHistoryService service,
        ISearchService searchService,
        ILogger<HistoryPageViewModel> logger)
    {
        _service = service;
        _searchService = searchService;
        _logger = logger;
    }

    [RelayCommand]
    private void Initialize()
    {
        if (IsSearchMode)
        {
            ExitSearch();
        }

        if (Sections is not null)
        {
            return;
        }

        var types = Enum.GetValues<ViewHistoryTabType>().Where(p => p != ViewHistoryTabType.All).ToList();
        var sections = new List<IHistorySectionDetailViewModel>();
        foreach (var type in types)
        {
            var vm = type switch
            {
                ViewHistoryTabType.Video => (IHistorySectionDetailViewModel)new VideoHistorySectionDetailViewModel(_service),
                ViewHistoryTabType.Article => new ArticleHistorySectionDetailViewModel(_service),
                ViewHistoryTabType.Live => new LiveHistorySectionDetailViewModel(_service),
                _ => null,
            };

            if (vm is not null)
            {
                sections.Add(vm);
            }
        }

        Sections = sections;
        SelectSection(Sections.First());
        SectionInitialized?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void Search(string text)
    {
        ExitSearch();
        SearchKeyword = text;
        IsSearchMode = true;
        LoadMoreSearchResultCommand.Execute(default);
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        if (IsSearchMode)
        {
            var keyword = SearchKeyword;
            SearchCommand.Execute(keyword);
            return;
        }

        await SelectedSection?.RefreshCommand.ExecuteAsync(this);
    }

    [RelayCommand]
    private async Task CleanAsync()
    {
        if (SelectedSection is null)
        {
            return;
        }

        await _service.CleanHistoryAsync(SelectedSection.Type);
        await Task.Delay(500);
        await SelectedSection.RefreshCommand.ExecuteAsync(this);
    }

    [RelayCommand]
    private void SelectSection(IHistorySectionDetailViewModel section)
    {
        if (section is null || section == SelectedSection)
        {
            return;
        }

        SelectedSection = section;
        section.TryFirstLoadCommand.Execute(this);
    }

    [RelayCommand]
    private async Task LoadMoreSearchResultAsync()
    {
        if (_preventLoadMoreSearch || IsSearching || !IsSearchMode || string.IsNullOrEmpty(SearchKeyword))
        {
            return;
        }

        try
        {
            IsSearching = true;
            CancelSearch();
            _searchCancellationTokenSource = new CancellationTokenSource();
            var (videos, total, hasMore) = await _searchService.SearchHistoryVideosAsync(SearchKeyword, _searchPn, _searchCancellationTokenSource.Token);
            _preventLoadMoreSearch = !hasMore || videos is null || videos.Count == 0;
            if (videos is not null)
            {
                foreach (var item in videos)
                {
                    SearchVideos.Add(new VideoItemViewModel(item, VideoCardStyle.History, RemoveSearchVideo));
                }

                SearchUpdated?.Invoke(this, EventArgs.Empty);
            }

            if (!_preventLoadMoreSearch)
            {
                _searchPn++;
            }
        }
        catch (TaskCanceledException)
        {
            // Do nothing.
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "搜索用户视频时失败");
            this.Get<AppViewModel>().ShowTipCommand.Execute((ResourceToolkit.GetLocalizedString(StringNames.FailedToSearchUserVideos), InfoType.Error));
        }
        finally
        {
            IsSearchEmpty = SearchVideos.Count == 0;
            IsSearching = false;
        }
    }

    [RelayCommand]
    private void ExitSearch()
    {
        IsSearchMode = false;
        SearchKeyword = string.Empty;
        IsSearchEmpty = false;
        this.Get<DispatcherQueue>().TryEnqueue(DispatcherQueuePriority.Low, SearchVideos.Clear);
        _preventLoadMoreSearch = false;
        _searchPn = 0;
        CancelSearch();
    }

    private void CancelSearch()
    {
        if (_searchCancellationTokenSource is not null)
        {
            _searchCancellationTokenSource.Cancel();
            _searchCancellationTokenSource.Dispose();
            _searchCancellationTokenSource = null;
        }
    }

    private void RemoveSearchVideo(VideoItemViewModel item)
    {
        SearchVideos.Remove(item);
        if (SearchVideos.Count == 0)
        {
            IsSearchEmpty = true;
        }

        SearchUpdated?.Invoke(this, EventArgs.Empty);
    }
}
