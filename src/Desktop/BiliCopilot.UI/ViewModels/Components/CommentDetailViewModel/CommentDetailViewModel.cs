// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.ViewModels.Items;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Richasy.BiliKernel.Bili.Comment;
using Richasy.WinUIKernel.Share.ViewModels;

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
            IsLoading = true;
            var targetId = Data.Data.CommentId;
            var targetType = Data.Data.CommentType;
            var view = await _service.GetDetailCommentsAsync(targetId, targetType, Richasy.BiliKernel.Models.CommentSortType.Time, Data.Data.Id, _offset);
            _offset = view.NextOffset;
            _preventLoadMore = view.NextOffset == 0 || view.Comments is null;
            foreach (var item in view.Comments)
            {
                if (Comments.Any(p => p.Data.Equals(item)))
                {
                    continue;
                }

                Comments.Add(new CommentItemViewModel(item, _service, SetReplyTarget));
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
    private void Refresh()
    {
        Clear();
        this.Get<DispatcherQueue>().TryEnqueue(DispatcherQueuePriority.Low, async () => await LoadItemsAsync());
    }

    [RelayCommand]
    private void Clear()
    {
        Comments.Clear();
        _offset = 0;
        _preventLoadMore = false;
        IsEmpty = false;
        ResetReplyTarget();
    }

    [RelayCommand]
    private void ShowMore()
    {
        _showMoreAction?.Invoke(this);
        InitializeCommand.Execute(default);
    }

    [RelayCommand]
    private async Task SendReplyAsync(string content)
    {
        if (IsReplying || string.IsNullOrEmpty(content))
        {
            return;
        }

        IsReplying = true;
        content = content.Trim();
        var targetId = Data.Data.CommentId;
        var rootId = _replyItem is null ? Data.Data.Id : _replyItem.Data.RootId;
        var replyCommentId = _replyItem is null ? rootId : _replyItem.Data.Id;
        try
        {
            await _service.SendTextCommentAsync(content, targetId, Data.Data.CommentType, rootId, replyCommentId);
            await Task.Delay(500);
            Refresh();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "回复评论时出错");
        }
        finally
        {
            IsReplying = false;
        }
    }

    [RelayCommand]
    private void ResetReplyTarget()
    {
        _replyItem = default;
        ReplyTarget = default;
    }

    private void SetReplyTarget(CommentItemViewModel item)
    {
        _replyItem = item;
        ReplyTarget = item?.Data.User.User.Name;
    }
}
