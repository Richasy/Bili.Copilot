// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Models;
using Richasy.BiliKernel.Models.Article;

namespace Richasy.BiliKernel.Bili.Article;

/// <summary>
/// 专栏服务.
/// </summary>
public interface IArticleService
{
    /// <summary>
    /// 获取专栏分区.
    /// </summary>
    Task<IReadOnlyList<Partition>> GetPartitionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取热门专栏分组.
    /// </summary>
    Task<Dictionary<int, string>> GetHotCategoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取热门文章.
    /// </summary>
    Task<IReadOnlyList<ArticleInformation>> GetHotArticlesAsync(int categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取推荐文章.
    /// </summary>
    Task<(IReadOnlyList<ArticleInformation> RecommendArticles, IReadOnlyList<ArticleInformation>? TopArticles, int NextPageNumber)> GetRecommendArticlesAsync(int pageNumber = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取专栏分区内的文章.
    /// </summary>
    /// <remarks>
    /// 分区可以传入主分区或子分区.
    /// </remarks>
    Task<(IReadOnlyList<ArticleInformation> RecommendArticles, int NextPageNumber)> GetPartitionArticlesAsync(Partition partition, ArticleSortType sortType, int pageNumber = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取专栏文章内容.
    /// </summary>
    /// <remarks>
    /// 这个 API 会返回 HTML 内容，最好使用浏览器来渲染.
    /// </remarks>
    Task<string?> GetArticleContentAsync(ArticleIdentifier identifier, CancellationToken cancellationToken = default);
}
