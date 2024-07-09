// Copyright (c) Richasy. All rights reserved.

using System;
using System.Linq;
using Richasy.BiliKernel.Adapters;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Article;

namespace Richasy.BiliKernel.Services.Article.Core;

internal static class ArticleAdapter
{
    public static Partition ToPartition(this ArticleCategory category)
    {
        var id = category.Id.ToString();
        var name = category.Name;
        var children = category.Children?.Select(p => p.ToPartition()).ToList();
        var parentId = category.ParentId?.ToString();
        return new Partition(id, name, default, children, parentId);
    }

    public static ArticleInformation ToArticleInformation(this Article article)
    {
        var identifier = new ArticleIdentifier(article.Id.ToString(), article.Title, article.Summary, article.CoverUrls?.FirstOrDefault()?.ToArticleCover());
        var user = UserAdapterBase.CreateUserProfile(article.Publisher.Mid, article.Publisher.Publisher, article.Publisher.PublisherAvatar, 48d);
        var publishTime = DateTimeOffset.FromUnixTimeSeconds(article.PublishTime).ToLocalTime();
        var communityInfo = new ArticleCommunityInformation(article.Id.ToString(), article.Stats.ViewCount, article.Stats.FavoriteCount, article.Stats.LikeCount, article.Stats.ReplyCount, article.Stats.ShareCount, article.Stats.CoinCount);
        var info = new ArticleInformation(identifier, user, publishTime, communityInfo);
        info.AddExtensionIfNotNull(ArticleExtensionDataId.WordCount, article.WordCount);
        info.AddExtensionIfNotNull(ArticleExtensionDataId.Partition, article.Category.ToPartition());
        info.AddExtensionIfNotNull(ArticleExtensionDataId.RelatedPartitions, article.RelatedCategories?.Select(p => p.ToPartition()).ToList());
        return info;
    }
}
