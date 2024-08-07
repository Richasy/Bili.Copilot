// Copyright (c) Bili Copilot. All rights reserved.

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
        if (FindVideoInformation() is VideoInformation vinfo)
        {
            VideoTitle = vinfo.Identifier.Title;
            VideoCover = vinfo.Identifier.Cover.Uri;
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
}
