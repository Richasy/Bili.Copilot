// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Richasy.BiliKernel.Bili.Search;
using Richasy.BiliKernel.Models.Search;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 用户搜索分区详情视图模型.
/// </summary>
public sealed partial class UserSearchSectionDetailViewModel : ViewModelBase, ISearchSectionDetailViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserSearchSectionDetailViewModel"/> class.
    /// </summary>
    public UserSearchSectionDetailViewModel(
        ISearchService service)
    {
        _service = service;
        _logger = this.Get<ILogger<UserSearchSectionDetailViewModel>>();
    }

    /// <inheritdoc/>
    public void Initialize(string keyword, SearchPartition partition)
    {
        Clear();
        _partition = partition;
        IsEmpty = false;
        _canRequest = !IsEmpty;
        _keyword = keyword;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _partition = default;
        _keyword = string.Empty;
        IsEmpty = false;
        _canRequest = false;
        this.Get<DispatcherQueue>().TryEnqueue(DispatcherQueuePriority.Low, Items.Clear);
    }

    [RelayCommand]
    private Task TryFirstLoadAsync()
        => Items.Count > 0 ? Task.CompletedTask : LoadItemsAsync();

    [RelayCommand]
    private async Task LoadItemsAsync()
    {
        if (!_canRequest || IsEmpty || IsLoading)
        {
            return;
        }

        try
        {
            IsLoading = true;
            var (result, nextOffset) = await _service.GetPartitionSearchResultAsync(_keyword, _partition, _offset);
            _offset = nextOffset;
            _canRequest = !string.IsNullOrEmpty(_offset);
            foreach (var item in result)
            {
                if (Items.Any(p => p.Data.Equals(item.User)))
                {
                    continue;
                }

                Items.Add(new UserItemViewModel(item.User));
            }

            ListUpdated?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"加载用户搜索结果时出错.");
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
        var partition = _partition;
        Initialize(keyword, partition);
        await LoadItemsAsync();
    }
}
