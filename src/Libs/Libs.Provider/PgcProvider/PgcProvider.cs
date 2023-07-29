// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Adapter;
using Bili.Copilot.Models.App.Constants;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.Models.BiliBili;
using Bili.Copilot.Models.Constants.Authorize;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Data.Appearance;
using Bili.Copilot.Models.Data.Pgc;

namespace Bili.Copilot.Libs.Provider;

/// <summary>
/// 获取专业内容创作数据的工具.
/// </summary>
public partial class PgcProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PgcProvider"/> class.
    /// </summary>
    private PgcProvider()
    {
        _pgcOffsetCache = new Dictionary<PgcType, string>();
        _pgcIndexCache = new Dictionary<PgcType, int>();
    }

    /// <summary>
    /// 获取顶部导航（过滤掉网页标签）.
    /// </summary>
    /// <param name="type">动漫类型.</param>
    /// <returns>顶部导航列表.</returns>
    public static async Task<IEnumerable<Models.Data.Community.Partition>> GetAnimeTabsAsync(PgcType type)
    {
        var queryParameters = GetTabQueryParameters(type);
        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, ApiConstants.Pgc.Tab, queryParameters, RequestClientType.IOS);
        var response = await HttpProvider.Instance.SendAsync(request);
        var data = await HttpProvider.ParseAsync<ServerResponse<List<PgcTab>>>(response);

        var items = data.Data.Where(p => p.Link.Contains("sub_page_id"))
            .Select(CommunityAdapter.ConvertToPartition)
            .ToList();
        return items;
    }

    /// <summary>
    /// 获取PGC索引条件.
    /// </summary>
    /// <param name="type">PGC类型.</param>
    /// <returns>PGC索引条件响应.</returns>
    public static async Task<IEnumerable<Filter>> GetPgcIndexFiltersAsync(PgcType type)
    {
        var queryParameters = GetPgcIndexBaseQueryParameters(type);
        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, ApiConstants.Pgc.IndexCondition, queryParameters, RequestClientType.IOS);
        var response = await HttpProvider.Instance.SendAsync(request);
        var data = await HttpProvider.ParseAsync<ServerResponse<PgcIndexConditionResponse>>(response);
        return PgcAdapter.ConvertToFilters(data.Data);
    }

    /// <summary>
    /// 追番/追剧.
    /// </summary>
    /// <param name="seasonId">剧Id.</param>
    /// <param name="isFollow">是否关注.</param>
    /// <returns>关注结果.</returns>
    public static async Task<bool> FollowAsync(string seasonId, bool isFollow)
    {
        var queryParameters = GetFollowQueryParameters(seasonId);
        var url = isFollow ? ApiConstants.Pgc.Follow : ApiConstants.Pgc.Unfollow;
        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Post, url, queryParameters, RequestClientType.IOS, true);
        var response = await HttpProvider.Instance.SendAsync(request);
        var data = await HttpProvider.ParseAsync<ServerResponse>(response);
        return data.IsSuccess();
    }

    /// <summary>
    /// 获取PGC内容发布时间线.
    /// </summary>
    /// <param name="type">类型.</param>
    /// <returns>时间轴响应结果.</returns>
    public static async Task<TimelineView> GetPgcTimelinesAsync(PgcType type)
    {
        var queryParameters = GetPgcTimeLineQueryParameters(type);
        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, ApiConstants.Pgc.TimeLine, queryParameters, RequestClientType.IOS);
        var response = await HttpProvider.Instance.SendAsync(request);
        var data = await HttpProvider.ParseAsync<ServerResponse2<PgcTimeLineResponse>>(response);
        return PgcAdapter.ConvertToTimelineView(data.Result);
    }

    /// <summary>
    /// 获取播放列表详情.
    /// </summary>
    /// <param name="listId">播放列表Id.</param>
    /// <returns>播放列表响应结果.</returns>
    public static async Task<PgcPlaylist> GetPgcPlaylistAsync(string listId)
    {
        var queryParameters = GetPgcPlayListQueryParameters(listId);
        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, ApiConstants.Pgc.PlayList, queryParameters, RequestClientType.IOS);
        var response = await HttpProvider.Instance.SendAsync(request);
        var data = await HttpProvider.ParseAsync<ServerResponse2<PgcPlayListResponse>>(response);
        return PgcAdapter.ConvertToPgcPlaylist(data.Result);
    }

    /// <summary>
    /// 从 Biliplus 处获取视频 Id 对应的番剧信息.
    /// </summary>
    /// <param name="videoId">视频 Id.</param>
    /// <returns><see cref="BiliPlusBangumi"/>.</returns>
    public static async Task<BiliPlusBangumi> GetBiliPlusBangumiInformationAsync(string videoId)
    {
        var handler = new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
        };
        using var client = new HttpClient(handler);
        var url = $"https://www.biliplus.com/api/view?id={videoId}";
        var response = await client.GetAsync(url);
        _ = response.EnsureSuccessStatusCode();
        var bytes = await response.Content.ReadAsByteArrayAsync();
        var str = Encoding.UTF8.GetString(bytes);
        var jObj = JsonSerializer.Deserialize<JsonElement>(str);
        var bangumi = jObj.GetProperty("bangumi").ToString();
        return JsonSerializer.Deserialize<BiliPlusBangumi>(bangumi);
    }

    /// <summary>
    /// 获取导航标签所指向的内容详情.
    /// </summary>
    /// <param name="tabId">标签Id.</param>
    /// <returns>内容详情.</returns>
    public static async Task<PgcPageView> GetPageDetailAsync(string tabId)
    {
        var queryParameters = GetPageDetailQueryParameters(tabId);
        var response = await GetPgcResponseInternalAsync(queryParameters);
        return PgcAdapter.ConvertToPgcPageView(response);
    }

    /// <summary>
    /// 获取PGC页面详情.
    /// </summary>
    /// <param name="type">类型.</param>
    /// <returns>内容详情.</returns>
    public async Task<PgcPageView> GetPageDetailAsync(PgcType type)
    {
        var cursor = _pgcOffsetCache.ContainsKey(type)
            ? _pgcOffsetCache[type]
            : string.Empty;
        var queryParameters = GetPageDetailQueryParameters(type, cursor);
        var response = await GetPgcResponseInternalAsync(queryParameters);
        _ = _pgcOffsetCache.Remove(type);
        _pgcOffsetCache.Add(type, response.NextCursor);
        return PgcAdapter.ConvertToPgcPageView(response);
    }

    /// <summary>
    /// 获取PGC索引结果.
    /// </summary>
    /// <param name="type">类型.</param>
    /// <param name="parameters">查询参数.</param>
    /// <returns>PGC索引结果响应.</returns>
    public async Task<(bool IsFinished, IEnumerable<SeasonInformation> Items)> GetPgcIndexResultAsync(PgcType type, Dictionary<string, string> parameters)
    {
        var index = _pgcIndexCache.ContainsKey(type)
                ? _pgcIndexCache[type]
                : 1;
        var queryParameters = GetPgcIndexResultQueryParameters(type, index, parameters);
        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, ApiConstants.Pgc.IndexResult, queryParameters, RequestClientType.IOS);
        var response = await HttpProvider.Instance.SendAsync(request);
        var data = await HttpProvider.ParseAsync<ServerResponse<PgcIndexResultResponse>>(response);
        var isFinish = data.Data.HasNext == 0;
        var items = data.Data.List.Select(PgcAdapter.ConvertToSeasonInformation).ToList();
        if (!isFinish)
        {
            index++;
        }

        _pgcIndexCache[type] = index;

        return (isFinish, items);
    }

    /// <summary>
    /// 重置PGC页面请求的状态.
    /// </summary>
    /// <param name="type">PGC类型.</param>
    public void ResetPageStatus(PgcType type)
        => _pgcOffsetCache.Remove(type);

    /// <summary>
    /// 重置索引页面请求状态.
    /// </summary>
    public void ResetIndexStatus(PgcType type)
        => _pgcIndexCache.Remove(type);
}
