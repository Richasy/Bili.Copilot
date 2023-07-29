// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Data.Community;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 二级评论模块视图详情.
/// </summary>
public sealed partial class CommentDetailModuleViewModel : InformationFlowViewModel<CommentItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommentDetailModuleViewModel"/> class.
    /// </summary>
    public CommentDetailModuleViewModel() => AttachIsRunningToAsyncCommand(p => IsSending = p, SendCommentCommand);

    /// <summary>
    /// 设置根评论.
    /// </summary>
    public void SetRoot(CommentItemViewModel rootItem)
    {
        TryClear(Items);
        RootComment = GetItemViewModel(rootItem.Data);
        _ = InitializeCommand.ExecuteAsync(null);
    }

    /// <summary>
    /// 清除数据.
    /// </summary>
    public void ClearData()
    {
        RootComment = null;
        TryClear(Items);
        BeforeReload();
        ResetSelectedComment();
    }

    /// <inheritdoc/>
    protected override void BeforeReload()
    {
        IsEmpty = false;
        _isEnd = false;
    }

    /// <inheritdoc/>
    protected override string FormatException(string errorMsg)
        => $"{ResourceToolkit.GetLocalizedString(StringNames.RequestReplyFailed)}\n{errorMsg}";

    /// <inheritdoc/>
    protected override async Task GetDataAsync()
    {
        if (_isEnd)
        {
            return;
        }

        var targetId = RootComment.Data.CommentId;
        var commentType = RootComment.Data.CommentType;
        var data = await CommunityProvider.Instance.GetCommentsAsync(targetId, commentType, CommentSortType.Time, RootComment.Data.Id);
        _isEnd = data.IsEnd;
        foreach (var item in data.Comments)
        {
            if (!Items.Any(p => p.Data.Equals(item)))
            {
                Items.Add(GetItemViewModel(item));
            }
        }

        IsEmpty = Items.Count == 0;
    }

    [RelayCommand]
    private void Back()
        => RequestBackToMain?.Invoke(this, EventArgs.Empty);

    [RelayCommand]
    private void ResetSelectedComment()
    {
        _selectedComment = null;
        ReplyTip = string.Empty;
    }

    [RelayCommand]
    private async Task SendCommentAsync()
    {
        if (IsSending || string.IsNullOrEmpty(ReplyText))
        {
            return;
        }

        var content = ReplyText;
        var targetId = RootComment.Data.CommentId;
        var rootId = _selectedComment == null ? RootComment.Data.Id : _selectedComment.Data.RootId;
        var replyCommentId = _selectedComment == null ? rootId : _selectedComment.Data.Id;

        var commentType = RootComment.Data.CommentType;
        var result = await CommunityProvider.AddCommentAsync(content, targetId, commentType, rootId, replyCommentId);
        if (result)
        {
            ReplyText = string.Empty;
            ResetSelectedComment();

            // 即便评论发送成功也需要等待一点时间才会显示.
            await Task.Delay(500);
            _ = ReloadCommand.ExecuteAsync(null);
        }
        else
        {
            AppViewModel.Instance.ShowTip(ResourceToolkit.GetLocalizedString(StringNames.AddReplyFailed), InfoType.Error);
        }
    }

    private CommentItemViewModel GetItemViewModel(CommentInformation information)
    {
        var highlightUserId = RootComment == null
            ? information.Publisher.User.Id
            : RootComment.Data.Publisher.User.Id;
        var vm = new CommentItemViewModel(
            information,
            null,
            vm =>
            {
                if (vm != RootComment)
                {
                    _selectedComment = vm;
                    ReplyTip = string.Format(ResourceToolkit.GetLocalizedString(StringNames.ReplySomeone), vm.Data.Publisher.User.Name);
                }
                else
                {
                    ResetSelectedComment();
                }
            })
        {
            IsUserHighlight = !string.IsNullOrEmpty(highlightUserId) && information.Publisher.User.Id == highlightUserId,
            ReplyCountText = string.Empty,
        };
        return vm;
    }
}
