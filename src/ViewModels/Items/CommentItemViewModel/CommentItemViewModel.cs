// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Provider;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Community;
using CommunityToolkit.Mvvm.Input;
using Humanizer;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 评论条目视图模型.
/// </summary>
public sealed partial class CommentItemViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommentItemViewModel"/> class.
    /// </summary>
    public CommentItemViewModel(
        CommentInformation information,
        Action<CommentItemViewModel> detailAction = null,
        Action<CommentItemViewModel> clickAction = null)
    {
        Data = information;
        _showCommentDetailAction = detailAction;
        _clickAction = clickAction;
        InitializeData();
    }

    private void InitializeData()
    {
        IsLiked = Data.CommunityInformation.IsLiked;
        LikeCountText = NumberToolkit.GetCountText(Data.CommunityInformation.LikeCount);
        PublishDateText = Data.PublishTime.Humanize();
        var replyCount = Data.CommunityInformation.ChildCommentCount;
        if (replyCount > 0)
        {
            ReplyCountText = string.Format(ResourceToolkit.GetLocalizedString(StringNames.MoreReplyDisplay), replyCount);
        }
    }

    [RelayCommand]
    private void ShowDetail()
        => _showCommentDetailAction?.Invoke(this);

    [RelayCommand]
    private void Click()
        => _clickAction?.Invoke(this);

    [RelayCommand]
    private async Task ToggleLikeAsync()
    {
        var isLike = !IsLiked;
        var result = await CommunityProvider.LikeCommentAsync(isLike, Data.Id, Data.CommentId, Data.CommentType);
        if (result)
        {
            IsLiked = isLike;
            if (isLike)
            {
                Data.CommunityInformation.LikeCount += 1;
            }
            else
            {
                Data.CommunityInformation.LikeCount -= 1;
            }

            LikeCountText = NumberToolkit.GetCountText(Data.CommunityInformation.LikeCount);
        }
        else
        {
            AppViewModel.Instance.ShowTip(ResourceToolkit.GetLocalizedString(StringNames.SetFailed), InfoType.Error);
        }
    }
}
