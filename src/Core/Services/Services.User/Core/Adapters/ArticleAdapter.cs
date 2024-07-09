// Copyright (c) Richasy. All rights reserved.

using System;
using System.Linq;
using Bilibili.App.Interface.V1;
using Richasy.BiliKernel.Adapters;
using Richasy.BiliKernel.Models.Article;
using Richasy.BiliKernel.Models.User;

namespace Richasy.BiliKernel.Services.User.Core;

internal static class ArticleAdapter
{
    public static ArticleInformation ToArticleInformation(this CursorItem cursorItem)
    {
        var article = cursorItem.CardArticle;
        var title = cursorItem.Title;
        var viewTime = DateTimeOffset.FromUnixTimeSeconds(cursorItem.ViewAt).ToLocalTime();
        var articleId = cursorItem.Kid.ToString();
        var coverUrl = article.Covers.FirstOrDefault();
        var cover = !string.IsNullOrEmpty(coverUrl) ? coverUrl.ToImage() : default;
        var identifier = new ArticleIdentifier(articleId, title, default, cover);
        var user = UserAdapterBase.CreateUserProfile(article.Mid, article.Name, default, 0d);
        var relation = article.Relation.Status switch
        {
            1 => UserRelationStatus.Unfollow,
            2 => UserRelationStatus.Following,
            3 => UserRelationStatus.BeFollowed,
            4 => UserRelationStatus.Friends,
            _ => UserRelationStatus.Unknown,
        };

        var info = new ArticleInformation(identifier, user);
        info.AddExtensionIfNotNull(ArticleExtensionDataId.CollectTime, viewTime);
        info.AddExtensionIfNotNull(ArticleExtensionDataId.RecommendReason, string.IsNullOrEmpty(article.Badge) ? default : article.Badge);
        info.AddExtensionIfNotNull(ArticleExtensionDataId.UserRelationStatus, relation);
        return info;
    }
}
