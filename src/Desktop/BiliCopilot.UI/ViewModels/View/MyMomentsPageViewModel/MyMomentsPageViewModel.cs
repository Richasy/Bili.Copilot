// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Moment;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 我的动态页面视图模型.
/// </summary>
public sealed partial class MyMomentsPageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MyMomentsPageViewModel"/> class.
    /// </summary>
    public MyMomentsPageViewModel(
        IMomentDiscoveryService service,
        ILogger<MyMomentsPageViewModel> logger,
        CommentMainViewModel comment)
    {
        _service = service;
        _logger = logger;
        CommentModule = comment;
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        if (Items.Count > 0 || _preventLoadMore)
        {
            return;
        }

        await LoadItemsAsync();
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        _offset = default;
        IsEmpty = false;
        _preventLoadMore = false;
        Items.Clear();
        await LoadItemsAsync();
    }

    [RelayCommand]
    private async Task LoadItemsAsync()
    {
        if (IsEmpty || IsLoading || _preventLoadMore)
        {
            return;
        }

        try
        {
            IsLoading = true;
            var (moments, offset, hasMore) = await _service.GetMyMomentsAsync(_offset);
            _offset = offset;
            _preventLoadMore = !hasMore || string.IsNullOrEmpty(offset);
            if (moments is not null)
            {
                foreach (var item in moments.Select(p => new MomentItemViewModel(p, ShowComment)))
                {
                    Items.Add(item);
                }

                ListUpdated?.Invoke(this, EventArgs.Empty);
            }
        }
        catch (Exception ex)
        {
            _preventLoadMore = true;
            _logger.LogError(ex, "加载动态列表失败.");
        }
        finally
        {
            IsEmpty = Items.Count == 0;
            IsLoading = false;
        }
    }

    private void ShowComment(MomentItemViewModel data)
    {
        var moment = data.Data;
        if (CommentModule.Id == moment.CommentId)
        {
            return;
        }

        IsCommentsOpened = true;
        CommentModule.Initialize(moment.CommentId, moment.CommentType!.Value, Richasy.BiliKernel.Models.CommentSortType.Hot);
        CommentModule.RefreshCommand.Execute(default);
    }
}
