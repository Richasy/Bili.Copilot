// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Adapter;
using Bili.Copilot.Models.App.Constants;
using Bili.Copilot.Models.BiliBili;
using Bili.Copilot.Models.Constants.Authorize;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Data.Article;
using static Bili.Copilot.Models.App.Constants.ServiceConstants;

namespace Bili.Copilot.Libs.Provider;

/// <summary>
/// 提供专栏文章相关的操作.
/// </summary>
public sealed partial class ArticleProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArticleProvider"/> class.
    /// </summary>
    public ArticleProvider()
        => _partitionCache = new Dictionary<string, (ArticleSortType Sort, int PageNumber)>();

    /// <summary>
    /// 获取专栏的全部分区/标签列表.
    /// </summary>
    /// <returns>标签列表.</returns>
    public static async Task<IEnumerable<Models.Data.Community.Partition>> GetPartitionsAsync()
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.Device, "phone" },
        };
        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, ApiConstants.Article.Categories, queryParameters);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse<List<ArticleCategory>>>(response);
        var partitions = result.Data.Select(CommunityAdapter.ConvertToPartition).ToList();
        partitions.Insert(0, new Models.Data.Community.Partition("0", "推荐"));
        return partitions;
    }

    /// <summary>
    /// 获取文章内容.
    /// </summary>
    /// <param name="articleId">文章 id.</param>
    /// <returns>文章内容.</returns>
    public static async Task<string> GetArticleContentAsync(string articleId)
    {
        var url = ApiConstants.Article.ArticleContent + $"?id={articleId}";
        var html = await HttpProvider.Instance.HttpClient.GetStringAsync(url);
        return html;
    }

    /// <summary>
    /// 获取推荐的文章.
    /// </summary>
    /// <param name="partitionId">分区标识符.</param>
    /// <param name="sortType">排序方式.</param>
    /// <returns>推荐文章响应结果.</returns>
    public async Task<ArticlePartitionView> GetPartitionArticlesAsync(string partitionId, ArticleSortType sortType)
    {
        var pageNumber = 1;
        if (_partitionCache.TryGetValue(partitionId, out var cache) && cache.Sort == sortType)
        {
            pageNumber = cache.PageNumber + 1;
        }

        var queryParameters = new Dictionary<string, string>
        {
            { Query.CategoryId, partitionId },
            { Query.PageNumber, pageNumber.ToString() },
            { Query.PageSizeSlim, "20" },
        };

        if (partitionId != "0")
        {
            queryParameters.Add(Query.Sort, ((int)sortType).ToString());
        }

        if (AccountProvider.Instance.UserId > 0)
        {
            queryParameters.Add(Query.MyId, AccountProvider.Instance.UserId.ToString());
        }

        var uri = partitionId == "0"
            ? ApiConstants.Article.Recommend
            : ApiConstants.Article.ArticleList;
        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, uri, queryParameters, RequestClientType.IOS);
        var response = await HttpProvider.Instance.SendAsync(request);
        ArticlePartitionView view;
        if (partitionId == "0")
        {
            var data = (await HttpProvider.ParseAsync<ServerResponse<ArticleRecommendResponse>>(response)).Data;
            view = ArticleAdapter.ConvertToArticlePartitionView(data);
        }
        else
        {
            var data = (await HttpProvider.ParseAsync<ServerResponse<List<Article>>>(response)).Data;
            view = ArticleAdapter.ConvertToArticlePartitionView(data);
        }

        _ = _partitionCache.Remove(partitionId);
        _partitionCache.Add(partitionId, (sortType, pageNumber));
        return view;
    }

    /// <summary>
    /// 重置分区请求状态.
    /// </summary>
    /// <param name="partitionId">分区 Id.</param>
    public void ResetPartitionStatus(string partitionId)
        => _partitionCache.Remove(partitionId);
}
