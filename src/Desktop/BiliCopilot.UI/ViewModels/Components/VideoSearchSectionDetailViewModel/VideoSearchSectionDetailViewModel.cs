// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Richasy.BiliKernel.Bili.Search;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Media;
using Richasy.BiliKernel.Models.Search;
using Richasy.WinUIKernel.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 视频搜索分区详情视图模型.
/// </summary>
public sealed partial class VideoSearchSectionDetailViewModel : ViewModelBase, ISearchSectionDetailViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoSearchSectionDetailViewModel"/> class.
    /// </summary>
    public VideoSearchSectionDetailViewModel(
        ISearchService service)
    {
        _service = service;
        _logger = this.Get<ILogger<VideoSearchSectionDetailViewModel>>();
        Sorts = [.. Enum.GetValues<ComprehensiveSearchSortType>()];
    }

    /// <inheritdoc/>
    public void Initialize(string keyword, SearchPartition partition)
    {
        Clear();
        Sort = ComprehensiveSearchSortType.Default;
        _keyword = keyword;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _keyword = string.Empty;
        IsEmpty = false;
        _isPreventLoadMore = false;
        _canRequest = false;
        this.Get<DispatcherQueue>().TryEnqueue(DispatcherQueuePriority.Low, Items.Clear);
    }

    internal void SetFirstPageData(IReadOnlyList<VideoInformation> videos, int? initialOffset)
    {
        _canRequest = true;
        _currentPage = initialOffset;
        this.Get<DispatcherQueue>().TryEnqueue(DispatcherQueuePriority.Low, () =>
        {
            Items.Clear();
            _isPreventLoadMore = videos is null || videos.Count == 0;

            if (!_isPreventLoadMore)
            {
                foreach (var item in videos)
                {
                    if (Items.Any(p => p.Data.Equals(item)))
                    {
                        continue;
                    }

                    Items.Add(new VideoItemViewModel(item, VideoCardStyle.Search));
                }

                ListUpdated?.Invoke(this, EventArgs.Empty);
            }

            IsEmpty = Items.Count == 0;
        });
    }

    [RelayCommand]
    private static async Task TryFirstLoadAsync() => await Task.CompletedTask;

    [RelayCommand]
    private async Task LoadItemsAsync()
    {
        if (!_canRequest || IsEmpty || _isPreventLoadMore || IsLoading || _currentPage == null)
        {
            return;
        }

        try
        {
            IsLoading = true;
            var (videos, nextOffset) = await _service.GetComprehensiveSearchResultAsync(_keyword, _currentPage, Sort);
            _currentPage = nextOffset;
            _canRequest = _currentPage != null;
            foreach (var item in videos)
            {
                if (Items.Any(p => p.Data.Equals(item)))
                {
                    continue;
                }

                Items.Add(new VideoItemViewModel(item, VideoCardStyle.Search));
            }

            ListUpdated?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _isPreventLoadMore = true;
            _logger.LogError(ex, "加载视频搜索结果时出错.");
        }
        finally
        {
            IsEmpty = Items.Count == 0;
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        this.Get<DispatcherQueue>().TryEnqueue(DispatcherQueuePriority.Low, async () =>
        {
            var keyword = _keyword;
            _currentPage = 1;
            Clear();
            _keyword = keyword;
            _canRequest = true;
            await LoadItemsAsync();
        });

        await Task.CompletedTask;
    }

    [RelayCommand]
    private void ChangeSortType(ComprehensiveSearchSortType type)
    {
        if (Sort == type)
        {
            return;
        }

        Sort = type;
        RefreshCommand.Execute(default);
    }
}
