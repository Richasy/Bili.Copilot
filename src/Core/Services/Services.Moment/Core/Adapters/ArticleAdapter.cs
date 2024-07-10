// Copyright (c) Richasy. All rights reserved.

using System.Linq;
using Bilibili.App.Dynamic.V2;
using Richasy.BiliKernel.Adapters;
using Richasy.BiliKernel.Models.Article;

namespace Richasy.BiliKernel.Services.Moment.Core;

internal static class ArticleAdapter
{
    public static ArticleInformation ToArticleInformation(this MdlDynArticle article)
    {
        var id = article.Id;
        var title = article.Title;
        var summary = article.Desc;
        var cover = article.Covers.FirstOrDefault()?.ToImage();
        var identifier = new ArticleIdentifier(id.ToString(), title, summary, cover);
        var info = new ArticleInformation(identifier);
        info.AddExtensionIfNotNull(ArticleExtensionDataId.Subtitle, article.Label);
        return info;
    }
}
