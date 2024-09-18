// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Pages.Overlay;
using BiliCopilot.UI.ViewModels.Core;
using CommunityToolkit.Mvvm.Input;
using Humanizer;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Comment;
using Richasy.BiliKernel.Models.Comment;
using Richasy.WinUI.Share.ViewModels;
using WinRT;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 评论项视图模型.
/// </summary>
[GeneratedBindableCustomProperty]
public sealed partial class CommentItemViewModel : ViewModelBase<CommentInformation>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommentItemViewModel"/> class.
    /// </summary>
    public CommentItemViewModel(CommentInformation data, ICommentService service, Action<CommentItemViewModel> markReplyTargetAction)
        : base(data)
    {
        _service = service;
        _logger = this.Get<ILogger<CommentItemViewModel>>();
        Level = data.User.Level ?? 0;
        IsTop = data.IsTop;
        IsLiked = data.CommunityInformation.IsLiked;
        LikeCount = Convert.ToInt32(data.CommunityInformation.LikeCount);
        RelativeTime = data.PublishTime.Humanize();
        ActualTime = data.PublishTime.ToString("yyyy-MM-dd HH:mm:ss");
        ChildCount = data.CommunityInformation.ChildCount;
        Avatar = data.User.User.Avatar.Uri;
        UserName = data.User.User.Name;
        IsVip = data.User.IsVip ?? false;
        Content = data.Content;
        _markReplyTargetAction = markReplyTargetAction;
    }

    [RelayCommand]
    private async Task ToggleLikeAsync()
    {
        var state = !IsLiked;
        try
        {
            await _service.ToggleLikeAsync(state, Data.Id, Data.CommentId, Data.CommentType);
            IsLiked = state;
            LikeCount += state ? 1 : -1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "切换评论点赞状态时失败");
        }
    }

    [RelayCommand]
    private void MarkReplyTarget()
        => _markReplyTargetAction?.Invoke(this);

    [RelayCommand]
    private void ShowUserSpace()
        => this.Get<NavigationViewModel>().NavigateToOver(typeof(UserSpacePage), Data.User.User);
}
