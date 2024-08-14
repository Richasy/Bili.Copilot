// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Pages.Overlay;
using BiliCopilot.UI.Toolkits;
using BiliCopilot.UI.ViewModels.Core;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Moment;
using Richasy.BiliKernel.Models.Appearance;
using Richasy.BiliKernel.Models.Media;
using Richasy.BiliKernel.Models.Moment;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 动态条目视图模型.
/// </summary>
public sealed partial class MomentItemViewModel : ViewModelBase<MomentInformation>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MomentItemViewModel"/> class.
    /// </summary>
    public MomentItemViewModel(MomentInformation data, Action<MomentItemViewModel> showCommentAction)
        : base(data)
    {
        _showMomentAction = showCommentAction;
        _operationService = this.Get<IMomentOperationService>();
        _logger = this.Get<ILogger<MomentItemViewModel>>();
        IsLiked = data.CommunityInformation?.IsLiked ?? false;
        LikeCount = data.CommunityInformation?.LikeCount;
        CommentCount = data.CommunityInformation?.CommentCount;
        Author = data.User?.Name;
        Avatar = data.User?.Avatar.Uri;
        Tip = data.Tip;
        Description = data.Description;
        NoData = data.Data is null;

        if (!NoData)
        {
            if (data.Data is MomentInformation forward)
            {
                InnerContent = new MomentItemViewModel(forward, showCommentAction);
            }
            else if (data.Data is VideoInformation video)
            {
                InnerContent = new VideoItemViewModel(video, VideoCardStyle.Moment);
            }
            else if (data.Data is EpisodeInformation episode)
            {
                InnerContent = new EpisodeItemViewModel(episode);
            }
            else if (data.Data is IEnumerable<BiliImage> images)
            {
                InnerContent = images;
            }

            if (FindInnerContent<VideoInformation>() is VideoInformation vinfo)
            {
                VideoTitle = vinfo.Identifier.Title ?? ResourceToolkit.GetLocalizedString(StringNames.NoTitleVideo);
                VideoCover = vinfo.Identifier.Cover.Uri;
            }
            else if (FindInnerContent<EpisodeInformation>() is EpisodeInformation einfo)
            {
                VideoTitle = einfo.Identifier.Title;
                VideoCover = einfo.Identifier.Cover.Uri;
            }
        }
    }

    [RelayCommand]
    private async Task ToggleLikeAsync()
    {
        var state = !IsLiked;
        try
        {
            if (state)
            {
                await _operationService.LikeMomentAsync(Data);
            }
            else
            {
                await _operationService.DislikeMomentAsync(Data);
            }

            IsLiked = state;
            LikeCount += state ? 1 : -1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "切换评论点赞状态时失败");
        }
    }

    [RelayCommand]
    private void ShowComment()
        => _showMomentAction?.Invoke(this);

    [RelayCommand]
    private void Activate()
    {
        if (FindInnerContent<VideoInformation>() is VideoInformation vinfo)
        {
            this.Get<NavigationViewModel>().NavigateToOver(typeof(VideoPlayerPage).FullName, vinfo);
        }
        else if (FindInnerContent<EpisodeInformation>() is EpisodeInformation einfo)
        {
            var identifier = new MediaIdentifier("ep_" + einfo.Identifier.Id, default, default);
            this.Get<NavigationViewModel>().NavigateToOver(typeof(PgcPlayerPage).FullName, identifier);
        }
        else if(FindInnerContent<LiveInformation>() is LiveInformation linfo)
        {
            this.Get<NavigationViewModel>().NavigateToOver(typeof(LivePlayerPage).FullName, linfo);
        }
    }

    private T? FindInnerContent<T>()
        where T : class
    {
        if (Data.Data is T info)
        {
            return info;
        }
        else if (Data.Data is MomentInformation minfo && minfo.Data is T info2)
        {
            return info2;
        }

        return default;
    }
}
