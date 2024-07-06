// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;
using System.Text.RegularExpressions;
using Flurl.Http;
using Richasy.BiliKernel.Bili;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.Net.Http;
using Richasy.BiliKernel.Bili.Authorization;
using System.Text.Json;
using Richasy.BiliKernel.Content;
using System.IO;

namespace Richasy.BiliKernel.Authenticator;

/// <summary>
/// 基础授权器.
/// </summary>
public sealed partial class BasicAuthenticator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BasicAuthenticator"/> class.
    /// </summary>
    public BasicAuthenticator(
        IBiliCookiesResolver? localCookiesResolver,
        IBiliTokenResolver? localTokenResolver)
    {
        _cookieResolver = localCookiesResolver;
        _tokenResolver = localTokenResolver;
    }

    /// <summary>
    /// 对 REST Api 请求进行授权.
    /// </summary>
    /// <param name="request">请求.</param>
    /// <param name="parameters">参数.</param>
    /// <param name="settings">请求设置.</param>
    public void AuthroizeRestRequest(IFlurlRequest request, Dictionary<string, string>? parameters = default, BasicAuthorizeExecutionSettings? settings = default)
    {
        Verify.NotNull(request, nameof(request));
        var executionSettings = settings ?? new BasicAuthorizeExecutionSettings();
        if (executionSettings.UseCookie && _cookieResolver != null)
        {
            var cookies = _cookieResolver.GetCookies();
            request.WithCookies(cookies);
        }

        if (request.Verb == HttpMethod.Post)
        {
            var queryParameters = AuthorizeRequestParameters(parameters, executionSettings);
            request.Content = new FormUrlEncodedContent(queryParameters);
        }
        else
        {
            request.Url.Query = GenerateAuthorizedQueryString(parameters, executionSettings);
        }
    }

    /// <summary>
    /// 对 gRPC 请求进行授权.
    /// </summary>
    public void AuthorizeGrpcRequest(IFlurlRequest request, bool needToken = true)
    {
        Verify.NotNull(request);
        var token = needToken ? _tokenResolver?.GetToken() : null;
        var accessToken = token?.AccessToken ?? string.Empty;
        var grpcConfig = new GrpcConfig(accessToken);
        var userAgent = $"bili-universal/80200100 "
            + $"os/ios model/{GrpcConfig.Model} mobi_app/iphone_i "
            + $"osVer/{GrpcConfig.OSVersion} "
            + $"network/{GrpcConfig.NetworkType} "
            + $"grpc-objc/1.47.0 grpc-c/25.0.0 (ios; cronet_http)";

        if (!string.IsNullOrEmpty(accessToken))
        {
            request.WithHeader("authorization", $"identify_v1 {accessToken}");
            request.WithHeader("x-bili-mid", token.UserId.ToString());
        }

        request.WithHeader("user-agent", userAgent);
        request.WithHeader("x-bili-device-bin", GrpcConfig.GetDeviceBin());
        request.WithHeader("x-bili-fawkes-req-bin", GrpcConfig.GetFawkesreqBin());
        request.WithHeader("x-bili-locale-bin", GrpcConfig.GetLocaleBin());
        request.WithHeader("x-bili-metadata-bin", grpcConfig.GetMetadataBin());
        request.WithHeader("x-bili-network-bin", GrpcConfig.GetNetworkBin());
        request.WithHeader("x-bili-restriction-bin", GrpcConfig.GetRestrictionBin());
        request.WithHeader("grpc-accept-encoding", "identity,deflate,gzip");
        request.WithHeader("grpc-timeout", "20100m");
        request.WithHeader("env", GrpcConfig.Envorienment);
        request.WithHeader("Transfer-Encoding", "chunked");
        request.WithHeader("TE", "trailers");
        request.WithHeader("x-bili-aurora-eid", GrpcConfig.GetAuroraEid(token?.UserId ?? 0));
        request.WithHeader("x-bili-trace-id", GrpcConfig.GetTraceId());
        request.WithHeader("buvid", GetBuvid());
    }

    /// <summary>
    /// 生成授权后的请求参数.
    /// </summary>
    /// <param name="parameters">基础参数.</param>
    /// <param name="settings">授权设置.</param>
    /// <returns>处理后的参数列表.</returns>
    /// <exception cref="KernelException">令牌获取失败.</exception>
    public Dictionary<string, string> AuthorizeRequestParameters(Dictionary<string, string>? parameters, BasicAuthorizeExecutionSettings? settings = default)
    {
        var queryParameters = parameters ?? new Dictionary<string, string>();
        var executionSettings = settings ?? new BasicAuthorizeExecutionSettings();
        if (executionSettings.NeedCSRF && !queryParameters.ContainsKey("csrf"))
        {
            queryParameters.Add("csrf", GetCsrfToken());
        }

        if (!queryParameters.ContainsKey("build"))
        {
            queryParameters.Add("build", BuildNumber);
        }

        InitializeDeviceParameters(queryParameters, executionSettings.ApiType, executionSettings.OnlyUseAppKey);

        if (!executionSettings.ForceNoToken)
        {
            var token = _tokenResolver?.GetToken();
            if (token == null && executionSettings.UseToken)
            {
                throw new KernelException("需要令牌，但令牌不存在，请重新登录");
            }

            var accessToken = token?.AccessToken ?? string.Empty;
            if (!string.IsNullOrEmpty(accessToken))
            {
                queryParameters.Add("access_key", accessToken);
            }
        }

        if (executionSettings.NeedRID)
        {
            var rid = GenerateRID(queryParameters);
            queryParameters.Add("w_rid", rid);
        }
        else
        {
            var sign = GenerateSign(queryParameters, executionSettings.ApiType);
            queryParameters.Add("sign", sign);
        }

        return queryParameters;
    }

    /// <summary>
    /// 生成授权后的查询字符串.
    /// </summary>
    /// <param name="parameters">基础参数.</param>
    /// <param name="settings">授权设置.</param>
    /// <returns>字符串.</returns>
    public string GenerateAuthorizedQueryString(Dictionary<string, string>? parameters, BasicAuthorizeExecutionSettings? settings = default)
    {
        var queryParameters = AuthorizeRequestParameters(parameters, settings);
        return GenerateQuery(queryParameters);
    }

    /// <summary>
    /// 初始化 Wbi.
    /// </summary>
    public async Task InitializeWbiAsync(CancellationToken cancellationToken = default)
    {
        using var client = new FlurlClient();
        var request = new FlurlRequest(BiliApis.Passport.WebNav);
        request.Verb = System.Net.Http.HttpMethod.Get;
        if (_cookieResolver != null)
        {
            request.WithCookies(_cookieResolver.GetCookies());
        }

        var response = await client.SendAsync(request, cancellationToken: cancellationToken).ConfigureAwait(false);
        var responseText = await response.ResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        var result = JsonSerializer.Deserialize<BiliDataResponse<WebNavResponse>>(responseText);
        var img = result?.Data?.Img?.ImgUrl ?? string.Empty;
        var sub = result?.Data?.Img?.SubUrl ?? string.Empty;
        _img = Path.GetFileNameWithoutExtension(img);
        _sub = Path.GetFileNameWithoutExtension(sub);
        _wbi = GenerateWbi(_img + _sub);

        string GenerateWbi(string key)
        {
            var binding = new List<byte>();
            var rawbiKey = Encoding.UTF8.GetBytes(key);
            foreach (var b in MIXIN_KEY_ENC_TAB)
            {
                binding.Add(rawbiKey[b]);
            }

            var mixinKey = Encoding.UTF8.GetString(binding.ToArray());
            return mixinKey.Substring(0, 32);
        }
    }

    private string GetCsrfToken()
    {
        if (_cookieResolver == null)
        {
            return string.Empty;
        }

        var cookie = _cookieResolver.GetCookieString();
        var csrfToken = string.Empty;
        var match = Regex.Match(cookie, @"bili_jct=(.*?);");
        if (match.Success)
        {
            csrfToken = match.Groups[1].Value;
        }

        return csrfToken;
    }
}
