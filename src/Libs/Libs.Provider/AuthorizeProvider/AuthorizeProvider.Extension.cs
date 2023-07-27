// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Args;
using Bili.Copilot.Models.App.Constants;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.Models.BiliBili;
using Bili.Copilot.Models.Constants.Authorize;
using Microsoft.UI.Xaml;
using Windows.Storage;
using Windows.Web.Http.Filters;
using static Bili.Copilot.Models.App.Constants.ApiConstants;
using static Bili.Copilot.Models.App.Constants.ServiceConstants;

namespace Bili.Copilot.Libs.Provider;

/// <summary>
/// 授权模块的属性集及扩展.
/// </summary>
public partial class AuthorizeProvider
{
    private readonly string _guid;
    private AuthorizeState _state;
    private TokenInfo _tokenInfo;
    private DateTimeOffset _lastAuthorizeTime;
    private string _internalQRAuthCode;
    private DispatcherTimer _qrTimer;
    private CancellationTokenSource _qrPollCancellationTokenSource;

    /// <summary>
    /// 当授权状态改变时发生.
    /// </summary>
    public event EventHandler<AuthorizeStateChangedEventArgs> StateChanged;

    /// <summary>
    /// 在二维码状态发生改变时发生.
    /// </summary>
    public event EventHandler<Tuple<QRCodeStatus, TokenInfo>> QRCodeStatusChanged;

    /// <summary>
    /// 实例.
    /// </summary>
    public static AuthorizeProvider Instance { get; } = new AuthorizeProvider();

    /// <summary>
    /// 当前的授权状态.
    /// </summary>
    public AuthorizeState State
    {
        get => _state;
        private set
        {
            var oldState = _state;
            var newState = value;
            if (oldState != newState)
            {
                _state = newState;
                StateChanged?.Invoke(this, new AuthorizeStateChangedEventArgs(oldState, newState));
            }
        }
    }

    /// <summary>
    /// 当前已登录的用户Id.
    /// </summary>
    public string CurrentUserId { get; private set; }

    internal static void GenerateAppKey(Dictionary<string, string> queryParameters, RequestClientType clientType, bool onlyAppKey = false)
    {
        if (clientType == RequestClientType.IOS)
        {
            queryParameters.Add(Query.AppKey, Keys.IOSKey);
            if (!onlyAppKey)
            {
                queryParameters.Add(Query.MobileApp, "iphone");
                queryParameters.Add(Query.Platform, "ios");
                queryParameters.Add(Query.TimeStamp, GetNowSeconds().ToString());
            }
        }
        else if (clientType == RequestClientType.Web)
        {
            queryParameters.Add(Query.AppKey, Keys.WebKey);
            if (!onlyAppKey)
            {
                queryParameters.Add(Query.Platform, "web");
                queryParameters.Add(Query.TimeStamp, GetNowMilliSeconds().ToString());
            }
        }
        else if (clientType == RequestClientType.Login)
        {
            queryParameters.Add(Query.AppKey, Keys.LoginKey);
            if (!onlyAppKey)
            {
                queryParameters.Add(Query.TimeStamp, GetNowMilliSeconds().ToString());
            }
        }
        else
        {
            queryParameters.Add(Query.AppKey, Keys.AndroidKey);
            if (!onlyAppKey)
            {
                queryParameters.Add(Query.MobileApp, "android");
                queryParameters.Add(Query.Platform, "android");
                queryParameters.Add(Query.TimeStamp, GetNowSeconds().ToString());
            }
        }
    }

    internal static string GenerateSign(Dictionary<string, string> queryParameters, RequestClientType clientType)
    {
        var queryList = queryParameters.Select(p => $"{p.Key}={p.Value}").ToList();
        queryList.Sort();

        var apiSecret = string.Empty;

        apiSecret = clientType switch
        {
            RequestClientType.IOS => Keys.IOSSecret,
            RequestClientType.Android => Keys.AndroidSecret,
            RequestClientType.Login => Keys.LoginSecret,
            _ => Keys.WebSecret,
        };
        var query = string.Join('&', queryList);
        var signQuery = query + apiSecret;
        using var md5Toolkit = new MD5Toolkit();
        var sign = md5Toolkit.GetMd5String(signQuery).ToLower();
        return sign;
    }

    internal async Task<TokenInfo> InternalRefreshTokenAsync()
    {
        try
        {
            if (!string.IsNullOrEmpty(_tokenInfo?.RefreshToken))
            {
                var queryParameters = new Dictionary<string, string>
                {
                    { Query.AccessToken, _tokenInfo.AccessToken },
                    { Query.RefreshToken, _tokenInfo.RefreshToken },
                };

                var httpProvider = HttpProvider.Instance;
                var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Post, Passport.RefreshToken, queryParameters);
                var response = await httpProvider.SendAsync(request);
                var result = await HttpProvider.ParseAsync<ServerResponse<TokenInfo>>(response);
                await SSOInitAsync();

                return result.Data;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }

        return null;
    }

    private static long GetNowSeconds() => DateTimeOffset.Now.ToLocalTime().ToUnixTimeSeconds();

    private static long GetNowMilliSeconds() => DateTimeOffset.Now.ToLocalTime().ToUnixTimeMilliseconds();

    private static void SaveCookie(CookieInfo cookieInfo)
    {
        var domain = CookieSetDomain;

        if (cookieInfo != null && cookieInfo.Cookies != null)
        {
            var filter = new HttpBaseProtocolFilter();
            foreach (var cookieItem in cookieInfo.Cookies)
            {
                _ = filter.CookieManager.SetCookie(new Windows.Web.Http.HttpCookie(cookieItem.Name, domain, "/")
                {
                    HttpOnly = cookieItem.HttpOnly == 1,
                    Secure = cookieItem.Secure == 1,
                    Expires = DateTimeOffset.FromUnixTimeSeconds(cookieItem.Expires),
                    Value = cookieItem.Value,
                });
            }
        }
    }

    private static async Task SSOInitAsync()
    {
        var url = Passport.SSO;
        var httpProvider = HttpProvider.Instance;
        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, url, needToken: false);
        _ = await httpProvider.SendAsync(request);
    }

    private static async Task<string> GetCookieToAccessKeyConfirmUrlAsync()
    {
        var httpProvider = HttpProvider.Instance;
        var query = new Dictionary<string, string>
            {
                { "api", ApiConstants.Passport.LoginAppThirdApi },
            };
        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Passport.LoginAppThird, query, RequestClientType.IOS, needCookie: true, needAppKey: true);
        var response = await httpProvider.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse<LoginAppThird>>(response);
        return result.Data.ConfirmUri;
    }

    private static async Task<string> GetAccessKeyAsync(string confirmUri)
    {
        try
        {
            var httpProvider = HttpProvider.Instance;
            var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, confirmUri, needCookie: true, needToken: false);
            var response = await httpProvider.SendAsync(request);
            var success = response.Headers.TryGetValues("location", out var locations);
            if (!success)
            {
                return default;
            }

            var redirectUrl = locations.FirstOrDefault();
            var uri = new Uri(redirectUrl);
            var queries = HttpUtility.ParseQueryString(uri.Query);
            var accessKey = queries.Get("access_key");
            return accessKey;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            throw;
        }
    }

    private async void OnQRTimerTickAsync(object sender, object e)
    {
        if (await IsTokenValidAsync())
        {
            StopQRLoginListener();
            return;
        }

        CleanQRCodeCancellationToken();
        _qrPollCancellationTokenSource = new CancellationTokenSource();
        var queryParameters = new Dictionary<string, string>
        {
            { Query.AuthCode, _internalQRAuthCode },
            { Query.LocalId, _guid },
        };

        try
        {
            var httpProvider = HttpProvider.Instance;
            var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Post, Passport.QRCodeCheck, queryParameters, needToken: false);
            var response = await httpProvider.SendAsync(request, _qrPollCancellationTokenSource.Token);
            var result = await HttpProvider.ParseAsync<ServerResponse<TokenInfo>>(response);

            // 保存cookie
            SaveCookie(result.Data.CookieInfo);

            // 获取确认链接
            var confirmUrl = await GetCookieToAccessKeyConfirmUrlAsync();

            // 获取新的访问令牌
            var accessKey = await GetAccessKeyAsync(confirmUrl);
            result.Data.AccessToken = accessKey;
            SaveAuthorizeResult(result.Data);
            QRCodeStatusChanged?.Invoke(this, new Tuple<QRCodeStatus, TokenInfo>(QRCodeStatus.Success, result.Data));
        }
        catch (ServiceException se)
        {
            if (se.InnerException is TaskCanceledException)
            {
                return;
            }

            if (se.Error != null)
            {
                var qrStatus = se.Error.Code == 86039
                    ? QRCodeStatus.NotConfirm
                    : se.Error.Code is 86038 or -3 ? QRCodeStatus.Expired : QRCodeStatus.Failed;

                QRCodeStatusChanged?.Invoke(this, new Tuple<QRCodeStatus, TokenInfo>(qrStatus, null));
            }
        }
    }

    private void CleanQRCodeCancellationToken()
    {
        if (_qrPollCancellationTokenSource != null)
        {
            if (_qrPollCancellationTokenSource.Token.CanBeCanceled)
            {
                _qrPollCancellationTokenSource.Cancel();
            }

            _qrPollCancellationTokenSource.Dispose();
            _qrPollCancellationTokenSource = null;
        }
    }

    private async Task<bool> NetworkVerifyTokenAsync()
    {
        if (!string.IsNullOrEmpty(_tokenInfo?.AccessToken))
        {
            var queryParameters = new Dictionary<string, string>
            {
                { Query.AccessToken, _tokenInfo.AccessToken },
            };

            try
            {
                var httpProvider = HttpProvider.Instance;
                var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Passport.CheckToken, queryParameters);
                _ = await httpProvider.SendAsync(request);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        return false;
    }

    private void SaveAuthorizeResult(TokenInfo result)
    {
        if (result != null)
        {
            var saveTime = DateTimeOffset.Now;
            var compositeValue = new ApplicationDataCompositeValue
            {
                [Settings.AccessTokenKey] = result.AccessToken,
                [Settings.RefreshTokenKey] = result.RefreshToken ?? string.Empty,
                [Settings.UserIdKey] = result.Mid,
                [Settings.ExpiresInKey] = result.ExpiresIn,
                [Settings.LastSaveAuthTimeKey] = saveTime.ToUnixTimeSeconds(),
            };

            CurrentUserId = result.Mid.ToString();
            ApplicationData.Current.LocalSettings.Values[Settings.AuthResultKey] = compositeValue;
            _lastAuthorizeTime = saveTime;
            _tokenInfo = result;
            State = AuthorizeState.SignedIn;
        }
    }

    private void RetrieveAuthorizeResult()
    {
        var localSettings = ApplicationData.Current.LocalSettings;
        if (localSettings.Values.ContainsKey(Settings.AuthResultKey))
        {
            var data = (ApplicationDataCompositeValue)localSettings.Values[Settings.AuthResultKey];
            var tokenInfo = new TokenInfo
            {
                AccessToken = data[Settings.AccessTokenKey].ToString(),
                RefreshToken = data[Settings.RefreshTokenKey].ToString(),
                Mid = Convert.ToInt64(data[Settings.UserIdKey]),
                ExpiresIn = (int)data[Settings.ExpiresInKey],
            };

            CurrentUserId = tokenInfo.Mid.ToString();
            _tokenInfo = tokenInfo;
            _lastAuthorizeTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(data[Settings.LastSaveAuthTimeKey]));
            State = AuthorizeState.SignedIn;
        }
        else
        {
            _tokenInfo = null;
            _lastAuthorizeTime = default;
        }
    }
}
