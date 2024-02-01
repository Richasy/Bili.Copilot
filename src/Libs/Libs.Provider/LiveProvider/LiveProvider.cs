// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Adapter;
using Bili.Copilot.Models.BiliBili;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Authorize;
using Bili.Copilot.Models.Data.Live;
using static Bili.Copilot.Models.App.Constants.ApiConstants;
using static Bili.Copilot.Models.App.Constants.ServiceConstants;

namespace Bili.Copilot.Libs.Provider;

/// <summary>
/// 提供直播相关的数据操作.
/// </summary>
public partial class LiveProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LiveProvider"/> class.
    /// </summary>
    private LiveProvider()
    {
        _feedPageNumber = 1;
        _partitionPageNumber = 1;
    }

    /// <summary>
    /// 获取直播间分区.
    /// </summary>
    /// <returns>分区列表.</returns>
    public static async Task<IEnumerable<Models.Data.Community.Partition>> GetLiveAreaIndexAsync()
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.Device, "phone" },
        };
        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Live.LiveArea, queryParameters, RequestClientType.IOS);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse<LiveAreaResponse>>(response);

        return result.Data.List.Select(CommunityAdapter.ConvertToPartition);
    }

    /// <summary>
    /// 获取直播间播放数据.
    /// </summary>
    /// <param name="roomId">直播间Id.</param>
    /// <param name="quality">清晰度.</param>
    /// <param name="audioOnly">是否仅音频.</param>
    /// <returns>播放信息.</returns>
    public static async Task<LiveMediaInformation> GetLiveMediaInformationAsync(string roomId, int quality, bool audioOnly)
    {
        var queryParameter = new Dictionary<string, string>
        {
            { Query.RoomId, roomId.ToString() },
            { Query.NoPlayUrl, "0" },
            { Query.Qn, quality.ToString() },
            { Query.Codec, Uri.EscapeDataString("0,1,2") },
            { Query.Dolby, "5" },
            { Query.Format, Uri.EscapeDataString("0,1,2") },
            { Query.OnlyAudio, audioOnly ? "1" : "0" },
            { Query.Protocol, Uri.EscapeDataString("0,1") },
            { Query.Mask, "1" },
            { "panorama", "1" },
        };

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Live.WebPlayInformation, queryParameter, RequestClientType.IOS, needCookie: true, forceNoToken: true, needCsrf: true);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse<LiveAppPlayInformation>>(response);
        return LiveAdapter.ConvertToLiveMediaInformation(result.Data);
    }

    /// <summary>
    /// 获取直播间详情.
    /// </summary>
    /// <param name="roomId">直播间Id.</param>
    /// <returns><see cref="LivePlayerView"/>.</returns>
    public static async Task<LivePlayerView> GetLiveRoomDetailAsync(string roomId)
    {
        var queryParameter = new Dictionary<string, string>
        {
            { Query.RoomId, roomId },
            { Query.Device, "phone" },
        };

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Live.RoomDetail, queryParameter, RequestClientType.IOS);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse<LiveRoomDetail>>(response);
        return LiveAdapter.ConvertToLivePlayerView(result.Data);
    }

    /// <summary>
    /// 发送消息.
    /// </summary>
    /// <param name="roomId">直播间Id.</param>
    /// <param name="message">消息内容.</param>
    /// <param name="color">弹幕颜色.</param>
    /// <param name="isStandardSize">是否为标准字体大小.</param>
    /// <param name="location">弹幕位置.</param>
    /// <returns>是否发送成功.</returns>
    public static async Task<bool> SendDanmakuAsync(string roomId, string message, string color, bool isStandardSize, DanmakuLocation location)
    {
        var queryParameter = new Dictionary<string, string>
        {
            { Query.Cid, roomId.ToString() },
            { Query.MyId, AccountProvider.Instance.UserId.ToString() },
            { Query.MessageSlim, message },
            { Query.Rnd, DateTimeOffset.Now.ToLocalTime().ToUnixTimeMilliseconds().ToString() },
            { Query.Mode, ((int)location).ToString() },
            { Query.Pool, "0" },
            { Query.Type, "json" },
            { Query.Color, color },
            { Query.FontSize, isStandardSize ? "25" : "18" },
            { Query.PlayTime, "0.0" },
        };

        try
        {
            var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Post, Live.SendMessage, queryParameter, RequestClientType.IOS, needToken: true);
            var response = await HttpProvider.Instance.SendAsync(request);
            var result = await HttpProvider.ParseAsync<ServerResponse>(response);
            return result.IsSuccess();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"直播弹幕发送失败：{ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 获取直播源列表.
    /// </summary>
    /// <param name="page">页码.</param>
    /// <returns><see cref="LiveFeedView"/>.</returns>
    public async Task<LiveFeedView> GetLiveFeedsAsync()
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.Page, _feedPageNumber.ToString() },
            { Query.RelationPage, _feedPageNumber.ToString() },
            { Query.Scale, "2" },
            { Query.LoginEvent, "1" },
            { Query.Device, "phone" },
        };
        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Live.LiveFeed, queryParameters, RequestClientType.IOS);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse<LiveFeedResponse>>(response);
        _feedPageNumber += 1;

        return LiveAdapter.ConvertToLiveFeedView(result.Data);
    }

    /// <summary>
    /// 进入直播间.
    /// </summary>
    /// <param name="roomId">直播间Id.</param>
    /// <returns>是否成功.</returns>
    public async Task<bool> EnterLiveRoomAsync(string roomId)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.RoomId, roomId },
            { Query.ActionKey, Query.AppKey },
        };

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Post, Live.EnterRoom, queryParameters, RequestClientType.IOS);
        var response = await HttpProvider.Instance.SendAsync(request);
        var data = await HttpProvider.ParseAsync<ServerResponse>(response);
        if (data.IsSuccess())
        {
            await ConnectLiveSocketAsync();
            if (_isLiveSocketConnected)
            {
                await SendLiveMessageAsync(
                            new
                            {
                                roomid = Convert.ToInt32(roomId),
                                uid = AccountProvider.Instance.UserId,
                            },
                            7);

                await SendHeartBeatAsync();
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// 获取直播分区详情.
    /// </summary>
    /// <param name="areaId">分区Id.</param>
    /// <param name="parentId">父分区Id.</param>
    /// <param name="sortType">排序方式.</param>
    /// <returns><see cref="LivePartitionView"/>.</returns>
    public async Task<LivePartitionView> GetLiveAreaDetailAsync(string areaId, string parentId, string sortType)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.Page, _partitionPageNumber.ToString() },
            { Query.PageSizeUnderline, "40" },
            { Query.AreaId, areaId.ToString() },
            { Query.ParentAreaId, parentId.ToString() },
            { Query.Device, "phone" },
        };

        if (!string.IsNullOrEmpty(sortType))
        {
            queryParameters.Add(Query.SortType, sortType);
        }

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Live.AreaDetail, queryParameters, RequestClientType.IOS);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse<LiveAreaDetailResponse>>(response);
        var data = LiveAdapter.ConvertToLivePartitionView(result.Data);
        data.Id = areaId.ToString();
        _partitionPageNumber += 1;

        return data;
    }

    /// <summary>
    /// 发送心跳包.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public async Task SendHeartBeatAsync()
    {
        if (_isLiveSocketConnected)
        {
            await SendLiveMessageAsync(string.Empty, 2);
        }
    }

    /// <summary>
    /// 接收直播消息.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public async Task LoopLiveMessageAsync()
    {
        while (!_liveCancellationToken.IsCancellationRequested && _liveWebSocket?.State == System.Net.WebSockets.WebSocketState.Open)
        {
            try
            {
                var buffer = new byte[4096];
                var result = await _liveWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _liveCancellationToken.Token);
                ParseLiveData(buffer.Take(result.Count).ToArray());
            }
            catch (Exception)
            {
                // Log data.
            }
        }
    }

    /// <summary>
    /// 重置推荐信息流.
    /// </summary>
    public void ResetPartitionDetailState()
        => _partitionPageNumber = 1;

    /// <summary>
    /// 重置分区详情的状态信息.
    /// </summary>
    public void ResetFeedState()
        => _feedPageNumber = 1;

    /// <summary>
    /// 重置直播连接.
    /// </summary>
    public void ResetLiveConnection()
    {
        _liveCancellationToken?.Cancel();
        _liveCancellationToken = new CancellationTokenSource();
        _isLiveSocketConnected = false;
        _liveWebSocket?.Dispose();
        _liveWebSocket = null;
    }
}
