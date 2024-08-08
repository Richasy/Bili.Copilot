// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
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
    public MomentItemViewModel(MomentInformation data)
        : base(data)
    {
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
                InnerContent = new MomentItemViewModel(forward);
            }
            else if (data.Data is VideoInformation video)
            {
                InnerContent = new VideoItemViewModel(video, VideoCardStyle.Moment);
            }
            else if (data.Data is IEnumerable<BiliImage> images)
            {
                InnerContent = images;
            }

            if (FindVideoInformation() is VideoInformation vinfo)
            {
                VideoTitle = vinfo.Identifier.Title ?? ResourceToolkit.GetLocalizedString(StringNames.NoTitleVideo);
                VideoCover = vinfo.Identifier.Cover.Uri;
            }
            else if (FindEpisodeInformation() is EpisodeInformation einfo)
            {
                VideoTitle = einfo.Identifier.Title;
                VideoCover = einfo.Identifier.Cover.Uri;
            }
        }
    }

    private VideoInformation? FindVideoInformation()
    {
        if (Data.Data is VideoInformation vinfo)
        {
            return vinfo;
        }
        else if (Data.Data is MomentInformation minfo && minfo.Data is VideoInformation vinfo2)
        {
            return vinfo2;
        }

        return default;
    }

    private EpisodeInformation? FindEpisodeInformation()
    {
        if (Data.Data is EpisodeInformation einfo)
        {
            return einfo;
        }
        else if (Data.Data is MomentInformation minfo && minfo.Data is EpisodeInformation einfo2)
        {
            return einfo2;
        }

        return default;
    }
}
