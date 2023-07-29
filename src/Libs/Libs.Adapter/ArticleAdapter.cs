// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.BiliBili;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Article;
using Bili.Copilot.Models.Data.Community;
using Bilibili.App.Dynamic.V2;
using Humanizer;

namespace Bili.Copilot.Libs.Adapter;

/// <summary>
/// 文章数据适配器.
/// </summary>
public static class ArticleAdapter
{
    /// <summary>
    /// 将专栏文章 <see cref="Article"/> 转换成文章信息.
    /// </summary>
    /// <param name="article">专栏文章.</param>
    /// <returns><see cref="ArticleInformation"/>.</returns>
    public static ArticleInformation ConvertToArticleInformation(Article article)
    {
        var id = article.Id.ToString();
        var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(article.Title);
        var summary = TextToolkit.ConvertToTraditionalChineseIfNeeded(article.Summary);
        var cover = article.CoverUrls?.Any() ?? false
            ? ImageAdapter.ConvertToArticleCardCover(article.CoverUrls.First())
            : null;
        var partition = CommunityAdapter.ConvertToPartition(article.Category);
        var relatedPartitions = article.RelatedCategories?.Any() ?? false
            ? article.RelatedCategories.Select(CommunityAdapter.ConvertToPartition).ToList()
            : null;
        var publishTime = DateTimeOffset.FromUnixTimeSeconds(article.PublishTime).ToLocalTime().DateTime;
        var user = UserAdapter.ConvertToRoleProfile(article.Publisher, AvatarSize.Size48);
        var subtitle = $"{user.User.Name} · {TextToolkit.ConvertToTraditionalChineseIfNeeded(publishTime.Humanize())}";
        var wordCount = article.WordCount;
        var communityInfo = CommunityAdapter.ConvertToArticleCommunityInformation(article.Stats, id);
        var identifier = new ArticleIdentifier(id, title, summary, cover);
        return new ArticleInformation(
            identifier,
            subtitle,
            partition,
            relatedPartitions,
            user,
            publishTime,
            communityInfo,
            wordCount);
    }

    /// <summary>
    /// 将专栏文章搜索结果 <see cref="ArticleSearchItem"/> 转换成文章信息.
    /// </summary>
    /// <param name="item">专栏文章搜索结果.</param>
    /// <returns><see cref="ArticleInformation"/>.</returns>
    public static ArticleInformation ConvertToArticleInformation(ArticleSearchItem item)
    {
        var id = item.Id.ToString();
        var title = Regex.Replace(item.Title, "<[^>]+>", string.Empty);
        title = TextToolkit.ConvertToTraditionalChineseIfNeeded(title);
        var summary = TextToolkit.ConvertToTraditionalChineseIfNeeded(item.Description);
        var cover = item.CoverUrls?.Any() ?? false
            ? ImageAdapter.ConvertToArticleCardCover(item.CoverUrls.First())
            : null;
        var subtitle = item.Name;
        var communityInfo = CommunityAdapter.ConvertToArticleCommunityInformation(item);
        var identifier = new ArticleIdentifier(id, title, summary, cover);
        return new ArticleInformation(
            identifier,
            subtitle,
            communityInformation: communityInfo);
    }

    /// <summary>
    /// 将收藏的专栏文章 <see cref="ArticleSearchItem"/> 转换成文章信息.
    /// </summary>
    /// <param name="item">收藏的专栏文章.</param>
    /// <returns><see cref="ArticleInformation"/>.</returns>
    public static ArticleInformation ConvertToArticleInformation(FavoriteArticleItem item)
    {
        var id = item.Id.ToString();
        var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(item.Title);
        var summary = TextToolkit.ConvertToTraditionalChineseIfNeeded(item.Summary);
        var cover = item.Images?.Any() ?? false
            ? ImageAdapter.ConvertToArticleCardCover(item.Images.First())
            : null;
        var collectTime = DateTimeOffset.FromUnixTimeSeconds(item.CollectTime).DateTime;
        var subtitle = TextToolkit.ConvertToTraditionalChineseIfNeeded($"{collectTime.Humanize()}收藏");
        var identifier = new ArticleIdentifier(id, title, summary, cover);
        return new ArticleInformation(
            identifier,
            subtitle);
    }

    /// <summary>
    /// 将动态文章 <see cref="MdlDynArticle"/> 转换成文章信息.
    /// </summary>
    /// <param name="article">动态文章.</param>
    /// <returns><see cref="ArticleInformation"/>.</returns>
    public static ArticleInformation ConvertToArticleInformation(MdlDynArticle article)
    {
        var id = article.Id.ToString();
        var title = TextToolkit.ConvertToTraditionalChineseIfNeeded(article.Title);
        var summary = TextToolkit.ConvertToTraditionalChineseIfNeeded(article.Desc);
        var cover = article.Covers?.Any() ?? false
            ? ImageAdapter.ConvertToArticleCardCover(article.Covers.First())
            : null;
        var identifier = new ArticleIdentifier(id, title, summary, cover);
        return new ArticleInformation(
            identifier,
            article.Label);
    }

    /// <summary>
    /// 将专栏推荐响应结果 <see cref="ArticleRecommendResponse"/> 转换成分区文章视图.
    /// </summary>
    /// <param name="response">专栏推荐响应结果.</param>
    /// <returns><see cref="ArticlePartitionView"/>.</returns>
    public static ArticlePartitionView ConvertToArticlePartitionView(ArticleRecommendResponse response)
    {
        var articles = response.Articles?.Any() ?? false
            ? response.Articles.Select(ConvertToArticleInformation)
            : null;

        var ranks = response.Ranks?.Any() ?? false
            ? response.Ranks.Select(ConvertToArticleInformation)
            : null;

        IEnumerable<BannerIdentifier> banners = null;
        if (response.Banners?.Any() ?? false)
        {
            var tempBanners = response.Banners.ToList();
            tempBanners.ForEach(p => p.NavigateUri = $"https://www.bilibili.com/read/cv{p.Id}");
            banners = tempBanners.Select(CommunityAdapter.ConvertToBannerIdentifier);
        }

        return new ArticlePartitionView(articles, banners, ranks);
    }

    /// <summary>
    /// 将专栏文章列表转换成分区文章视图.
    /// </summary>
    /// <param name="articles">文章列表.</param>
    /// <returns><see cref="ArticlePartitionView"/>.</returns>
    public static ArticlePartitionView ConvertToArticlePartitionView(IEnumerable<Article> articles)
    {
        var items = articles.Select(ConvertToArticleInformation);
        return new ArticlePartitionView(items);
    }

    /// <summary>
    /// 将收藏的专栏文章响应 <see cref="ArticleFavoriteListResponse"/> 转换为文章集.
    /// </summary>
    /// <param name="response">收藏的专栏文章响应.</param>
    /// <returns><see cref="ArticleSet"/>.</returns>
    public static ArticleSet ConvertToArticleSet(ArticleFavoriteListResponse response)
    {
        var count = response.Count;
        var items = response.Items.Select(ConvertToArticleInformation);
        return new ArticleSet(items, count);
    }
}
