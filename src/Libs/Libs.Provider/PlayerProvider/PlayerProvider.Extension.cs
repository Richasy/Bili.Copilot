// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Adapter;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Constants;
using Bili.Copilot.Models.BiliBili;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Authorize;
using Bili.Copilot.Models.Data.Player;
using Bilibili.App.Playurl.V1;
using static Bili.Copilot.Models.App.Constants.ApiConstants;
using static Bili.Copilot.Models.App.Constants.ServiceConstants;

namespace Bili.Copilot.Libs.Provider;

/// <summary>
/// 提供视频相关操作.
/// </summary>
public sealed partial class PlayerProvider
{
    private static readonly Lazy<PlayerProvider> _lazyInstance = new(() => new PlayerProvider());

    /// <summary>
    /// 实例.
    /// </summary>
    public static PlayerProvider Instance => _lazyInstance.Value;

    private static CancellationToken GetExpiryToken(int seconds = 5)
    {
        var source = new CancellationTokenSource(TimeSpan.FromSeconds(seconds));
        return source.Token;
    }

    private static Dictionary<string, string> GetEpisodeInteractionQueryParameters(string episodeId)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.EpisodeId, episodeId },
        };

        return queryParameters;
    }

    private static Dictionary<string, string> GetPgcDetailInformationQueryParameters(int episodeId, int seasonId, string area)
    {
        var queryParameters = new Dictionary<string, string>
        {
            { Query.AutoPlay, "0" },
            { Query.IsShowAllSeries, "0" },
        };

        if (!string.IsNullOrEmpty(area))
        {
            queryParameters.Add(Query.Area, area);
        }

        if (episodeId > 0)
        {
            queryParameters.Add(Query.EpisodeId, episodeId.ToString());
        }

        if (seasonId > 0)
        {
            queryParameters.Add(Query.SeasonId, seasonId.ToString());
        }

        return queryParameters;
    }

    private static async Task<MediaInformation> InternalGetDashAsync(string cid, string aid = "", string seasonType = "", string proxy = "", string area = "", string episodeId = "")
    {
        var isPgc = string.IsNullOrEmpty(aid) && !string.IsNullOrEmpty(seasonType);

        var url = isPgc ? ApiConstants.Pgc.PlayInformation(proxy) : ApiConstants.Video.PlayInformation;
        var requestType = isPgc ? RequestClientType.Web : RequestClientType.IOS;

        var queryParameters = new Dictionary<string, string>
        {
            { Query.Fnver, "0" },
            { Query.Cid, cid.ToString() },
            { Query.Fourk, "1" },
            { Query.Fnval, "4048" },
            { Query.Qn, "64" },
            { Query.OType, "json" },
        };

        if (isPgc)
        {
            queryParameters.Add(Query.Module, "bangumi");
            queryParameters.Add(Query.SeasonType, seasonType);
            queryParameters.Add(Query.EpisodeId, episodeId);
        }
        else
        {
            queryParameters.Add(Query.AVid, aid);
        }

        if (AccountProvider.Instance.UserId != 0)
        {
            queryParameters.Add(Query.MyId, AccountProvider.Instance.UserId.ToString());
        }

        var otherQuery = string.Empty;
        if (!string.IsNullOrEmpty(area))
        {
            otherQuery = $"area={area}";
        }

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, url, queryParameters, requestType, additionalQuery: otherQuery);
        var response = await HttpProvider.Instance.SendAsync(request);
        var data = await HttpProvider.ParseAsync<ServerResponse<PlayerInformation>, ServerResponse2<PlayerInformation>>(response.ResponseMessage, (str) =>
        {
            var jobj = JsonSerializer.Deserialize<JsonElement>(str);
            return jobj.TryGetProperty("data", out var _);
        });

        if (data is ServerResponse<PlayerInformation> res1)
        {
            return PlayerAdapter.ConvertToMediaInformation(res1.Data);
        }
        else if (data is ServerResponse2<PlayerInformation> res2)
        {
            return PlayerAdapter.ConvertToMediaInformation(res2.Result);
        }

        return null;
    }

    private static async Task<MediaInformation> InternalGetDashFromgRPCAsync(string videoId, string partId)
    {
        var preferCodec = SettingsToolkit.ReadLocalSetting(SettingNames.PreferCodec, PreferCodec.H264);
        var codeType = preferCodec switch
        {
            PreferCodec.H265 => CodeType.Code265,
            PreferCodec.H264 => CodeType.Code264,
            PreferCodec.Av1 => CodeType.Codeav1,
            _ => CodeType.Code264,
        };

        var playUrlReq = new PlayViewReq
        {
            Aid = Convert.ToInt64(videoId),
            Cid = Convert.ToInt64(partId),
            Fourk = true,
            Fnval = 4048,
            Qn = 64,
            ForceHost = 2,
            PreferCodecType = codeType,
        };
        var appReq = await HttpProvider.GetRequestMessageAsync(Video.PlayUrl, playUrlReq);
        var appRes = await HttpProvider.Instance.SendAsync(appReq);
        var reply = await HttpProvider.ParseAsync(appRes, PlayViewReply.Parser);
        return PlayerAdapter.ConvertToMediaInformation(reply);
    }
}
