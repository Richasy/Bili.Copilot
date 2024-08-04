// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Bili.Copilot.Models.Constants.Authorize;
using Bili.Copilot.Models.Grpc;
using Flurl.Http;
using Google.Protobuf;
using static Bili.Copilot.Models.App.Constants.ServiceConstants;

namespace Bili.Copilot.Libs.Provider;

/// <summary>
/// 用于网络请求的模块.
/// </summary>
public sealed partial class HttpProvider
{
    /// <summary>
    /// 创建 <see cref="HttpProvider"/> 的实例.
    /// </summary>
    private HttpProvider() => InitHttpClient();

    /// <summary>
    /// 获取 <see cref="FlurlRequest"/>.
    /// </summary>
    /// <param name="method">请求方法.</param>
    /// <param name="url">请求地址.</param>
    /// <param name="queryParams">查询参数.</param>
    /// <param name="clientType">需要模拟的设备类型.</param>
    /// <param name="needToken">是否需要令牌.</param>
    /// <param name="additionalQuery">附加查询参数.</param>
    /// <param name="needCookie">是否需要cookie.</param>
    /// <param name="needAppKey">是否需要appKey.</param>
    /// <param name="forceNoToken">是否强制不AccessKey.</param>
    /// <param name="needRid">是否需要 RID 签名.</param>
    /// <param name="needCsrf">是否需要 CSRF 令牌.</param>
    /// <returns><see cref="FlurlRequest"/>.</returns>
    public static async Task<IFlurlRequest> GetRequestMessageAsync(
        HttpMethod method,
        string url,
        Dictionary<string, string> queryParams = null,
        RequestClientType clientType = RequestClientType.Android,
        bool needToken = true,
        string additionalQuery = "",
        bool needCookie = false,
        bool needAppKey = false,
        bool forceNoToken = false,
        bool needRid = false,
        bool needCsrf = false)
    {
        FlurlRequest requestMessage;

        if (needCsrf && queryParams != null && !queryParams.ContainsKey("csrf"))
        {
            queryParams.Add("csrf", AuthorizeProvider.GetCsrfToken());
        }

        if (method == HttpMethod.Get && (needAppKey || needRid || !string.IsNullOrEmpty(additionalQuery)))
        {
            if (needRid)
            {
                var query = AuthorizeProvider.Instance.GenerateAuthorizedQueryStringFirstSign(queryParams);
                url += $"?{query}";
            }
            else if (needAppKey)
            {
                var query = AuthorizeProvider.GenerateAuthorizedQueryStringFirstSign(queryParams, clientType);
                url += $"?{query}";
            }

            if (!string.IsNullOrEmpty(additionalQuery))
            {
                url += $"&{additionalQuery}";
            }

            requestMessage = new FlurlRequest(url);
        }
        else if (method == HttpMethod.Get || method == HttpMethod.Delete)
        {
            var query = await AuthorizeProvider.Instance.GenerateAuthorizedQueryStringAsync(queryParams, clientType, needToken, forceNoToken, needRid);
            if (!string.IsNullOrEmpty(additionalQuery))
            {
                query += $"&{additionalQuery}";
            }

            url += $"?{query}";
            requestMessage = new FlurlRequest(url);
        }
        else
        {
            var query = await AuthorizeProvider.Instance.GenerateAuthorizedQueryDictionaryAsync(queryParams, clientType, needToken, forceNoToken, needRid);
            requestMessage = new FlurlRequest(url)
            {
                Content = new FormUrlEncodedContent(query),
            };
        }

        if (needCookie)
        {
            var cookies = AuthorizeProvider.GetCookieDict();
            if (cookies.Count > 0)
            {
                requestMessage.WithCookies(cookies);
            }
        }

        requestMessage.Verb = method;

        return requestMessage;
    }

    /// <summary>
    /// 从响应中获取 cookie.
    /// </summary>
    /// <param name="response">响应.</param>
    /// <returns>结果.</returns>
    public static Dictionary<string, string> GetCookieFromResponse(IFlurlResponse response)
    {
        var cookies = response.Cookies.Select(p => (p.Name, p.Value)).ToDictionary();
        return cookies;
    }

    /// <summary>
    /// 解析响应.
    /// </summary>
    /// <param name="response">得到的 <see cref="HttpResponseMessage"/>.</param>
    /// <typeparam name="T">需要转换的目标类型.</typeparam>
    /// <returns>转换结果.</returns>
    public static Task<T> ParseAsync<T>(IFlurlResponse response)
        => ParseAsync<T>(response.ResponseMessage);

    /// <summary>
    /// 解析响应.
    /// </summary>
    /// <param name="response">得到的 <see cref="HttpResponseMessage"/>.</param>
    /// <typeparam name="T">需要转换的目标类型.</typeparam>
    /// <returns>转换结果.</returns>
    public static async Task<T> ParseAsync<T>(HttpResponseMessage response)
    {
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(responseString);
    }

    /// <summary>
    /// 解析响应.
    /// </summary>
    /// <param name="response">得到的 <see cref="HttpResponseMessage"/>.</param>
    /// <param name="parser">对应gRPC类型的转换器.</param>
    /// <typeparam name="T">需要转换的gRPC目标类型.</typeparam>
    /// <returns>转换结果.</returns>
    public static async Task<T> ParseAsync<T>(HttpResponseMessage response, MessageParser<T> parser)
        where T : IMessage<T>
    {
        var bytes = await response.Content.ReadAsByteArrayAsync();
        return parser.ParseFrom(bytes.Skip(5).ToArray());
    }

    /// <summary>
    /// 解析响应，并选择满足条件的目标类型返回.
    /// </summary>
    /// <param name="response">得到的 <see cref="HttpResponseMessage"/>.</param>
    /// <param name="condition">条件.</param>
    /// <typeparam name="T1">需要转换的目标类型1.</typeparam>
    /// <typeparam name="T2">需要转换的目标类型2.</typeparam>
    /// <returns>转换结果.</returns>
    public static async Task<object> ParseAsync<T1, T2>(HttpResponseMessage response, Func<string, bool> condition)
    {
        var responseString = await response.Content.ReadAsStringAsync();
        return condition(responseString)
            ? JsonSerializer.Deserialize<T1>(responseString)
            : JsonSerializer.Deserialize<T2>(responseString);
    }

    /// <summary>
    /// 获取 <see cref="HttpRequestMessage"/>.
    /// </summary>
    /// <param name="url">请求地址.</param>
    /// <param name="grpcMessage">gRPC信息.</param>
    /// <param name="needToken">是否需要令牌.</param>
    /// <returns><see cref="HttpRequestMessage"/>.</returns>
    public static async Task<HttpRequestMessage> GetRequestMessageAsync(string url, IMessage grpcMessage, bool needToken = false)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
        var isTokenValid = await AuthorizeProvider.Instance.IsTokenValidAsync();
        var token = string.Empty;
        if (needToken || isTokenValid)
        {
            token = await AuthorizeProvider.Instance.GetTokenAsync();
        }

        var grpcConfig = new GRPCConfig(token);
        var userAgent = $"bili-inter/73300300 "
            + $"os/ios model/{GRPCConfig.Model} mobi_app/iphone_i "
            + $"osVer/{GRPCConfig.OSVersion} "
            + $"network/{GRPCConfig.NetworkType} "
            + $"grpc-objc/1.47.0 grpc-c/25.0.0 (ios; cronet_http)";

        if (!string.IsNullOrEmpty(token))
        {
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue(Headers.Identify, token);
            requestMessage.Headers.Add(Headers.BiliMid, AuthorizeProvider.Instance.CurrentUserId);
        }

        requestMessage.Headers.Add(Headers.UserAgent, userAgent);
        requestMessage.Headers.Add(Headers.AppKey, GRPCConfig.MobileApp);
        requestMessage.Headers.Add(Headers.BiliDevice, GRPCConfig.GetDeviceBin());
        requestMessage.Headers.Add(Headers.BiliFawkes, GRPCConfig.GetFawkesreqBin());
        requestMessage.Headers.Add(Headers.BiliLocale, GRPCConfig.GetLocaleBin());
        requestMessage.Headers.Add(Headers.BiliMeta, grpcConfig.GetMetadataBin());
        requestMessage.Headers.Add(Headers.BiliNetwork, GRPCConfig.GetNetworkBin());
        requestMessage.Headers.Add(Headers.BiliRestriction, GRPCConfig.GetRestrictionBin());
        requestMessage.Headers.Add(Headers.GRPCAcceptEncodingKey, Headers.GRPCAcceptEncodingValue);
        requestMessage.Headers.Add(Headers.GRPCTimeOutKey, Headers.GRPCTimeOutValue);
        requestMessage.Headers.Add(Headers.Envoriment, GRPCConfig.Envorienment);
        requestMessage.Headers.Add(Headers.TransferEncodingKey, Headers.TransferEncodingValue);
        requestMessage.Headers.Add(Headers.TEKey, Headers.TEValue);
        requestMessage.Headers.Add(Headers.AuroraEid, GRPCConfig.GetAuroraEid(string.IsNullOrEmpty(AuthorizeProvider.Instance.CurrentUserId) ? 0 : Convert.ToInt64(AuthorizeProvider.Instance.CurrentUserId)));
        requestMessage.Headers.Add(Headers.TraceId, GRPCConfig.GetTraceId());
        requestMessage.Headers.Add(Headers.Buvid, GetBuvid());

        var messageBytes = grpcMessage.ToByteArray();

        // 校验用?第五位为数组长度
        var stateBytes = new byte[] { 0, 0, 0, 0, (byte)messageBytes.Length };

        // 合并两个字节数组
        var bodyBytes = new byte[5 + messageBytes.Length];
        stateBytes.CopyTo(bodyBytes, 0);
        messageBytes.CopyTo(bodyBytes, 5);

        var byteArrayContent = new ByteArrayContent(bodyBytes);
        byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue(Headers.GRPCContentType);
        byteArrayContent.Headers.ContentLength = bodyBytes.Length;

        requestMessage.Content = byteArrayContent;
        return requestMessage;
    }

    /// <summary>
    /// 发送请求.
    /// </summary>
    /// <param name="request">需要发送的 <see cref="HttpRequestMessage"/>.</param>
    /// <returns>返回的 <see cref="HttpResponseMessage"/>.</returns>
    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        => SendAsync(request, CancellationToken.None);

    /// <summary>
    /// 发送请求.
    /// </summary>
    /// <param name="request">需要发送的 <see cref="IFlurlRequest"/>.</param>
    /// <returns>返回的 <see cref="IFlurlResponse"/>.</returns>
    public Task<IFlurlResponse> SendAsync(IFlurlRequest request)
        => SendAsync(request, CancellationToken.None);

    /// <summary>
    /// 发送请求.
    /// </summary>
    /// <param name="request">需要发送的 <see cref="HttpRequestMessage"/>.</param>
    /// <param name="cancellationToken">请求的 <see cref="CancellationToken"/>.</param>
    /// <returns>返回的 <see cref="HttpResponseMessage"/>.</returns>
    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpResponseMessage response;
        try
        {
            response = await SendRequestAsync(request, cancellationToken);
        }
        catch (Exception)
        {
            throw;
        }

        return response;
    }

    /// <summary>
    /// 发送请求.
    /// </summary>
    /// <param name="request">需要发送的 <see cref="IFlurlRequest"/>.</param>
    /// <param name="cancellationToken">请求的 <see cref="CancellationToken"/>.</param>
    /// <returns>返回的 <see cref="HttpResponseMessage"/>.</returns>
    public async Task<IFlurlResponse> SendAsync(IFlurlRequest request, CancellationToken cancellationToken)
    {
        IFlurlResponse response;
        try
        {
            response = await SendRequestAsync(request, cancellationToken);
        }
        catch (Exception)
        {
            throw;
        }

        return response;
    }
}
