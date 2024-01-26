// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Args;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.Models.BiliBili;
using Bili.Copilot.Models.BiliBili.Others;
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
    private readonly byte[] _mIXIN_KEY_ENC_TAB =
        [
            46, 47, 18, 2, 53, 8, 23, 32, 15, 50, 10, 31, 58, 3, 45, 35, 27, 43, 5, 49, 33, 9, 42,
            19, 29, 28, 14, 39, 12, 38, 41, 13, 37, 48, 7, 16, 24, 55, 40, 61, 26, 17, 0, 1, 60,
            51, 30, 4, 22, 25, 54, 21, 56, 59, 6, 63, 57, 62, 11, 36, 20, 34, 44, 52,
        ];
    private readonly string _guid;
    private AuthorizeState _state;
    private TokenInfo _tokenInfo;
    private DateTimeOffset _lastAuthorizeTime;
    private string _internalQRAuthCode;
    private DispatcherTimer _qrTimer;
    private CancellationTokenSource _qrPollCancellationTokenSource;

    private string _img;
    private string _sub;
    private string _wbi;

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

    internal string GenerateRid(Dictionary<string, string> queryParameters)
    {
        if (!queryParameters.ContainsKey("wts"))
        {
            queryParameters.Add("wts", DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
        }

        var queryList = queryParameters.Select(p => $"{p.Key}={p.Value}").ToList();
        queryList.Sort();
        var query = string.Join('&', queryList);
        var signQuery = query + _wbi;
        using var md5Toolkit = new MD5Toolkit();
        var rid = md5Toolkit.GetMd5String(signQuery).ToLower();
        return rid;
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

                if (result.Data.CookieInfo is not null)
                {
                    SaveCookie(result.Data.CookieInfo);
                }

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
            { Query.Guid, Guid.NewGuid().ToString() },
        };

        try
        {
            var httpProvider = HttpProvider.Instance;
            var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Post, Passport.QRCodeCheck, queryParameters, clientType: RequestClientType.Login, needToken: false);
            var response = await httpProvider.SendAsync(request, _qrPollCancellationTokenSource.Token);
            var result = await HttpProvider.ParseAsync<ServerResponse<TokenInfo>>(response);

            // 保存cookie
            SaveCookie(result.Data.CookieInfo);
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
                var qrStatus = se.Error.Code is 86090 or 86039
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

    private async Task<string> GetWbiKeyAsync()
    {
        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Passport.WebNav, needToken: false);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse<WebNavResponse>>(response);
        if (result?.Data?.Img is not null)
        {
            _img = Path.GetFileNameWithoutExtension(result.Data.Img.ImgUrl);
            _sub = Path.GetFileNameWithoutExtension(result.Data.Img.SubUrl);
            return _img + _sub;
        }

        return string.Empty;
    }

    private string GetMixinKey(string key)
    {
        var binding = new List<byte>();
        var rawbiKey = Encoding.UTF8.GetBytes(key);
        foreach (var b in _mIXIN_KEY_ENC_TAB)
        {
            binding.Add(rawbiKey[b]);
        }

        var mixinKey = Encoding.UTF8.GetString(binding.ToArray());
        return mixinKey.Substring(0, 32);
    }
}
