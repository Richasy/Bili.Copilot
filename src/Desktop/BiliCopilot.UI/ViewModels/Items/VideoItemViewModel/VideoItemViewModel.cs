// Copyright (c) Bili Copilot. All rights reserved.

using System.Globalization;
using BiliCopilot.UI.Models.Constants;
using BiliCopilot.UI.Toolkits;
using Humanizer;
using Richasy.BiliKernel.Models.Media;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 视频项视图模型.
/// </summary>
public sealed partial class VideoItemViewModel : ViewModelBase<VideoInformation>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoItemViewModel"/> class.
    /// </summary>
    public VideoItemViewModel(VideoInformation info, VideoCardStyle style)
        : base(info)
    {
        Style = style;
        Title = info.Identifier.Title;
        Cover = info.Identifier.Cover.Uri;
        Author = info.Publisher?.User?.Name;
        Avatar = info.Publisher?.User?.Avatar?.Uri;
        Duration = AppToolkit.FormatDuration(TimeSpan.FromSeconds(info.Duration ?? 0));
        PublishRelativeTime = info.PublishTime?.Humanize(culture: new CultureInfo("zh-CN"));
        PlayCount = info.CommunityInformation?.PlayCount;
        DanmakuCount = info.CommunityInformation?.DanmakuCount;
        TagName = info.GetExtensionIfNotNull<string?>(VideoExtensionDataId.TagName);
        RecommendReason = info.GetExtensionIfNotNull<string?>(VideoExtensionDataId.RecommendReason);
        Subtitle = info.GetExtensionIfNotNull<string?>(VideoExtensionDataId.Subtitle);
    }
}
