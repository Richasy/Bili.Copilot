// Copyright (c) Richasy. All rights reserved.

using System.Linq;
using Bilibili.Polymer.App.Search.V1;
using Richasy.BiliKernel.Adapters;
using Richasy.BiliKernel.Models.Article;

namespace Richasy.BiliKernel.Services.Search.Core;

internal static class ArticleAdapter
{
    public static ArticleInformation ToArticleInformation(this Item item)
    {
        var article = item.Article;
        var title = article.Title.Replace("<em class=\"keyword\">", string.Empty).Replace("</em>", string.Empty);
        var identifier = new ArticleIdentifier(article.Id.ToString(), title, article.Desc, article.ImageUrls.FirstOrDefault()?.ToArticleCover());
        var user = UserAdapterBase.CreateUserProfile(article.Mid, article.Name, default, 0d);
        var communityInfo = new ArticleCommunityInformation(article.Id.ToString(), article.View, article.Like, article.Reply);
        return new ArticleInformation(identifier, user, communityInformation: communityInfo);
    }
}
