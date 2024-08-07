// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Search;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Media;
using Richasy.BiliKernel.Models.Search;
using Richasy.WinUI.Share.ViewModels;

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
        Sorts = Enum.GetValues<ComprehensiveSearchSortType>().ToList();
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
        Count = default;
        _canRequest = false;
        Items.Clear();
    }

    internal void SetFirstPageData(IReadOnlyList<VideoInformation> videos, string initialOffset)
    {
        _canRequest = true;
        _offset = initialOffset;
        Items.Clear();
        _isPreventLoadMore = videos is null || videos.Count == 0;

        if (!_isPreventLoadMore)
        {
            foreach (var item in videos)
            {
                Items.Add(new VideoItemViewModel(item, VideoCardStyle.Search));
            }

            ListUpdated?.Invoke(this, EventArgs.Empty);
        }

        IsEmpty = Items.Count == 0;
    }

    [RelayCommand]
    private static async Task TryFirstLoadAsync() => await Task.CompletedTask;

    [RelayCommand]
    private async Task LoadItemsAsync()
    {
        if (!_canRequest || IsEmpty || _isPreventLoadMore || IsLoading)
        {
            return;
        }

        try
        {
            IsLoading = true;
            var (videos, _, nextOffset) = await _service.GetComprehensiveSearchResultAsync(_keyword, _offset, Sort);
            _offset = nextOffset;
            _canRequest = !string.IsNullOrEmpty(_offset);
            foreach (var item in videos)
            {
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
        var keyword = _keyword;
        Clear();
        _keyword = keyword;
        _canRequest = true;
        await LoadItemsAsync();
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
