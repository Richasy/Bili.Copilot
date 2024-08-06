﻿// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Search;
using Richasy.BiliKernel.Models.Search;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 直播搜索分区详情视图模型.
/// </summary>
public sealed partial class LiveSearchSectionDetailViewModel : ViewModelBase, ISearchSectionDetailViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LiveSearchSectionDetailViewModel"/> class.
    /// </summary>
    public LiveSearchSectionDetailViewModel(
        ISearchService service)
    {
        _service = service;
        _logger = this.Get<ILogger<LiveSearchSectionDetailViewModel>>();
    }

    /// <inheritdoc/>
    public void Initialize(string keyword, SearchPartition partition)
    {
        Clear();
        _partition = partition;
        Count = partition.TotalItemCount;
        IsEmpty = Count == 0;
        _canRequest = !IsEmpty;
        _keyword = keyword;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _partition = default;
        _keyword = string.Empty;
        IsEmpty = false;
        Count = default;
        _canRequest = false;
        Items.Clear();
    }

    [RelayCommand]
    private Task TryFirstLoadAsync()
        => Items.Count > 0 ? Task.CompletedTask : LoadItemsAsync();

    [RelayCommand]
    private async Task LoadItemsAsync()
    {
        if (!_canRequest || IsEmpty || IsLoading || Items.Count >= Count)
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
                Items.Add(new LiveItemViewModel(item.Live));
            }

            ListUpdated?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "加载直播搜索结果时出错.");
        }
        finally
        {
            IsLoading = false;
        }
    }
}
