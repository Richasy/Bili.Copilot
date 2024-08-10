// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Comment;
using Richasy.BiliKernel.Models;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 评论主视图模型.
/// </summary>
public sealed partial class CommentMainViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommentMainViewModel"/> class.
    /// </summary>
    public CommentMainViewModel(
        ICommentService service,
        ILogger<CommentMainViewModel> logger)
    {
        _service = service;
        _logger = logger;

        Sorts = Enum.GetValues<CommentSortType>().ToList();
    }

    /// <summary>
    /// 重置评论源.
    /// </summary>
    public void Initialize(string sourceId, CommentTargetType targetType, CommentSortType sortType)
    {
        Clear();
        _targetId = sourceId;
        _targetType = targetType;
        SortType = sortType;
        Initialized?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        var targetId = _targetId;
        var targetType = _targetType;
        var sortType = SortType;
        Clear();
        _targetId = targetId;
        _targetType = targetType;
        SortType = sortType;
        await LoadItemsAsync();
    }

    [RelayCommand]
    private async Task LoadItemsAsync()
    {
        if (IsLoading || _preventLoadMore || IsEmpty)
        {
            return;
        }

        try
        {
            IsLoading = true;
            var view = await _service.GetCommentsAsync(_targetId, _targetType, SortType, _offset);
            _preventLoadMore = view.IsEnd;
            if (view.TopComment is not null)
            {
                TopItem = new CommentDetailViewModel(new CommentItemViewModel(view.TopComment, _service), ShowMore, _service);
            }

            foreach (var item in view.Comments)
            {
                if (!Comments.Any(p => p.Data.Data.Equals(item)))
                {
                    Comments.Add(new CommentDetailViewModel(new CommentItemViewModel(item, _service), ShowMore, _service));
                }
            }

            _offset = view.NextOffset;
            ListUpdated?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "尝试加载评论时失败");
            _preventLoadMore = true;
        }
        finally
        {
            IsLoading = false;
            IsEmpty = Comments.Count == 0;
        }
    }

    [RelayCommand]
    private async Task ChangeSortTypeAsync(CommentSortType type)
    {
        SortType = type;
        await RefreshAsync();
    }

    [RelayCommand]
    private void BackToMain()
        => SelectedItem = default;

    private void ShowMore(CommentDetailViewModel vm)
        => SelectedItem = vm;

    private void Clear()
    {
        TopItem?.ClearCommand.Execute(default);
        if (Comments is not null)
        {
            foreach (var item in Comments)
            {
                item.ClearCommand.Execute(default);
            }
        }

        Comments.Clear();
        _offset = 0;
        _preventLoadMore = false;
        _targetId = default;
        _targetType = default;
        SortType = default;
        IsEmpty = false;
        TopItem = default;
        SelectedItem = default;
    }
}
