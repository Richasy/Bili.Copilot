// Copyright (c) Richasy. All rights reserved.

using Bilibili.Polymer.App.Search.V1;
using Richasy.BiliKernel.Models.Article;

namespace Richasy.BiliKernel.Services.Search.Core;

internal static class ArticleAdapter
{
    public static ArticleInformation ToArticleInformation(this Item item)
    {
        var articleId = item.Param;
        return default;
    }
}
