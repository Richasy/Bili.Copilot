// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Data.Community;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 主评论模块视图模型.
/// </summary>
public sealed partial class CommentMainModuleViewModel : InformationFlowViewModel<CommentItemViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommentMainModuleViewModel"/> class.
    /// </summary>
    public CommentMainModuleViewModel()
    {
        SortCollection = new ObservableCollection<CommentSortHeader>
        {
            new CommentSortHeader(CommentSortType.Hot, ResourceToolkit.GetLocalizedString(StringNames.SortByHot)),
            new CommentSortHeader(CommentSortType.Time, ResourceToolkit.GetLocalizedString(StringNames.SortByNewest)),
        };

        CurrentSort = SortCollection.First();

        AttachIsRunningToAsyncCommand(p => IsSending = p, SendCommentCommand);
    }

    /// <summary>
    /// 设置评论区目标.
    /// </summary>
    public void SetTarget(string targetId, CommentType type, CommentSortType defaultSort = CommentSortType.Hot)
    {
        TryClear(Items);
        _targetId = targetId;
        _commentType = type;
        var sort = SortCollection.First(p => p.Type == defaultSort);
        CurrentSort = sort;
        _ = InitializeCommand.ExecuteAsync(null);
    }

    /// <summary>
    /// 清除数据.
    /// </summary>
    public void ClearData()
    {
        TryClear(Items);
        BeforeReload();
        ResetSelectedComment();
    }

    /// <inheritdoc/>
    protected override void BeforeReload()
    {
        _isEnd = false;
        IsEmpty = false;
        TopComment = null;
        CommunityProvider.Instance.ResetMainCommentsStatus();
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

        var data = await CommunityProvider.Instance.GetCommentsAsync(_targetId, _commentType, CurrentSort.Type);
        _isEnd = data.IsEnd;
        if (data.TopComment != null)
        {
            var top = GetItemViewModel(data.TopComment);
            TopComment = top;
        }

        foreach (var item in data.Comments)
        {
            if (!Items.Any(p => p.Data.Equals(item)))
            {
                var vm = GetItemViewModel(item);
                Items.Add(vm);
            }
        }

        IsEmpty = Items.Count == 0 && TopComment == null;
    }

    [RelayCommand]
    private void ChangeSort(CommentSortHeader sort)
    {
        CurrentSort = sort;
        _ = ReloadCommand.ExecuteAsync(null);
    }

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
        var replyCommentId = _selectedComment == null ? "0" : _selectedComment.Data.Id;
        var result = await CommunityProvider.AddCommentAsync(content, _targetId, _commentType, "0", replyCommentId);
        if (result)
        {
            ReplyText = string.Empty;
            ResetSelectedComment();
            if (CurrentSort.Type == CommentSortType.Time)
            {
                // 即便评论发送成功也需要等待一点时间才会显示.
                await Task.Delay(500);
                _ = ReloadCommand.ExecuteAsync(null);
            }
        }
        else
        {
            AppViewModel.Instance.ShowTip(ResourceToolkit.GetLocalizedString(StringNames.AddReplyFailed), InfoType.Error);
        }
    }

    private CommentItemViewModel GetItemViewModel(CommentInformation information)
    {
        var commentVM = new CommentItemViewModel(
            information,
            vm =>
            {
                RequestShowDetail?.Invoke(this, vm);
            },
            vm =>
            {
                _selectedComment = vm;
                ReplyTip = string.Format(ResourceToolkit.GetLocalizedString(StringNames.ReplySomeone), vm.Data.Publisher.User.Name);
            });
        return commentVM;
    }
}
