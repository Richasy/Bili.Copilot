// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Comment;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Components;

/// <summary>
/// 评论详情视图模型.
/// </summary>
public sealed partial class CommentDetailViewModel : ViewModelBase<CommentItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommentDetailViewModel"/> class.
    /// </summary>
    public CommentDetailViewModel(CommentItemViewModel data, Action<CommentDetailViewModel> showMoreAction, ICommentService service)
        : base(data)
    {
        _service = service;
        _logger = this.Get<ILogger<CommentDetailViewModel>>();
        _showMoreAction = showMoreAction;
    }

    [RelayCommand]
    private void Initialize()
    {
        if (Comments.Count > 0)
        {
            return;
        }

        LoadItemsCommand.Execute(default);
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
            var targetId = Data.Data.CommentId;
            var targetType = Data.Data.CommentType;
            var view = await _service.GetDetailCommentsAsync(targetId, targetType, Richasy.BiliKernel.Models.CommentSortType.Time, Data.Data.Id, _offset);
            _offset = view.NextOffset;
            foreach (var item in view.Comments)
            {
                if (Comments.Any(p => p.Data.Equals(item)))
                {
                    continue;
                }

                Comments.Add(new CommentItemViewModel(item, _service));
            }

            ListUpdated?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "加载评论详情时出错");
            _preventLoadMore = true;
        }
        finally
        {
            IsEmpty = Comments.Count == 0;
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        Clear();
        await LoadItemsAsync();
    }

    [RelayCommand]
    private void Clear()
    {
        Comments.Clear();
        _offset = 0;
        _preventLoadMore = false;
        IsEmpty = false;
    }

    [RelayCommand]
    private void ShowMore()
    {
        _showMoreAction?.Invoke(this);
        InitializeCommand.Execute(default);
    }
}
