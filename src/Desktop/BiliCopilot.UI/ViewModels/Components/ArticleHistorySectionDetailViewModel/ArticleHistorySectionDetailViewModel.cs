// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.User;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 文章历史记录详情视图模型.
/// </summary>
public sealed partial class ArticleHistorySectionDetailViewModel : ViewModelBase, IHistorySectionDetailViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArticleHistorySectionDetailViewModel"/> class.
    /// </summary>
    public ArticleHistorySectionDetailViewModel(
        IViewHistoryService service)
    {
        _service = service;
        _logger = this.Get<ILogger<ArticleHistorySectionDetailViewModel>>();
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
    private async Task RefreshAsync()
    {
        IsEmpty = false;
        _isPreventLoadMore = false;
        Items = new System.Collections.ObjectModel.ObservableCollection<ArticleItemViewModel>();
        _offset = 0;
        await LoadItemsAsync();
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
            if (group.Articles is not null)
            {
                foreach (var article in group.Articles)
                {
                    Items.Add(new ArticleItemViewModel(article, removeAction: RemoveArticle));
                }
            }

            ListUpdated?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _isPreventLoadMore = true;
            _logger.LogError(ex, "加载文章历史记录时出错.");
        }
        finally
        {
            IsEmpty = Items.Count == 0;
            IsLoading = false;
        }
    }

    private void RemoveArticle(ArticleItemViewModel item)
    {
        Items.Remove(item);
        IsEmpty = Items.Count == 0;
        ListUpdated?.Invoke(this, EventArgs.Empty);
    }
}
