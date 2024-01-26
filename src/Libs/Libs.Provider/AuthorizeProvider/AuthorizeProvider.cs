// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Constants;
using Bili.Copilot.Models.BiliBili;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Authorize;
using Microsoft.UI.Xaml;
using QRCoder;
using Windows.Web.Http.Filters;
using static Bili.Copilot.Models.App.Constants.ApiConstants;
using static Bili.Copilot.Models.App.Constants.ServiceConstants;

namespace Bili.Copilot.Libs.Provider;

/// <summary>
/// 授权模块.
/// </summary>
public sealed partial class AuthorizeProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizeProvider"/> class.
    /// </summary>
    private AuthorizeProvider()
    {
        State = AuthorizeState.SignedOut;
        RetrieveAuthorizeResult();
        _guid = Guid.NewGuid().ToString("N");
    }

    /// <summary>
    /// 获取Cookie字符串.
    /// </summary>
    /// <returns>Cookie字符串.</returns>
    public static string GetCookieString()
    {
        var fiter = new HttpBaseProtocolFilter();
        var cookies = fiter.CookieManager.GetCookies(new Uri(CookieGetDomain));
        var cookieList = cookies.Select(x =>
        {
            return $"{x.Name}={x.Value}";
        });
        var result = string.Join(';', cookieList);
        return result;
    }

    /// <summary>
    /// 获取CSRF Token.
    /// </summary>
    /// <returns>CSRF token.</returns>
    public static string GetCsrfToken()
    {
        var cookie = GetCookieString();
        var csrfToken = string.Empty;
        var pattern = @"bili_jct=(.*?);";
        var match = Regex.Match(cookie, pattern);
        if (match.Success)
        {
            csrfToken = match.Groups[1].Value;
        }

        return csrfToken;
    }

    /// <summary>
    /// 获取包含授权码的查询字典，无token且先加盐.
    /// </summary>
    /// <param name="queryParameters">请求所需的查询参数.</param>
    /// <param name="clientType">请求需要模拟的客户端类型.</param>
    /// <returns>包含授权验证码的查询字典.</returns>
    public static string GenerateAuthorizedQueryStringFirstSign(
        Dictionary<string, string> queryParameters,
        RequestClientType clientType)
    {
        queryParameters ??= new Dictionary<string, string>();

        // 先加盐再加appKey
        var sign = GenerateSign(queryParameters, clientType);
        queryParameters.Add(ServiceConstants.Query.Sign, sign);
        GenerateAppKey(queryParameters, clientType, true);
        var queryList = queryParameters.Select(p => $"{p.Key}={p.Value}").ToList();
        queryList.Sort();
        var query = string.Join('&', queryList);
        return query;
    }

    /// <summary>
    /// 获取包含授权码的查询字典.
    /// </summary>
    /// <param name="queryParameters">请求所需的查询参数.</param>
    /// <returns>包含授权验证码的查询字典.</returns>
    public string GenerateAuthorizedQueryStringFirstSign(
        Dictionary<string, string> queryParameters)
    {
        queryParameters ??= new Dictionary<string, string>();
        var rid = GenerateRid(queryParameters);
        queryParameters.Add("w_rid", rid);
        var queryList = queryParameters.Select(p => $"{p.Key}={p.Value}").ToList();
        queryList.Sort();
        var query = string.Join('&', queryList);
        return query;
    }

    /// <summary>
    /// 获取包含授权码的查询字典.
    /// </summary>
    /// <param name="queryParameters">请求所需的查询参数.</param>
    /// <param name="clientType">请求需要模拟的客户端类型.</param>
    /// <param name="needToken">是否需要访问令牌.</param>
    /// <param name="forceNoToken">是否强制不需要令牌.</param>
    /// <param name="needRid">是否需要 RID 签名.</param>
    /// <returns>包含授权验证码的查询字典.</returns>
    public async Task<Dictionary<string, string>> GenerateAuthorizedQueryDictionaryAsync(
        Dictionary<string, string> queryParameters,
        RequestClientType clientType,
        bool needToken = false,
        bool forceNoToken = false,
        bool needRid = false)
    {
        queryParameters ??= new Dictionary<string, string>();

        queryParameters.Add(ServiceConstants.Query.Build, ServiceConstants.BuildNumber);
        GenerateAppKey(queryParameters, clientType);

        var token = string.Empty;
        if (await IsTokenValidAsync() && !forceNoToken)
        {
            token = _tokenInfo.AccessToken;
        }
        else if (needToken)
        {
            token = await GetTokenAsync();
        }

        if (!string.IsNullOrEmpty(token))
        {
            queryParameters.Add(ServiceConstants.Query.AccessKey, token);
        }
        else if (needToken)
        {
            throw new OperationCanceledException("需要令牌，但获取访问令牌失败.");
        }

        if (needRid)
        {
            var rid = GenerateRid(queryParameters);
            queryParameters.Add("w_rid", rid);
        }
        else
        {
            var sign = GenerateSign(queryParameters, clientType);
            queryParameters.Add(ServiceConstants.Query.Sign, sign);
        }

        return queryParameters;
    }

    /// <summary>
    /// 获取包含授权码的查询字符串.
    /// </summary>
    /// <param name="queryParameters">请求所需的查询参数.</param>
    /// <param name="clientType">请求需要模拟的客户端类型.</param>
    /// <param name="needToken">是否需要令牌.</param>
    /// <param name="forceNoToken">是否强制不需要令牌.</param>
    /// <param name="needRid">是否需要 RID.</param>
    /// <returns>包含授权验证的查询字符串.</returns>
    public async Task<string> GenerateAuthorizedQueryStringAsync(
        Dictionary<string, string> queryParameters,
        RequestClientType clientType,
        bool needToken = true,
        bool forceNoToken = false,
        bool needRid = false)
    {
        var parameters = await GenerateAuthorizedQueryDictionaryAsync(queryParameters, clientType, needToken, forceNoToken, needRid);
        var queryList = parameters.Select(p => $"{p.Key}={p.Value}").ToList();
        queryList.Sort();
        var query = string.Join('&', queryList);
        return query;
    }

    /// <summary>
    /// 获取当前登录用户的访问令牌.
    /// </summary>
    /// <returns>账户授权的令牌.</returns>
    public async Task<string> GetTokenAsync()
    {
        try
        {
            if (_tokenInfo != null)
            {
                if (await IsTokenValidAsync())
                {
                    State = AuthorizeState.SignedIn;
                    return _tokenInfo.AccessToken;
                }
                else
                {
                    var tokenInfo = await InternalRefreshTokenAsync();
                    if (tokenInfo != null)
                    {
                        SaveAuthorizeResult(tokenInfo);
                        return tokenInfo.AccessToken;
                    }
                }
            }
            else
            {
                SignOut();
            }
        }
        catch (Exception)
        {
            StopQRLoginListener();
            SignOut();
            throw;
        }

        return default;
    }

    /// <summary>
    /// 获取登录二维码图片.
    /// </summary>
    /// <returns>图片类型.</returns>
    public async Task<Stream> GetQRImageAsync()
    {
        try
        {
            StopQRLoginListener();
            var queryParameters = new Dictionary<string, string>
            {
                { Query.LocalId, _guid },
            };
            var httpProvider = HttpProvider.Instance;
            var request = await HttpProvider.GetRequestMessageAsync(System.Net.Http.HttpMethod.Post, Passport.QRCode, queryParameters, clientType: RequestClientType.Login, needToken: false);
            var response = await httpProvider.SendAsync(request);
            var result = await HttpProvider.ParseAsync<ServerResponse<QRInfo>>(response);

            _internalQRAuthCode = result.Data.AuthCode;
            var generator = new QRCodeGenerator();
            var data = generator.CreateQrCode(result.Data.Url, QRCodeGenerator.ECCLevel.Q);
            var code = new QRCode(data);
            var image = code.GetGraphic(20);
            var ms = new MemoryStream();
            image.Save(ms, ImageFormat.Png);
            _ = ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// 开始本地轮询二维码状态.
    /// </summary>
    public void StartQRLoginListener()
    {
        if (_qrTimer == null)
        {
            _qrTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3),
            };
            _qrTimer.Tick += OnQRTimerTickAsync;
        }

        _qrTimer?.Start();
    }

    /// <summary>
    /// 停止本地轮询二维码状态.
    /// </summary>
    public void StopQRLoginListener()
    {
        _qrTimer?.Stop();
        _qrTimer = null;
        CleanQRCodeCancellationToken();
    }

    /// <summary>
    /// 用户登录.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public async Task<bool> TrySignInAsync()
    {
        await ResetWbiAsync();
        if (await IsTokenValidAsync() || State != AuthorizeState.SignedOut)
        {
            return true;
        }

        State = AuthorizeState.Loading;

        var token = await GetTokenAsync();

        if (string.IsNullOrEmpty(token))
        {
            SignOut();
            return false;
        }

        return true;
    }

    /// <summary>
    /// 用户退出.
    /// </summary>
    public void SignOut()
    {
        State = AuthorizeState.Loading;

        SettingsToolkit.DeleteLocalSetting(SettingNames.BiliUserId);
        SettingsToolkit.DeleteLocalSetting(SettingNames.AuthorizeResult);

        if (_tokenInfo != null)
        {
            _tokenInfo = null;
        }

        State = AuthorizeState.SignedOut;
        CurrentUserId = default;
        AccountProvider.Instance.UserId = 0;
    }

    /// <summary>
    /// 当前的访问令牌是否有效.
    /// </summary>
    /// <param name="isNetworkVerify">是否需要联网验证.</param>
    /// <returns>有效为<c>true</c>，无效为<c>false</c>.</returns>
    public async Task<bool> IsTokenValidAsync(bool isNetworkVerify = false)
    {
        var isLocalValid = _tokenInfo != null &&
            !string.IsNullOrEmpty(_tokenInfo.AccessToken) &&
            _lastAuthorizeTime != DateTimeOffset.MinValue &&
            (DateTimeOffset.Now - _lastAuthorizeTime).TotalSeconds < _tokenInfo.ExpiresIn;

        var result = isLocalValid && isNetworkVerify
            ? await NetworkVerifyTokenAsync()
            : isLocalValid;

        return result;
    }

    /// <summary>
    /// 重置 WBI 签名.
    /// </summary>
    /// <returns><see cref="Task"/>.</returns>
    public async Task ResetWbiAsync()
    {
        var key = await GetWbiKeyAsync();
        if (string.IsNullOrEmpty(key))
        {
            return;
        }

        _wbi = GetMixinKey(key);
    }
}
