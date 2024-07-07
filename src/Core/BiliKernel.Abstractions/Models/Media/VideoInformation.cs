// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using Richasy.BiliKernel.Models.User;

namespace Richasy.BiliKernel.Models.Media;

/// <summary>
/// 视频基础信息.
/// </summary>
public sealed class VideoInformation : VideoBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoInformation"/> class.
    /// </summary>
    /// <param name="identifier">视频标识信息.</param>
    /// <param name="publisher">视频发布者信息.</param>
    /// <param name="bvId">视频的 BV id.</param>
    /// <param name="publishTime">视频发布时间.</param>
    /// <param name="collaborators">视频合作者信息.</param>
    /// <param name="communityInformation">社区信息.</param>
    public VideoInformation(
        VideoIdentifier identifier,
        PublisherProfile publisher,
        string? bvId = default,
        DateTimeOffset? publishTime = default,
        IList<PublisherProfile>? collaborators = default,
        VideoCommunityInformation? communityInformation = default)
    {
        Identifier = identifier;
        Publisher = publisher;
        Collaborators = collaborators;
        BvId = bvId;
        PublishTime = publishTime;
        CommunityInformation = communityInformation;
    }

    /// <summary>
    /// 备用 Id.
    /// </summary>
    /// <remarks>
    /// 该 Id 对于 BiliBili 视频来说是 bvid，在应用里，以 aid 作为主 id 进行数据处理.
    /// </remarks>
    public string? BvId { get; set; }

    /// <summary>
    /// 发布时间.
    /// </summary>
    public DateTimeOffset? PublishTime { get; }

    /// <summary>
    /// 发布者信息.
    /// </summary>
    public PublisherProfile Publisher { get; set; }

    /// <summary>
    /// 视频合作者列表.
    /// </summary>
    /// <remarks>
    /// 有时一个视频是由多位作者共同合作发布.
    /// </remarks>
    public IList<PublisherProfile>? Collaborators { get; }

    /// <summary>
    /// 社区交互数据.
    /// </summary>
    public VideoCommunityInformation? CommunityInformation { get; set; }

    

    /// <inheritdoc/>
    public override bool Equals(object obj)
        => obj is VideoInformation information && EqualityComparer<VideoIdentifier>.Default.Equals(Identifier, information.Identifier);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Identifier);
}

/// <summary>
/// 视频附加数据标识.
/// </summary>
public static class VideoExtensionDataId
{
    /// <summary>
    /// 视频描述.
    /// </summary>
    public const string Description = "Description";

    /// <summary>
    /// 视频副标题.
    /// </summary>
    public const string Subtitle = "Subtitle";

    /// <summary>
    /// 视频推荐理由.
    /// </summary>
    public const string RecommendReason = "RecommendReason";

    /// <summary>
    /// 视频标签.
    /// </summary>
    public const string TagId = "TagId";

    /// <summary>
    /// 视频标签名称.
    /// </summary>
    public const string TagName = "TagName";

    /// <summary>
    /// 视频投稿时间.
    /// </summary>
    public const string CreateTime = "CreateTime";

    /// <summary>
    /// 视频发布位置.
    /// </summary>
    public const string PublishLocation = "PublishLocation";

    /// <summary>
    /// 视频第一帧.
    /// </summary>
    public const string FirstFrame = "FirstFrame";

    /// <summary>
    /// 视频 CID.
    /// </summary>
    public const string Cid = "Cid";

    /// <summary>
    /// 视频进度.
    /// </summary>
    public const string Progress = "Progress";

    /// <summary>
    /// 视频收藏/添加进稍后再看的时间.
    /// </summary>
    public const string CollectTime = "CollectTime";

    /// <summary>
    /// 视频分享链接.
    /// </summary>
    public const string ShortLink = "ShortLink";

    /// <summary>
    /// 视频类型.
    /// </summary>
    public const string MediaType = "MediaType";

    /// <summary>
    /// 如果是番剧，这里是剧集 Id.
    /// </summary>
    public const string EpisodeId = "EpisodeId";

    /// <summary>
    /// 视频的附加菜单.
    /// </summary>
    public const string OverflowFlyout = "OverflowFlyout";
}
