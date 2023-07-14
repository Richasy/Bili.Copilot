// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Adapter;
using Bili.Copilot.Models.BiliBili;
using Bili.Copilot.Models.Constants.Authorize;
using Bili.Copilot.Models.Data.Article;
using Bili.Copilot.Models.Data.Live;
using Bili.Copilot.Models.Data.Pgc;
using Bili.Copilot.Models.Data.Search;
using Bili.Copilot.Models.Data.User;
using static Bili.Copilot.Models.App.Constants.ServiceConstants;

namespace Bili.Copilot.Libs.Provider;

/// <summary>
/// 搜索工具.
/// </summary>
public partial class SearchProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchProvider"/> class.
    /// </summary>
    private SearchProvider() => ClearStatus();

    /// <summary>
    /// 获取搜索建议.
    /// </summary>
    /// <param name="keyword">搜索关键词.</param>
    /// <param name="cancellationToken">异步中止令牌.</param>
    /// <returns>搜索建议列表.</returns>
    public static async Task<IEnumerable<SearchSuggest>> GetSearchSuggestionAsync(string keyword, CancellationToken cancellationToken)
    {
        var req = new Bilibili.App.Interfaces.V1.SuggestionResult3Req()
        {
            Keyword = keyword,
            Highlight = 0,
        };

        var request = await HttpProvider.GetRequestMessageAsync(Models.App.Constants.ApiConstants.Search.Suggestion, req);
        var response = await HttpProvider.Instance.SendAsync(request, cancellationToken);
        var result = await HttpProvider.ParseAsync(response, Bilibili.App.Interfaces.V1.SuggestionResult3Reply.Parser);
        return !cancellationToken.IsCancellationRequested
            ? result.List.Select(p => SearchAdapter.ConvertToSearchSuggest(p)).ToList()
            : null;
    }

    /// <summary>
    /// 获取热搜列表.
    /// </summary>
    /// <returns>热搜推荐列表.</returns>
    public static async Task<IEnumerable<SearchSuggest>> GetHotSearchListAsync()
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.Device, "phone" },
            { Query.From, "0" },
            { Query.Limit, "50" },
        };

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Models.App.Constants.ApiConstants.Search.Square, queryParameters, RequestClientType.IOS);
        var response = await HttpProvider.Instance.SendAsync(request);
        var resData = await HttpProvider.ParseAsync<ServerResponse<List<SearchSquareItem>>>(response);
        var list = resData.Data.Where(p => p.Type == "trending")
            .SelectMany(p => p.Data.List)
            .Select(p => SearchAdapter.ConvertToSearchSuggest(p));
        return list;
    }

    /// <summary>
    /// 获取文章搜索结果.
    /// </summary>
    /// <param name="keyword">搜索关键词.</param>
    /// <param name="orderType">排序方式.</param>
    /// <param name="partitionId">分区Id.</param>
    /// <returns>文章搜索结果.</returns>
    public async Task<SearchSet<ArticleInformation>> GetArticleSearchResultAsync(string keyword, string orderType, string partitionId)
    {
        var parameters = new Dictionary<string, string>
        {
            { Query.FullCategoryId, partitionId },
        };
        var data = await GetSubModuleResultAsync<ArticleSearchItem>(6, keyword, orderType, _articlePageNumber, parameters);
        _articlePageNumber++;
        var items = data.ItemList == null
            ? new List<ArticleInformation>()
            : data.ItemList.Select(p => ArticleAdapter.ConvertToArticleInformation(p));
        return new SearchSet<ArticleInformation>(items, data.PageNumber < _articlePageNumber);
    }

    /// <summary>
    /// 获取动漫搜索结果.
    /// </summary>
    /// <param name="keyword">搜索关键词.</param>
    /// <param name="orderType">排序方式.</param>
    /// <returns>响应结果.</returns>
    public async Task<SearchSet<SeasonInformation>> GetAnimeSearchResultAsync(string keyword, string orderType)
    {
        var data = await GetSubModuleResultAsync<PgcSearchItem>(7, keyword, orderType, _animePageNumber);
        _animePageNumber++;
        var items = data.ItemList == null
            ? new List<SeasonInformation>()
            : data.ItemList.Select(p => PgcAdapter.ConvertToSeasonInformation(p)).ToList();
        return new SearchSet<SeasonInformation>(items, data.PageNumber < _animePageNumber);
    }

    /// <summary>
    /// 获取电影电视剧搜索结果.
    /// </summary>
    /// <param name="keyword">搜索关键词.</param>
    /// <param name="orderType">排序方式.</param>
    /// <returns>响应结果.</returns>
    public async Task<SearchSet<SeasonInformation>> GetMovieSearchResultAsync(string keyword, string orderType)
    {
        var data = await GetSubModuleResultAsync<PgcSearchItem>(8, keyword, orderType, _moviePageNumber);
        _moviePageNumber++;
        var items = data.ItemList == null
            ? new List<SeasonInformation>()
            : data.ItemList.Select(p => PgcAdapter.ConvertToSeasonInformation(p)).ToList();
        return new SearchSet<SeasonInformation>(items, data.PageNumber < _moviePageNumber);
    }

    /// <summary>
    /// 获取用户搜索结果.
    /// </summary>
    /// <param name="keyword">搜索关键词.</param>
    /// <param name="orderType">排序方式.</param>
    /// <param name="orderSort">排序规则.</param>
    /// <param name="userType">用户类型.</param>
    /// <returns>用户搜索结果.</returns>
    public async Task<SearchSet<AccountInformation>> GetUserSearchResultAsync(string keyword, string orderType, string orderSort, string userType)
    {
        var parameters = new Dictionary<string, string>
        {
            { Query.OrderSort, orderSort },
            { Query.UserType, userType },
        };
        var data = await GetSubModuleResultAsync<UserSearchItem>(2, keyword, orderType, _userPageNumber, parameters);
        _userPageNumber++;
        var items = data.ItemList == null
            ? new List<AccountInformation>()
            : data.ItemList.Select(p => UserAdapter.ConvertToAccountInformation(p)).ToList();
        return new SearchSet<AccountInformation>(items, data.PageNumber < _userPageNumber);
    }

    /// <summary>
    /// 获取综合搜索结果.
    /// </summary>
    /// <param name="keyword">搜索关键词.</param>
    /// <param name="orderType">排序方式.</param>
    /// <param name="partitionId">分区筛选.</param>
    /// <param name="duration">时长筛选.</param>
    /// <returns>综合搜索结果.</returns>
    public async Task<ComprehensiveSet> GetComprehensiveSearchResultAsync(string keyword, string orderType, string partitionId, string duration)
    {
        var queryParameters = GetSearchBasicQueryParameters(keyword, orderType, _comprehensivePageNumber);
        queryParameters.Add(Query.Recommend, "1");
        queryParameters.Add(Query.PartitionId, partitionId);
        queryParameters.Add(Query.Duration, duration);
        queryParameters.Add(Query.HighLight, "0");
        queryParameters.Add(Query.IsOrgQuery, "0");
        queryParameters.Add(Query.Device, "phone");
        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Models.App.Constants.ApiConstants.Search.ComprehensiveSearch, queryParameters, RequestClientType.IOS);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse<ComprehensiveSearchResultResponse>>(response);
        var data = SearchAdapter.ConvertToComprehensiveSet(result.Data);
        _comprehensivePageNumber++;
        return data;
    }

    /// <summary>
    /// 获取直播间搜索结果.
    /// </summary>
    /// <param name="keyword">搜索关键词.</param>
    /// <returns>直播搜索结果.</returns>
    public async Task<SearchSet<LiveInformation>> GetLiveSearchResultAsync(string keyword)
    {
        var queryParameters = GetSearchBasicQueryParameters(keyword, string.Empty, _livePageNumber);
        queryParameters.Add(Query.Type, "4");
        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Models.App.Constants.ApiConstants.Search.LiveModuleSearch, queryParameters, RequestClientType.IOS);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse<LiveSearchResultResponse>>(response);
        _livePageNumber++;
        var items = result.Data.RoomResult?.Items == null
            ? new List<LiveInformation>()
            : result.Data.RoomResult.Items.Select(p => LiveAdapter.ConvertToLiveInformation(p));
        return new SearchSet<LiveInformation>(items, result.Data.PageNumber < _livePageNumber);
    }

    /// <summary>
    /// 重置综合搜索的请求状态.
    /// </summary>
    public void ResetComprehensiveStatus()
        => _comprehensivePageNumber = 1;

    /// <summary>
    /// 重置动漫搜索请求的状态.
    /// </summary>
    public void ResetAnimeStatus()
        => _animePageNumber = 1;

    /// <summary>
    /// 重置电影搜索请求的状态.
    /// </summary>
    public void ResetMovieStatus()
        => _moviePageNumber = 1;

    /// <summary>
    /// 重置用户搜索请求的状态.
    /// </summary>
    public void ResetUserStatus()
        => _userPageNumber = 1;

    /// <summary>
    /// 重置文章搜索请求的状态.
    /// </summary>
    public void ResetArticleStatus()
        => _articlePageNumber = 1;

    /// <summary>
    /// 重置直播搜索请求状态.
    /// </summary>
    public void ResetLiveStatus()
        => _livePageNumber = 1;

    /// <summary>
    /// 清空所有状态.
    /// </summary>
    public void ClearStatus()
    {
        ResetComprehensiveStatus();
        ResetAnimeStatus();
        ResetArticleStatus();
        ResetLiveStatus();
        ResetMovieStatus();
        ResetUserStatus();
    }
}
