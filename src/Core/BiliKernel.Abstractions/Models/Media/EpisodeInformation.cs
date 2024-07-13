// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;

namespace Richasy.BiliKernel.Models.Media;

/// <summary>
/// PGC内容的单集.
/// </summary>
public sealed class EpisodeInformation : VideoBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EpisodeInformation"/> class.
    /// </summary>
    /// <param name="identifier">单集标识.</param>
    /// <param name="duration">视频时长.</param>
    /// <param name="index">索引.</param>
    /// <param name="publishTime">发布时间.</param>
    /// <param name="communityInformation">社区信息.</param>
    public EpisodeInformation(
        MediaIdentifier identifier,
        long? duration = default,
        int? index = default,
        DateTimeOffset? publishTime = default,
        VideoCommunityInformation? communityInformation = default)
    {
        Identifier = identifier;
        Index = index;
        PublishTime = publishTime;
        CommunityInformation = communityInformation;
    }

    /// <summary>
    /// 发布时间.
    /// </summary>
    public DateTimeOffset? PublishTime { get; }

    /// <summary>
    /// 视频时长，以秒为单位.
    /// </summary>
    public long? Duration { get; }

    /// <summary>
    /// 分集排序索引.
    /// </summary>
    public int? Index { get; }

    /// <summary>
    /// 社区信息.
    /// </summary>
    public VideoCommunityInformation? CommunityInformation { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is EpisodeInformation information && EqualityComparer<MediaIdentifier>.Default.Equals(Identifier, information.Identifier);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Identifier);
}

/// <summary>
/// 分集附加数据标识.
/// </summary>
public static class EpisodeExtensionDataId
{
    /// <summary>
    /// 副标题.
    /// </summary>
    public const string Subtitle = "Subtitle";

    /// <summary>
    /// 作为视频时的 AID.
    /// </summary>
    public const string Aid = "Aid";

    /// <summary>
    /// 作为视频时的 CID.
    /// </summary>
    public const string Cid = "Cid";

    /// <summary>
    /// 剧集Id.
    /// </summary>
    public const string SeasonId = "SeasonId";

    /// <summary>
    /// 剧集类型.
    /// </summary>
    public const string SeasonType = "SeasonType";

    /// <summary>
    /// 是否为预告片.
    /// </summary>
    public const string IsPreview = "IsPreview";

    /// <summary>
    /// 单集推荐理由.
    /// </summary>
    public const string RecommendReason = "RecommendReason";

    /// <summary>
    /// 是否为会员专属剧集.
    /// </summary>
    public const string IsVip = "IsVip";

    /// <summary>
    /// 收集时间.
    /// </summary>
    public const string CollectTime = "CollectTime";
}
