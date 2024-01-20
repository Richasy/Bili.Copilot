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
using Bili.Copilot.Models.Data.Community;
using Bili.Copilot.Models.Data.Video;
using Bilibili.App.Show.V1;
using static Bili.Copilot.Models.App.Constants.ServiceConstants;

namespace Bili.Copilot.Libs.Provider;

/// <summary>
/// 提供分区及标签的数据操作.
/// </summary>
public sealed partial class HomeProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HomeProvider"/> class.
    /// </summary>
    private HomeProvider()
    {
        _hotOffsetId = 0;
        _recommendOffsetId = 0;
        _cacheVideoPartitionOffsets = new Dictionary<string, (int OffsetId, int PageNumber)>();
    }

    /// <summary>
    /// 获取精选视频详情.
    /// </summary>
    /// <returns>精选视频列表.</returns>
    public static async Task<IEnumerable<VideoInformation>> GetFeaturedVideosAsync()
    {
        var queryParameters = new Dictionary<string, string>
        {
            { "fresh_idx", "1" },
            { Query.PageSizeSlim, "10" },
            { Query.PlatformSlim, "1" },
            { "feed_version", "CLIENT_SELECTED" },
            { "fresh_type", "0" },
        };

        var request = await HttpProvider.GetRequestMessageAsync(
                       HttpMethod.Get,
                       ApiConstants.Home.Featured,
                       queryParameters,
                       RequestClientType.Web,
                       needCookie: true);
        var response = await HttpProvider.Instance.SendAsync(request);
        var data = await HttpProvider.ParseAsync<ServerResponse<WebRecommendResponse>>(response);
        var result = data.Data.Items
            .Where(p => p.Goto == Av)
            .Select(VideoAdapter.ConvertToVideoInformation)
            .ToList();
        return result;
    }

    /// <summary>
    /// 获取排行榜详情.
    /// </summary>
    /// <param name="partitionId">分区Id. 如果是全区则为0.</param>
    /// <returns>排行榜信息.</returns>
    public static async Task<IEnumerable<VideoInformation>> GetRankDetailAsync(string partitionId)
    {
        var rankRequest = new RankRegionResultReq() { Rid = System.Convert.ToInt32(partitionId) };
        var request = await HttpProvider.GetRequestMessageAsync(ApiConstants.Home.RankingGRPC, rankRequest);
        var response = await HttpProvider.Instance.SendAsync(request);
        var data = await HttpProvider.ParseAsync(response, RankListReply.Parser);
        return data.Items.ToList().Select(VideoAdapter.ConvertToVideoInformation);
    }

    /// <summary>
    /// 获取视频分区索引.
    /// </summary>
    /// <returns>全部分区索引，但不包括需要网页显示的分区.</returns>
    public static async Task<IEnumerable<Models.Data.Community.Partition>> GetVideoPartitionIndexAsync()
    {
        var localCache = await GetPartitionCacheAsync();

        if (localCache != null)
        {
            return localCache;
        }

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, ApiConstants.Partition.PartitionIndex, clientType: RequestClientType.IOS);
        var response = await HttpProvider.Instance.SendAsync(request);
        var data = await HttpProvider.ParseAsync<ServerResponse<List<Models.BiliBili.Partition>>>(response);

        var items = data.Data.Where(p => !string.IsNullOrEmpty(p.Uri) &&
                                    p.Uri.StartsWith("bilibili") &&
                                    p.Uri.Contains("region/") &&
                                    p.Children != null &&
                                    p.Children.Count > 0);
        var result = items.Select(CommunityAdapter.ConvertToPartition);
        await CachePartitionsAsync(result);
        return result;
    }

    /// <summary>
    /// 请求推荐视频列表.
    /// </summary>
    /// <returns>推荐视频或番剧的列表.</returns>
    /// <remarks>
    /// 视频推荐请求返回的是一个信息流，每请求一次返回一个固定数量的集合。
    /// 除第一次请求外，后一次请求依赖前一次请求的偏移值，以避免出现重复的视频.
    /// 视频推荐服务内置偏移值管理，如果要重置偏移值（比如需要刷新），可以调用 <c>Reset</c> 方法重置.
    /// </remarks>
    public async Task<IEnumerable<IVideoBase>> RequestRecommendVideosAsync()
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.Idx, _recommendOffsetId.ToString() },
            { Query.Flush, "5" },
            { Query.Column, "4" },
            { Query.Device, "pad" },
            { Query.DeviceName, "iPad 6" },
            { Query.Pull, (_recommendOffsetId == 0).ToString().ToLower() },
        };

        var request = await HttpProvider.GetRequestMessageAsync(
            HttpMethod.Get,
            ApiConstants.Home.Recommend,
            queryParameters,
            RequestClientType.IOS);
        var response = await HttpProvider.Instance.SendAsync(request);
        var data = await HttpProvider.ParseAsync<ServerResponse<HomeRecommendInfo>>(response);
        var offsetId = data.Data.Items.Last().Index;
        var items = data.Data.Items.Where(p => !string.IsNullOrEmpty(p.Goto)).ToList();

        // 目前只接受视频和剧集.
        var list = new List<IVideoBase>();
        foreach (var item in items)
        {
            if (item.CardGoto == Av)
            {
                list.Add(VideoAdapter.ConvertToVideoInformation(item));
            }
            else if (item.CardGoto is Bangumi
                or ServiceConstants.Pgc)
            {
                list.Add(PgcAdapter.ConvertToEpisodeInformation(item));
            }
        }

        _recommendOffsetId = offsetId;
        return list;
    }

    /// <summary>
    /// 获取热门详情.
    /// </summary>
    /// <returns>热门视频信息.</returns>
    public async Task<IEnumerable<VideoInformation>> RequestHotVideosAsync()
    {
        var isLogin = AuthorizeProvider.Instance.State == AuthorizeState.SignedIn;
        var popularReq = new PopularResultReq()
        {
            Idx = _hotOffsetId,
            LoginEvent = isLogin ? 2 : 1,
            Qn = 112,
            Fnval = 464,
            Fourk = 1,
            Spmid = "creation.hot-tab.0.0",
            PlayerArgs = new Bilibili.App.Archive.Middleware.V1.PlayerArgs
            {
                Qn = 112,
                Fnval = 464,
            },
        };
        var request = await HttpProvider.GetRequestMessageAsync(ApiConstants.Home.PopularGRPC, popularReq);
        var response = await HttpProvider.Instance.SendAsync(request);
        var data = await HttpProvider.ParseAsync(response, PopularReply.Parser);
        var result = data.Items
            .Where(p => p.ItemCase == Bilibili.App.Card.V1.Card.ItemOneofCase.SmallCoverV5)
            .Where(p => p.SmallCoverV5 != null)
            .Where(p => p.SmallCoverV5.Base.CardGoto == Av)
            .Where(p => !p.SmallCoverV5.Base.Uri.Contains("bangumi"))
            .Select(VideoAdapter.ConvertToVideoInformation);
        _hotOffsetId = data.Items.Where(p => p.SmallCoverV5 != null).Last().SmallCoverV5.Base.Idx;

        return result;
    }

    /// <summary>
    /// 获取视频子分区数据.
    /// </summary>
    /// <param name="subPartitionId">子分区Id.</param>
    /// <param name="isRecommend">是否是推荐子分区.</param>
    /// <param name="sortType">排序方式.</param>
    /// <returns>返回的子分区数据.</returns>
    public async Task<VideoPartitionView> GetVideoSubPartitionDataAsync(
        string subPartitionId,
        bool isRecommend,
        VideoSortType sortType = VideoSortType.Default)
    {
        RetriveCachedSubPartitionOffset(subPartitionId);
        var isOffset = _videoPartitionOffsetId > 0;
        var isDefaultOrder = sortType == VideoSortType.Default;
        SubPartition data;

        var requestUrl = isRecommend
            ? isOffset ? ApiConstants.Partition.SubPartitionRecommendOffset : ApiConstants.Partition.SubPartitionRecommend
            : !isDefaultOrder
                ? ApiConstants.Partition.SubPartitionOrderOffset
                : isOffset ? ApiConstants.Partition.SubPartitionNormalOffset : ApiConstants.Partition.SubPartitionNormal;

        var queryParameters = new Dictionary<string, string>
        {
            { Query.PartitionId, subPartitionId },
            { Query.Pull, "0" },
        };

        if (isOffset)
        {
            queryParameters.Add(Query.CreateTime, _videoPartitionOffsetId.ToString());
        }

        if (!isDefaultOrder)
        {
            var sortStr = string.Empty;
            switch (sortType)
            {
                case VideoSortType.Newest:
                    sortStr = Sort.Newest;
                    break;
                case VideoSortType.Play:
                    sortStr = Sort.Play;
                    break;
                case VideoSortType.Reply:
                    sortStr = Sort.Reply;
                    break;
                case VideoSortType.Danmaku:
                    sortStr = Sort.Danmaku;
                    break;
                case VideoSortType.Favorite:
                    sortStr = Sort.Favorite;
                    break;
                default:
                    break;
            }

            queryParameters.Add(Query.Order, sortStr);
            queryParameters.Add(Query.PageNumber, _videoPartitionPageNumber.ToString());
            queryParameters.Add(Query.PageSizeSlim, "30");
        }

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, requestUrl, queryParameters, RequestClientType.IOS);
        var response = await HttpProvider.Instance.SendAsync(request);
        if (isOffset)
        {
            data = (await HttpProvider.ParseAsync<ServerResponse<SubPartition>>(response)).Data;
        }
        else if (!isRecommend)
        {
            if (!isDefaultOrder)
            {
                var list = (await HttpProvider.ParseAsync<ServerResponse<List<PartitionVideo>>>(response)).Data;
                data = new SubPartition()
                {
                    NewVideos = list,
                };
            }
            else
            {
                data = (await HttpProvider.ParseAsync<ServerResponse<SubPartitionDefault>>(response)).Data;
            }
        }
        else
        {
            data = (await HttpProvider.ParseAsync<ServerResponse<Models.BiliBili.SubPartitionRecommend>>(response)).Data;
        }

        var id = subPartitionId;
        var videos = data.NewVideos
            .Concat(data.RecommendVideos ?? new List<PartitionVideo>())
            .Select(VideoAdapter.ConvertToVideoInformation);
        IEnumerable<BannerIdentifier> banners = null;
        if (data is Models.BiliBili.SubPartitionRecommend recommendView
            && recommendView.Banner != null)
        {
            banners = recommendView.Banner.TopBanners.Select(CommunityAdapter.ConvertToBannerIdentifier);
        }

        _videoPartitionOffsetId = data.BottomOffsetId;
        _videoPartitionPageNumber = !isRecommend && sortType != VideoSortType.Default ? _videoPartitionPageNumber + 1 : 1;

        UpdateVideoPartitionCache();

        return new VideoPartitionView(id, videos, banners);
    }

    /// <summary>
    /// 重置子分区请求状态，将偏移值归零.
    /// </summary>
    /// <param name="id">指定要清除的分区 Id.</param>
    /// <remarks>
    /// 分区 Id 默认为空，如果设定该参数，则会在清除后，将内部存储的当前分区设置为该分区.
    /// </remarks>
    public void ResetSubPartitionState(string id = default)
    {
        if (!string.IsNullOrEmpty(id))
        {
            RetriveCachedSubPartitionOffset(id);
        }

        _videoPartitionOffsetId = 0;
        _videoPartitionPageNumber = 1;
        UpdateVideoPartitionCache();
    }

    /// <summary>
    /// 重置分区请求状态，将缓存和偏移值归零.
    /// </summary>
    public void ClearPartitionState()
    {
        ResetSubPartitionState();
        _cacheVideoPartitionOffsets.Clear();
    }

    /// <summary>
    /// 重置推荐列表状态，将偏移值归零.
    /// </summary>
    public void ResetRecommendState()
        => _recommendOffsetId = 0;

    /// <summary>
    /// 重置热门视频状态，将偏移值归零.
    /// </summary>
    public void ResetHotState()
        => _hotOffsetId = 0;
}
