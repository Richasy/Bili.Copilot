// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;

namespace Richasy.BiliKernel.Models.Media;

/// <summary>
/// 剧集信息.
/// </summary>
public sealed class SeasonInformation : VideoBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SeasonInformation"/> class.
    /// </summary>
    /// <param name="identifier">剧集标识.</param>
    /// <param name="communityInformation">社区交互信息.</param>
    /// <param name="isTracking">是否已追番/追剧.</param>
    public SeasonInformation(
        VideoIdentifier identifier,
        VideoCommunityInformation communityInformation = default,
        bool? isTracking = default)
    {
        Identifier = identifier;
        CommunityInformation = communityInformation;
        IsTracking = isTracking;
    }

    /// <summary>
    /// 社区交互信息.
    /// </summary>
    public VideoCommunityInformation CommunityInformation { get; }

    /// <summary>
    /// 是否追番/追剧.
    /// </summary>
    public bool? IsTracking { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is SeasonInformation information && EqualityComparer<VideoIdentifier>.Default.Equals(Identifier, information.Identifier);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Identifier);
}

/// <summary>
/// 剧集附加数据标识.
/// </summary>
public static class SeasonExtensionDataId
{
    /// <summary>
    /// 副标题.
    /// </summary>
    public const string Subtitle = "Subtitle";

    /// <summary>
    /// 创作内容类型.
    /// </summary>
    public const string PgcType = "PgcType";

    /// <summary>
    /// 高亮信息.
    /// </summary>
    public const string Highlight = "Highlight";

    /// <summary>
    /// 标签.
    /// </summary>
    public const string Tags = "Tags";

    /// <summary>
    /// 评分人数.
    /// </summary>
    public const string RatingCount = "RatingCount";

    /// <summary>
    /// 原始名称.
    /// </summary>
    public const string OriginName = "OriginName";

    /// <summary>
    /// 别名.
    /// </summary>
    public const string Alias = "Alias";

    /// <summary>
    /// 发布日期的可读文本. 比如 <c>2022年5月1日开播</c>.
    /// </summary>
    public const string DisplayReleaseDate = "ReleaseDate";

    /// <summary>
    /// 连载进度的可读文本. 比如 <c>已完结，共1话</c>.
    /// </summary>
    public const string DisplayProgress = "Progress";

    /// <summary>
    /// 剧集简介.
    /// </summary>
    public const string Description = "Description";

    /// <summary>
    /// 台前幕后相关人员的说明.
    /// </summary>
    /// <remarks>
    /// 一般来说，服务器返回的演职人员及制作人员的信息都是换行文本，采用 <c>标题</c> + <c>内容</c> 的组织形式.
    /// 这里暂不做改动，<c>Key</c> 表示该区块的标题，比如 <c>角色声优</c>，<c>Value</c> 表示具体的内容.
    /// </remarks>
    public const string LaborSections = "LaborSections";

    /// <summary>
    /// 参演人员中的明星，他们有自己的头像和角色说明.
    /// </summary>
    public const string Celebrity = "Celebrity";

    /// <summary>
    /// 发布时间.
    /// </summary>
    public const string PublishTime = "PublishTime";

    /// <summary>
    /// 分集 ID.
    /// </summary>
    public const string EpisodeId = "EpisodeId";
}
