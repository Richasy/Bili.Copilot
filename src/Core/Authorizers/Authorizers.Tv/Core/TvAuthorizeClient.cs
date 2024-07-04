// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Richasy.BiliKernel.Authenticator;
using Richasy.BiliKernel.Bili;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Content;
using Richasy.BiliKernel.Http;
using Richasy.BiliKernel.Models.Authorization;

namespace Richasy.BiliKernel.Authorizers.TV.Core;

internal sealed class TVAuthorizeClient
{
    private readonly string _localId;
    private readonly BiliHttpClient _httpClient;
    private readonly IBiliCookiesResolver _localCookiesResolver;
    private readonly IBiliTokenResolver _localTokenResolver;
    private readonly BasicAuthenticator _basicAuthenticator;
    private bool _isScanCheckInvoking;

    public TVAuthorizeClient(
        BiliHttpClient httpClient,
        IBiliCookiesResolver localCookiesResolver,
        IBiliTokenResolver localTokenResolver,
        BasicAuthenticator basicAuthenticator)
    {
        _localId = Guid.NewGuid().ToString("N");
        _httpClient = httpClient;
        _localCookiesResolver = localCookiesResolver;
        _localTokenResolver = localTokenResolver;
        _basicAuthenticator = basicAuthenticator;
    }

    public async Task EnsureTokenValidAsync(CancellationToken cancellationToken = default)
    {
        var localToken = _localTokenResolver.GetToken();
        BasicTokenCheck(localToken);
        var expiredTime = DateTimeOffset.FromUnixTimeSeconds(localToken.ExpiresIn);
        if (DateTimeOffset.Now < expiredTime)
        {
            return;
        }

        var newToken = await RefreshTokenAsync(cancellationToken).ConfigureAwait(false);
        if (newToken != null)
        {
            SaveToken(newToken);
            return;
        }

        throw new KernelException("令牌已过期，请重新登录");
    }

    public async Task<TVQRCode> GetQRCodeAsync(CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "local_id", _localId },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Post, new Uri(BiliApis.Passport.QRCode));
        _basicAuthenticator.AuthroizeRequest(request, parameters, new BasicAuthorizeExecutionSettings() { ForceNoToken = true });
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync<BiliDataResponse<TVQRCode>>(response).ConfigureAwait(false);
        return responseObj.Data;
    }

    public async Task<BiliToken?> WaitQRCodeScanAsync(TVQRCode code, CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (_isScanCheckInvoking)
            {
                await Task.Delay(500, cancellationToken).ConfigureAwait(false);
                continue;
            }

            _isScanCheckInvoking = true;
            try
            {
                var token = await GetTokenIfCodeScannedAsync(code.AuthCode, cancellationToken).ConfigureAwait(false);
                TrySaveCookiesFromToken(token);
                SaveToken(token);
                return token;
            }
            catch (Exception ex)
            {
                if (ex is KernelException ke && ke.InnerException is TaskCanceledException)
                {
                    break;
                }

                if (ex is HttpOperationException he && he.InnerException is not null && he.InnerException.Message.Contains("Code"))
                {
                    var error = JsonSerializer.Deserialize<BiliResponse>(he.InnerException.Message);
                    if (error != null)
                    {
                        var qrStatus = error.Code is 86090 or 86039
                            ? QRCodeStatus.NotConfirm
                            : error.Code is 86038 or -3 ? QRCodeStatus.Expired : QRCodeStatus.Failed;

                        if (qrStatus == QRCodeStatus.NotConfirm)
                        {
                            await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
                        }
                        else
                        {
                            throw new Exception(qrStatus == QRCodeStatus.Expired ? "二维码已过期，请刷新二维码" : "二维码扫描失败", he.InnerException);
                        }
                    }
                }
            }
            finally
            {
                _isScanCheckInvoking = false;
            }
        }

        return default;
    }

    public void SignOut()
    {
        _localTokenResolver.RemoveToken();
        _localCookiesResolver.RemoveCookies();
    }

    private static void BasicTokenCheck(BiliToken? localToken)
    {
        if (localToken == null)
        {
            throw new InvalidOperationException("没有本地缓存的令牌信息，请重新登录");
        }

        if (string.IsNullOrEmpty(localToken.AccessToken)
            || string.IsNullOrEmpty(localToken.RefreshToken)
            || localToken.ExpiresIn < 1)
        {
            throw new KernelException("令牌信息不完整，请重新登录");
        }
    }

    private async Task<BiliToken> RefreshTokenAsync(CancellationToken cancellationToken = default)
    {
        var localToken = _localTokenResolver.GetToken();
        BasicTokenCheck(localToken);

        var paramters = new Dictionary<string, string>
        {
            { "access_token", localToken.AccessToken },
            { "refresh_token", localToken.RefreshToken },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Post, new Uri(BiliApis.Passport.RefreshToken));
        _basicAuthenticator.AuthroizeRequest(request, paramters);
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync<BiliDataResponse<BiliToken>>(response).ConfigureAwait(false);
        var token = responseObj.Data;
        TrySaveCookiesFromToken(token);

        return token;
    }

    private async Task<BiliToken> GetTokenIfCodeScannedAsync(string authCode, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            { "auth_code", authCode },
            { "local_id", _localId },
            { "guid", Guid.NewGuid().ToString() },
        };

        var request = BiliHttpClient.CreateRequest(HttpMethod.Post, new Uri(BiliApis.Passport.QRCodeCheck));
        _basicAuthenticator.AuthroizeRequest(request, parameters, new BasicAuthorizeExecutionSettings() { ForceNoToken = true });
        var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseObj = await BiliHttpClient.ParseAsync<BiliDataResponse<BiliToken>>(response).ConfigureAwait(false);
        var token = responseObj?.Data;
        TrySaveCookiesFromToken(token);
        return token;
    }

    private void TrySaveCookiesFromToken(BiliToken? token)
    {
        if (token?.CookieInfo is null)
        {
            return;
        }

        var cookies = token.CookieInfo.Cookies.ToDictionary(cookie => cookie.Name!, cookie => cookie.Value ?? string.Empty);
        _localCookiesResolver.SaveCookies(cookies);
    }

    private void SaveToken(BiliToken token)
    {
        token.ExpiresIn = DateTimeOffset.Now.ToLocalTime().ToUnixTimeSeconds() + token.ExpiresIn;
        _localTokenResolver.SaveToken(token);
    }
}
