// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Richasy.BiliKernel.Bili.User;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 直播历史记录分区详情视图模型.
/// </summary>
public sealed partial class LiveHistorySectionDetailViewModel : ViewModelBase, IHistorySectionDetailViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LiveHistorySectionDetailViewModel"/> class.
    /// </summary>
    public LiveHistorySectionDetailViewModel(
        IViewHistoryService service)
    {
        _service = service;
        _logger = this.Get<ILogger<LiveHistorySectionDetailViewModel>>();
    }

    [RelayCommand]
    private async Task TryFirstLoadAsync()
    {
        if (Items.Count > 0 || _isPreventLoadMore)
        {
            return;
        }

        await LoadItemsAsync();
    }

    [RelayCommand]
    private Task Refresh()
    {
        IsEmpty = false;
        _isPreventLoadMore = false;
        this.Get<DispatcherQueue>().TryEnqueue(DispatcherQueuePriority.Low, async () =>
        {
            Items.Clear();
            _offset = 0;
            await LoadItemsAsync();
        });

        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task LoadItemsAsync()
    {
        if (IsEmpty || _isPreventLoadMore || IsLoading)
        {
            return;
        }

        try
        {
            IsLoading = true;
            var group = await _service.GetViewHistoryAsync(Type, _offset);
            _offset = group.Offset ?? 0;
            _isPreventLoadMore = group is null || _offset == 0;
            if (group.Lives is not null)
            {
                foreach (var live in group.Lives)
                {
                    Items.Add(new LiveItemViewModel(live, Models.Constants.LiveCardStyle.History, removeAction: RemoveHistory));
                }
            }

            ListUpdated?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _isPreventLoadMore = true;
            _logger.LogError(ex, "加载直播历史记录时出错.");
        }
        finally
        {
            IsEmpty = Items.Count == 0;
            IsLoading = false;
        }
    }

    private void RemoveHistory(LiveItemViewModel vm)
    {
        Items.Remove(vm);
        IsEmpty = Items.Count == 0;
    }
}
