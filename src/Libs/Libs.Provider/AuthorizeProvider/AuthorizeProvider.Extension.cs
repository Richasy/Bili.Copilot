// Copyright (c) Bili Copilot. All rights reserved.
#define FIRST_RUN_

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.App.Args;
using Bili.Copilot.Models.App.Other;
using Bili.Copilot.Models.BiliBili;
using Bili.Copilot.Models.BiliBili.Others;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Authorize;
using HtmlAgilityPack;
using Microsoft.UI.Xaml;
using Windows.Storage;
using static Bili.Copilot.Models.App.Constants.ApiConstants;
using static Bili.Copilot.Models.App.Constants.ServiceConstants;

namespace Bili.Copilot.Libs.Provider;

/// <summary>
/// 授权模块的属性集及扩展.
/// </summary>
public partial class AuthorizeProvider
{
    private static readonly Lazy<AuthorizeProvider> _lazyInstance = new(() => new AuthorizeProvider());
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
    /// 在二维码扫描失败时发生.
    /// </summary>
    public event EventHandler<Exception> QRCodeScanFailed;

    /// <summary>
    /// 实例.
    /// </summary>
    public static AuthorizeProvider Instance => _lazyInstance.Value;

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

    /// <summary>
    /// 保存Cookie.
    /// </summary>
    /// <param name="cookies">Cookie 信息.</param>
    public static void SaveCookies(Dictionary<string, string> cookies)
    {
        var localCookies = GetCookieDict();
        foreach (var cookie in cookies)
        {
            if (localCookies.ContainsKey(cookie.Key))
            {
                localCookies[cookie.Key] = cookie.Value;
            }
            else
            {
                localCookies.Add(cookie.Key, cookie.Value);
            }
        }

        SettingsToolkit.WriteLocalSetting(SettingNames.LocalCookie, JsonSerializer.Serialize(localCookies));
    }

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
        var cookies = cookieInfo.Cookies.Select(p => (p.Name, p.Value)).ToDictionary();
        SettingsToolkit.WriteLocalSetting(Models.Constants.App.SettingNames.LocalCookie, JsonSerializer.Serialize(cookies));
    }

    private static async Task AccessBiliBiliAsync()
    {
        var req = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, CookieGetDomain, needToken: false, forceNoToken: true, needCookie: true);
        var res = await HttpProvider.Instance.SendAsync(req);
        var cookies = HttpProvider.GetCookieFromResponse(res);
        SaveCookies(cookies);
    }

    private static string GetCorrespondPath(long timestamp)
    {
        var publicKeyPEM = @"
            -----BEGIN PUBLIC KEY-----
            MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDLgd2OAkcGVtoE3ThUREbio0Eg
            Uc/prcajMKXvkCKFCWhJYJcLkcM2DKKcSeFpD/j6Boy538YXnR6VhcuUJOhH2x71
            nzPjfdTcqMz7djHum0qSZA0AyCBDABUqCrfNgCiJ00Ra7GmRj+YCK1NJEuewlb40
            JNrRuoEUXpabUzGB8QIDAQAB
            -----END PUBLIC KEY-----
        ";

        var dataToEncrypt = Encoding.UTF8.GetBytes($"refresh_{timestamp}");
        var oaepsha256 = RSAEncryptionPadding.OaepSHA256;
        var rsa = RSA.Create();
        rsa.ImportFromPem(publicKeyPEM);
        var encryptedData = rsa.Encrypt(dataToEncrypt, oaepsha256);

        var sb = new StringBuilder();
        foreach (var b in encryptedData)
        {
            sb.AppendFormat("{0:x2}", b);
        }

        return sb.ToString();
    }

    private async void OnQRTimerTickAsync(object sender, object e)
    {
        if (await IsTokenValidAsync())
        {
            StopQRLoginListener();
            return;
        }

        await CheckQRStatusAsync(_internalQRAuthCode);
    }

    private async Task CheckQRStatusAsync(string authCode, bool shouldSaveCookie = true)
    {
        CleanQRCodeCancellationToken();
        _qrPollCancellationTokenSource = new CancellationTokenSource();
        var queryParameters = new Dictionary<string, string>
        {
            { Query.AuthCode, authCode },
            { Query.LocalId, _guid },
            { Query.Guid, Guid.NewGuid().ToString() },
        };

        try
        {
            var httpProvider = HttpProvider.Instance;
            var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Post, Passport.QRCodeCheck, queryParameters, clientType: RequestClientType.Login, needToken: false);
            var response = await httpProvider.SendAsync(request, _qrPollCancellationTokenSource.Token);
            var result = await HttpProvider.ParseAsync<ServerResponse<TokenInfo>>(response);

            if (shouldSaveCookie)
            {
                SaveCookie(result.Data.CookieInfo);
            }

            SaveAuthorizeResult(result.Data);
            SettingsToolkit.WriteLocalSetting(SettingNames.IsWebSignIn, !shouldSaveCookie);
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

                if (qrStatus == QRCodeStatus.Failed)
                {
                    QRCodeScanFailed?.Invoke(this, new Exception($"QRCodeScanFailed - {se.Error.Code} | {se.Error.Message}"));
                }
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

    /// <summary>
    /// 获取登录二维码信息.
    /// </summary>
    /// <returns><see cref="QRInfo"/>.</returns>
    private async Task<QRInfo> GetQRCodeInfoAsync()
    {
        var queryParameters = new Dictionary<string, string>
            {
                { Query.LocalId, _guid },
            };
        var httpProvider = HttpProvider.Instance;
        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Post, Passport.QRCode, queryParameters, clientType: RequestClientType.Login, needToken: false);
        var response = await httpProvider.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse<QRInfo>>(response);
        return result.Data;
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

    private async Task TryUpdateCookieAsync()
    {
        var csrf = GetCsrfToken();
        if (string.IsNullOrEmpty(csrf))
        {
            return;
        }

        var query = new Dictionary<string, string>
        {
            { "csrf", csrf },
        };

        var request = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, Passport.CookieValidate, query, needCookie: true);
        var response = await HttpProvider.Instance.SendAsync(request);
        var result = await HttpProvider.ParseAsync<ServerResponse<CookieValidateResponse>>(response);

#if FIRST_RUN
        if (!(result.Data?.NeedRefresh ?? true))
        {
            return;
        }
#endif

        var timestamp = result.Data is not null ? result.Data.Timestamp : DateTimeOffset.Now.ToUnixTimeSeconds();
        var correspondPath = GetCorrespondPath(timestamp);
        var refreshUrl = Passport.RefreshCsrf(correspondPath);
        var htmlReq = await HttpProvider.GetRequestMessageAsync(HttpMethod.Get, refreshUrl, needCookie: true);
        var htmlRes = await HttpProvider.Instance.SendAsync(htmlReq);
        var html = await htmlRes.GetStringAsync();
        var docEle = new HtmlDocument();
        docEle.LoadHtml(html);
        var csrfNode = docEle.DocumentNode.SelectSingleNode("//div[@id='1-name']");
        var refreshCsrfToken = csrfNode.InnerText;
        var refreshParams = new Dictionary<string, string>
        {
            { "csrf", csrf },
            { "refresh_csrf", refreshCsrfToken },
            { "source", "main_web" },
            { "refresh_token",  _tokenInfo.RefreshToken },
        };

        var refreshReq = await HttpProvider.GetRequestMessageAsync(HttpMethod.Post, Passport.RefreshCookie, refreshParams, needCookie: true, needToken: false, forceNoToken: true);
        var refreshResponse = await HttpProvider.Instance.SendAsync(refreshReq);
        var refreshResult = await HttpProvider.ParseAsync<ServerResponse<CookieRefreshResponse>>(refreshResponse);

        if (!string.IsNullOrEmpty(refreshResult.Data.RefreshToken))
        {
            if (refreshResponse.Cookies.Count > 0)
            {
                SaveCookies(HttpProvider.GetCookieFromResponse(refreshResponse));
            }
            else
            {
                await AccessBiliBiliAsync();
            }

            var oldRefreshToken = _tokenInfo.RefreshToken;
            _tokenInfo.RefreshToken = refreshResult.Data.RefreshToken;
            SaveAuthorizeResult(_tokenInfo);

            var confirmParams = new Dictionary<string, string>
            {
                { "csrf", GetCsrfToken() },
                { "refresh_token", oldRefreshToken },
            };
            var confirmReq = await HttpProvider.GetRequestMessageAsync(HttpMethod.Post, Passport.ConfirmCookie, confirmParams, needCookie: true);
            _ = await HttpProvider.Instance.SendAsync(confirmReq);
        }
        else
        {
            Debug.WriteLine($"Cookie 刷新失败：{refreshResult.Data.Message}");
        }

        var newCsrf = GetCsrfToken();
        if (csrf == newCsrf)
        {
            Debug.WriteLine("妈的");
        }
    }
}
