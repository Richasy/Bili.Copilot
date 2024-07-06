// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using Richasy.BiliKernel.Models.User;

namespace Richasy.BiliKernel.Models.Media;

/// <summary>
/// 直播信息.
/// </summary>
public sealed class LiveInformation : VideoBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LiveInformation"/> class.
    /// </summary>
    /// <param name="identifier">直播间标识.</param>
    /// <param name="user">正在直播的用户资料.</param>
    /// <param name="relation">与直播UP的关系.</param>
    public LiveInformation(
        VideoIdentifier identifier,
        UserProfile? user = default,
        UserRelationStatus? relation = default)
    {
        Identifier = identifier;
        User = user;
        Relation = relation;
    }

    /// <summary>
    /// 直播 UP 主信息.
    /// </summary>
    public UserProfile User { get; }

    /// <summary>
    /// 与 UP 主的关系.
    /// </summary>
    public UserRelationStatus? Relation { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is LiveInformation information && EqualityComparer<VideoIdentifier>.Default.Equals(Identifier, information.Identifier);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Identifier);
}

/// <summary>
/// 直播附加数据标识.
/// </summary>
public static class LiveExtensionDataId
{
    /// <summary>
    /// 副标题.
    /// </summary>
    public const string Subtitle = "Subtitle";

    /// <summary>
    /// 直播间描述.
    /// </summary>
    public const string Description = "Description";

    /// <summary>
    /// 在线观看人数.
    /// </summary>
    public const string ViewerCount = "ViewerCount";

    /// <summary>
    /// 标签名称.
    /// </summary>
    public const string TagName = "TagName";

    /// <summary>
    /// 收集的时间.
    /// </summary>
    public const string CollectTime = "CollectTime";

    /// <summary>
    /// 是否正在直播.
    /// </summary>
    public const string IsLiving = "IsLiving";
}
