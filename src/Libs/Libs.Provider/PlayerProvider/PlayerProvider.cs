// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Adapter;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.Models.BiliBili;
using Bili.Copilot.Models.BiliBili.Player;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Authorize;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Data.Community;
using Bili.Copilot.Models.Data.Pgc;
using Bili.Copilot.Models.Data.Player;
using Bili.Copilot.Models.Data.Video;
using Bilibili.App.Playeronline.V1;
using Bilibili.App.View.V1;
using Bilibili.Community.Service.Dm.V1;
using static Bili.Copilot.Models.App.Constants.ApiConstants;
using static Bili.Copilot.Models.App.Constants.ServiceConstants;

namespace Bili.Copilot.Libs.Provider;

/// <summary>
/// 提供视频操作.
/// </summary>
public partial class PlayerProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerProvider"/> class.
    /// </summary>
    private PlayerProvider()
    {
    }

    /// <summary>
    /// 获取视频详细信息，包括分P内容.
    /// </summary>
    /// <param name="videoId">视频Id.</param>
    /// <returns><see cref="VideoPlayerView"/>.</returns>
    public static async Task<VideoPlayerView> GetVideoDetailAsync(string videoId)
    {
        var type = VideoToolkit.GetVideoIdType(videoId, out var avId);
        try
        {
            var viewRequest = new ViewReq();
            if (type == VideoIdType.Av && !string.IsNullOrEmpty(avId))
            {
                viewRequest.Aid = Convert.ToInt64(avId);
            }
            else if (type == VideoIdType.Bv)
            {
                viewRequest.Bvid = videoId;
            }

            viewRequest.Fnval = 16;
            var request = await HttpProvider.GetRequestMessageAsync(Video.DetailGrpc, viewRequest);
            var response = await HttpProvider.Instance.SendAsync(request);
            var data = await HttpProvider.ParseAsync(response, ViewReply.Parser);
            return VideoAdapter.ConvertToVideoView(data);
        }
        catch (ServiceException se)
        {
            if (se.Message == Messages.NoData)
            {
                // 使用网页 API 请求数据.
                var queryParameters = new Dictionary<string, string>();
                if (type == VideoIdType.Av)
                {
                    queryParameters.Add("aid", avId);
                }
                else
                {
                    queryParameters.Add("bvid", videoId);
                }

                var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Video.Detail, queryParameters, RequestClientType.IOS, needToken: false);
                var response = await HttpProvider.Instance.SendAsync(request);
                var data = await HttpProvider.ParseAsync<ServerResponse<VideoPageResponse>>(response);
                return VideoAdapter.ConvertToVideoView(data.Data);
            }
        }

        return default;
    }

    /// <summary>
    /// 获取PGC内容的详细信息.
    /// </summary>
    /// <param name="episodeId">(可选项) 单集Id.</param>
    /// <param name="seasonId">(可选项) 剧集/系列Id.</param>
    /// <param name="proxy">代理地址.</param>
    /// <param name="area">地区.</param>
    /// <returns>PGC内容详情.</returns>
    public static async Task<PgcPlayerView> GetPgcDetailAsync(string episodeId, string seasonId, string proxy = "", string area = "")
    {
        var queryParameters = GetPgcDetailInformationQueryParameters(int.Parse(episodeId), int.Parse(seasonId), area);
        var otherQuery = string.Empty;

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Models.App.Constants.ApiConstants.Pgc.SeasonDetail(proxy), queryParameters, RequestClientType.IOS, additionalQuery: otherQuery);
        var response = await HttpProvider.Instance.SendAsync(request);
        var data = await HttpProvider.ParseAsync<ServerResponse<PgcDisplayInformation>>(response);
        return PgcAdapter.ConvertToPgcPlayerView(data.Data);
    }

    /// <summary>
    /// 获取分集的交互信息，包括用户的投币/点赞/收藏.
    /// </summary>
    /// <param name="episodeId">分集Id.</param>
    /// <returns>交互信息.</returns>
    public static async Task<EpisodeInteractionInformation> GetEpisodeInteractionInformationAsync(string episodeId)
    {
        var queryParameters = GetEpisodeInteractionQueryParameters(episodeId);
        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Models.App.Constants.ApiConstants.Pgc.EpisodeInteraction, queryParameters, RequestClientType.IOS);
        var response = await HttpProvider.Instance.SendAsync(request);
        var data = await HttpProvider.ParseAsync<ServerResponse<EpisodeInteraction>>(response);
        return CommunityAdapter.ConvertToEpisodeInteractionInformation(data.Data);
    }

    /// <summary>
    /// 获取同时在线观看人数.
    /// </summary>
    /// <param name="videoId">视频Id.</param>
    /// <param name="partId">视频分P的Id.</param>
    /// <returns>同时在线观看人数.</returns>
    public static async Task<string> GetOnlineViewerCountAsync(string videoId, string partId)
    {
        var req = new PlayerOnlineReq
        {
            Aid = Convert.ToInt64(videoId),
            Cid = Convert.ToInt64(partId),
            PlayOpen = true,
        };

        var request = await HttpProvider.GetRequestMessageAsync(Video.OnlineViewerCount, req);
        var response = await HttpProvider.Instance.SendAsync(request);
        var data = await HttpProvider.ParseAsync(response, PlayerOnlineReply.Parser);
        return data.TotalText ?? "--";
    }

    /// <summary>
    /// 获取Dash播放信息.
    /// </summary>
    /// <param name="videoId">视频Id.</param>
    /// <param name="partId">视频分P的Id.</param>
    /// <returns><see cref="MediaInformation"/>.</returns>
    public static async Task<MediaInformation> GetVideoMediaInformationAsync(string videoId, string partId)
        => await InternalGetDashAsync(partId, videoId);

    /// <summary>
    /// 获取PGC的剧集Dash播放信息.
    /// </summary>
    /// <param name="partId">对应剧集的Cid.</param>
    /// <param name="episodeId">对应分集Id.</param>
    /// <param name="seasonType">剧集类型.</param>
    /// <param name="proxy">代理地址.</param>
    /// <param name="area">地区.</param>
    /// <returns><see cref="MediaInformation"/>.</returns>
    public static async Task<MediaInformation> GetPgcMediaInformationAsync(string partId, string episodeId, string seasonType, string proxy = default, string area = default)
        => await InternalGetDashAsync(partId, string.Empty, seasonType, proxy, area, episodeId);

    /// <summary>
    /// 获取分段弹幕信息.
    /// </summary>
    /// <param name="videoId">视频Id.</param>
    /// <param name="partId">视频分P的Id.</param>
    /// <param name="segmentIndex">分段索引，6分钟为一段.</param>
    /// <returns>分段弹幕信息.</returns>
    public static async Task<IEnumerable<DanmakuInformation>> GetSegmentDanmakuAsync(string videoId, string partId, int segmentIndex)
    {
        var req = new DmSegMobileReq
        {
            Pid = Convert.ToInt64(videoId),
            Oid = Convert.ToInt64(partId),
            SegmentIndex = segmentIndex,
            Type = 1,
        };
        var request = await HttpProvider.GetRequestMessageAsync(Video.SegmentDanmaku, req);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync(response, DmSegMobileReply.Parser);
        return result.Elems.Select(PlayerAdapter.ConvertToDanmakuInformation).ToList();
    }

    /// <summary>
    /// 报告播放进度记录.
    /// </summary>
    /// <param name="videoId">视频Id.</param>
    /// <param name="partId">视频分P的Id.</param>
    /// <param name="progress">播放进度.</param>
    /// <returns>进度上报是否成功.</returns>
    public static async Task<bool> ReportProgressAsync(string videoId, string partId, double progress)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.Aid, videoId.ToString() },
            { Query.Cid, partId.ToString() },
            { Query.Progress, Convert.ToInt32(progress).ToString() },
            { Query.Type, "3" },
        };

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Post, Video.ProgressReport, queryParameters, RequestClientType.IOS, true);
        var response = await HttpProvider.Instance.SendAsync(request, new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token);
        var result = await HttpProvider.ParseAsync<ServerResponse>(response);
        return result.Code == 0;
    }

    /// <summary>
    /// 报告播放进度记录.
    /// </summary>
    /// <param name="videoId">视频Id.</param>
    /// <param name="partId">分P Id.</param>
    /// <param name="episodeId">分集Id.</param>
    /// <param name="seasonId">剧集Id.</param>
    /// <param name="progress">播放进度.</param>
    /// <returns>进度上报是否成功.</returns>
    public static async Task<bool> ReportProgressAsync(string videoId, string partId, string episodeId, string seasonId, double progress)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.Aid, videoId.ToString() },
            { Query.Cid, partId.ToString() },
            { Query.EpisodeIdSlim, episodeId.ToString() },
            { Query.SeasonIdSlim, seasonId.ToString() },
            { Query.Progress, Convert.ToInt32(progress).ToString() },
            { Query.Type, "4" },
            { Query.SubType, "1" },
        };

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Post, Video.ProgressReport, queryParameters, RequestClientType.IOS, true);
        var response = await HttpProvider.Instance.SendAsync(request, new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token);
        var result = await HttpProvider.ParseAsync<ServerResponse>(response);
        return result.Code == 0;
    }

    /// <summary>
    /// 点赞视频.
    /// </summary>
    /// <param name="videoId">视频Id.</param>
    /// <param name="isLike">是否点赞.</param>
    /// <returns>结果.</returns>
    public static async Task<bool> LikeAsync(string videoId, bool isLike)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.Aid, videoId },
            { Query.Like, isLike ? "0" : "1" },
        };

        try
        {
            var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Post, Video.Like, queryParameters, needToken: true);
            var response = await HttpProvider.Instance.SendAsync(request, GetExpiryToken());
            var result = await HttpProvider.ParseAsync<ServerResponse>(response);
            return result.Code == 0;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 投币.
    /// </summary>
    /// <param name="videoId">视频Id.</param>
    /// <param name="number">投币数量，上限为2.</param>
    /// <param name="alsoLike">是否同时点赞.</param>
    /// <returns>投币结果.</returns>
    public static async Task<bool> CoinAsync(string videoId, int number, bool alsoLike)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.Aid, videoId },
            { Query.Multiply, number.ToString() },
            { Query.AlsoLike, alsoLike ? "1" : "0" },
        };

        try
        {
            var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Post, Video.Coin, queryParameters, needToken: true);
            var response = await HttpProvider.Instance.SendAsync(request, GetExpiryToken());
            var result = await HttpProvider.ParseAsync<ServerResponse<CoinResult>>(response);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 添加收藏.
    /// </summary>
    /// <param name="videoId">视频Id.</param>
    /// <param name="needAddFavoriteList">需要添加的收藏夹列表.</param>
    /// <param name="needRemoveFavoriteList">需要移除的收藏夹列表.</param>
    /// <param name="isVideo">是否为视频.</param>
    /// <returns>收藏结果.</returns>
    public static async Task<FavoriteResult> FavoriteAsync(string videoId, IEnumerable<string> needAddFavoriteList, IEnumerable<string> needRemoveFavoriteList, bool isVideo)
    {
        var resourceId = isVideo ? $"{videoId}:2" : $"{videoId}:24";
        var queryParameters = new Dictionary<string, string>
        {
            { Query.Resources, resourceId },
        };

        if (needAddFavoriteList?.Any() ?? false)
        {
            queryParameters.Add(Query.AddFavoriteIds, string.Join(",", needAddFavoriteList));
        }

        if (needRemoveFavoriteList?.Any() ?? false)
        {
            queryParameters.Add(Query.DeleteFavoriteIds, string.Join(",", needRemoveFavoriteList));
        }

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Post, Video.ModifyFavorite, queryParameters, RequestClientType.IOS, true);
        try
        {
            var response = await HttpProvider.Instance.SendAsync(request, GetExpiryToken());
        }
        catch (ServiceException ex)
        {
            var result = (FavoriteResult)ex.Error.Code;
            return result;
        }

        return FavoriteResult.Success;
    }

    /// <summary>
    /// 一键三连.
    /// </summary>
    /// <param name="videoId">视频Id.</param>
    /// <returns>三连结果.</returns>
    public static async Task<TripleInformation> TripleAsync(string videoId)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.Aid, videoId },
        };

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Post, Video.Triple, queryParameters, needToken: true);
        var response = await HttpProvider.Instance.SendAsync(request, GetExpiryToken());
        var result = await HttpProvider.ParseAsync<ServerResponse<TripleResult>>(response);
        return CommunityAdapter.ConvertToTripleInformation(result.Data, videoId);
    }

    /// <summary>
    /// 发送弹幕.
    /// </summary>
    /// <param name="content">弹幕内容.</param>
    /// <param name="videoId">视频 Id.</param>
    /// <param name="partId">分P Id.</param>
    /// <param name="progress">播放进度.</param>
    /// <param name="color">弹幕颜色.</param>
    /// <param name="isStandardSize">是否为标准字体大小.</param>
    /// <param name="location">弹幕位置.</param>
    /// <returns>是否发送成功.</returns>
    public static async Task<bool> SendDanmakuAsync(string content, string videoId, string partId, int progress, string color, bool isStandardSize, DanmakuLocation location)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.Aid, videoId },
            { Query.Type, "1" },
            { Query.Oid, partId },
            { Query.MessageSlim, content },
            { Query.Progress, (progress * 1000).ToString() },
            { Query.Color, color },
            { Query.FontSize, isStandardSize ? "25" : "18" },
            { Query.Mode, ((int)location).ToString() },
            { Query.Rnd, DateTimeOffset.Now.ToLocalTime().ToUnixTimeMilliseconds().ToString() },
        };

        try
        {
            var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Post, Video.SendDanmaku, queryParameters, RequestClientType.IOS, needToken: true);
            var response = await HttpProvider.Instance.SendAsync(request);
            var result = await HttpProvider.ParseAsync<ServerResponse>(response);
            return result.IsSuccess();
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 获取视频字幕索引.
    /// </summary>
    /// <param name="videoId">视频Id.</param>
    /// <param name="partId">分P Id.</param>
    /// <returns>字幕索引.</returns>
    public static async Task<IEnumerable<SubtitleMeta>> GetSubtitleIndexAsync(string videoId, string partId)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.Id, $"cid:{partId}" },
            { Query.Aid, videoId },
        };

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Video.Subtitle, queryParameters, RequestClientType.IOS);
        var response = await HttpProvider.Instance.SendAsync(request);
        var text = await response.ResponseMessage.Content.ReadAsStringAsync();
        if (!string.IsNullOrEmpty(text) && text.Contains("subtitle"))
        {
            var json = Regex.Match(text, @"<subtitle>(.*?)</subtitle>").Groups[1].Value;
            var index = JsonSerializer.Deserialize<SubtitleIndexResponse>(json);
            return index.Subtitles.Select(PlayerAdapter.ConvertToSubtitleMeta).ToList();
        }

        return null;
    }

    /// <summary>
    /// 获取视频字幕详情.
    /// </summary>
    /// <param name="url">字幕地址.</param>
    /// <returns>字幕详情.</returns>
    public static async Task<IEnumerable<SubtitleInformation>> GetSubtitleDetailAsync(string url)
    {
        if (!url.StartsWith("http"))
        {
            url = "https:" + url;
        }

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, url, clientType: RequestClientType.IOS, needCookie: true);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<SubtitleDetailResponse>(response);
        return result.Body.Select(PlayerAdapter.ConvertToSubtitleInformation).ToList();
    }

    /// <summary>
    /// 获取互动视频选区.
    /// </summary>
    /// <param name="videoId">视频Id.</param>
    /// <param name="graphVersion">版本号.</param>
    /// <param name="edgeId">选区Id.</param>
    /// <returns>选区响应.</returns>
    public static async Task<IEnumerable<InteractionInformation>> GetInteractionInformationsAsync(string videoId, string graphVersion, string edgeId)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.Aid, videoId },
            { Query.GraphVersion, graphVersion },
            { Query.EdgeId, edgeId },
        };

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Video.InteractionEdge, queryParameters, clientType: RequestClientType.IOS);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse<InteractionEdgeResponse>>(response);
        if (result.Data?.Edges?.Questions?.Any() ?? false)
        {
            var choices = result.Data.Edges.Questions.First().Choices;
            return choices.Select(p => PlayerAdapter.ConvertToInteractionInformation(p, result.Data.HiddenVariables)).ToList();
        }

        return null;
    }

    /// <summary>
    /// 获取视频的社区数据.
    /// </summary>
    /// <param name="videoId">视频Id.</param>
    /// <returns>社区数据.</returns>
    public static async Task<VideoCommunityInformation> GetVideoCommunityInformationAsync(string videoId)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.Aid, videoId.ToString() },
        };

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Video.Stat, queryParameters);
        var response = await HttpProvider.Instance.SendAsync(request, new CancellationTokenSource(TimeSpan.FromSeconds(3)).Token);
        var result = await HttpProvider.ParseAsync<ServerResponse<VideoStatusInfo>>(response);
        return CommunityAdapter.ConvertToVideoCommunityInformation(result.Data);
    }
}
