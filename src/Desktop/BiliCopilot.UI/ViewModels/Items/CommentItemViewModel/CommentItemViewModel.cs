// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Pages.Overlay;
using BiliCopilot.UI.ViewModels.Core;
using CommunityToolkit.Mvvm.Input;
using Humanizer;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Comment;
using Richasy.BiliKernel.Models.Comment;
using Richasy.WinUIKernel.Share.ViewModels;
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

    /// <summary>
    /// 跳转到评论中提到的时间点.
    /// </summary>
    /// <param name="seconds">目标秒数.</param>
    /// <returns><see cref="Task"/>.</returns>
    [RelayCommand]
    private async Task SeekToTimestampAsync(double seconds)
    {
        var players = this.Get<AppViewModel>().Players;
        var player = players.LastOrDefault();
        if (player is null || seconds <= 0)
        {
            return;
        }

        await player.ChangePositionAsync(seconds);
    }

    [RelayCommand]
    private void ShowUserSpace()
        => this.Get<NavigationViewModel>().NavigateToOver(typeof(UserSpacePage), Data.User.User);
}
