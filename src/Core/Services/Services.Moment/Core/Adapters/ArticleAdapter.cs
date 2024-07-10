// Copyright (c) Richasy. All rights reserved.

using Bilibili.App.Dynamic.V2;
using Richasy.BiliKernel.Models.Article;

namespace Richasy.BiliKernel.Services.Moment.Core;

internal static class ArticleAdapter
{
    public static ArticleInformation ToArticleInformation(this MdlDynArticle article)
    {
        var id = article.Id;
    }
}
