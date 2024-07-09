// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using Richasy.BiliKernel.Models.User;

namespace Richasy.BiliKernel.Models.Article;

/// <summary>
/// 专栏文章信息.
/// </summary>
public sealed class ArticleInformation
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArticleInformation"/> class.
    /// </summary>
    /// <param name="identifier">文章标识符.</param>
    /// <param name="user">用户资料.</param>
    /// <param name="publishDateTime">发布时间.</param>
    /// <param name="communityInformation">社区信息.</param>
    public ArticleInformation(
        ArticleIdentifier identifier,
        UserProfile? user = default,
        DateTimeOffset? publishDateTime = default,
        ArticleCommunityInformation? communityInformation = default)
    {
        Identifier = identifier;
        Publisher = user;
        PublishDateTime = publishDateTime;
        CommunityInformation = communityInformation;
    }

    /// <summary>
    /// 文章标识.
    /// </summary>
    public ArticleIdentifier Identifier { get; }

    /// <summary>
    /// 发布者.
    /// </summary>
    public UserProfile? Publisher { get; }

    /// <summary>
    /// 发布时间.
    /// </summary>
    public DateTimeOffset? PublishDateTime { get; }

    /// <summary>
    /// 文章社区信息，包含点赞数、阅读数等.
    /// </summary>
    public ArticleCommunityInformation? CommunityInformation { get; set; }

    /// <summary>
    /// 扩展数据.
    /// </summary>
    public Dictionary<string, object>? ExtensionData { get; private set; }

    /// <summary>
    /// 添加扩展数据.
    /// </summary>
    public void AddExtensionIfNotNull<T>(string key, T? data)
    {
        if (data == null)
        {
            return;
        }

        ExtensionData ??= new Dictionary<string, object>();

        if (ExtensionData.ContainsKey(key))
        {
            ExtensionData[key] = data;
            return;
        }

        ExtensionData.Add(key, data);
    }

    /// <summary>
    /// 获取扩展数据.
    /// </summary>
    public T? GetExtensionIfNotNull<T>(string key)
    {
        return ExtensionData?.ContainsKey(key) != true
            ? default
            : (T)ExtensionData[key];
    }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is ArticleInformation information && EqualityComparer<ArticleIdentifier>.Default.Equals(Identifier, information.Identifier);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Identifier);
}

/// <summary>
/// 专栏附加数据标识.
/// </summary>
public static class ArticleExtensionDataId
{
    /// <summary>
    /// 副标题.
    /// </summary>
    public const string Subtitle = "Subtitle";

    /// <summary>
    /// 文章字数.
    /// </summary>
    public const string WordCount = "WordCount";

    /// <summary>
    /// 文章所属分区.
    /// </summary>
    public const string Partition = "Partition";

    /// <summary>
    /// 文章关联分区.
    /// </summary>
    public const string RelatedPartitions = "RelatedPartitions";

    /// <summary>
    /// 收集时间.
    /// </summary>
    public const string CollectTime = "CollectTime";

    /// <summary>
    /// 推荐理由.
    /// </summary>
    public const string RecommendReason = "RecommendReason";

    /// <summary>
    /// 与文章发布者的关系.
    /// </summary>
    public const string UserRelationStatus = "UserRelationStatus";
}
