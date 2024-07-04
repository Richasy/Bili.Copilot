// Copyright (c) Richasy. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using Richasy.BiliKernel.Bili.Authorization;
using Richasy.BiliKernel.Models;

namespace Richasy.BiliKernel.Authenticator;

/// <summary>
/// 基础授权器.
/// </summary>
public sealed partial class BasicAuthenticator
{
    private const string AndroidKey = "dfca71928277209b";
    private const string AndroidSecret = "b5475a8825547a4fc26c7d518eaaa02e";
    private const string IOSKey = "27eb53fc9058f8c3";
    private const string IOSSecret = "c2ed53a74eeefe3cf99fbd01d8c9c375";
    private const string WebKey = "aa1e74ee4874176e";
    private const string WebSecret = "54e6a9a31b911cd5fc0daa66ebf94bc4";
    private const string LoginKey = "4409e2ce8ffd12b8";
    private const string LoginSecret = "59b43e04ad6965f34319062b478f83dd";
    private const string BuildNumber = "5520400";

    private readonly IBiliCookiesResolver? _cookieResolver;
    private readonly IBiliTokenResolver? _tokenResolver;

#pragma warning disable IDE1006 // 命名样式
    private static readonly byte[] MIXIN_KEY_ENC_TAB =
#pragma warning restore IDE1006 // 命名样式
        [
            46, 47, 18, 2, 53, 8, 23, 32, 15, 50, 10, 31, 58, 3, 45, 35, 27, 43, 5, 49, 33, 9, 42,
            19, 29, 28, 14, 39, 12, 38, 41, 13, 37, 48, 7, 16, 24, 55, 40, 61, 26, 17, 0, 1, 60,
            51, 30, 4, 22, 25, 54, 21, 56, 59, 6, 63, 57, 62, 11, 36, 20, 34, 44, 52,
        ];

    private static string? _img;
    private static string? _sub;
    private static string? _wbi;

    private static string GetAppKey(BiliDeviceType deviceType)
    {
        return deviceType switch
        {
            BiliDeviceType.Android => AndroidKey,
            BiliDeviceType.Web => WebKey,
            BiliDeviceType.Login => LoginKey,
            _ => IOSKey,
        };
    }

    private static string GetAppSecret(BiliDeviceType deviceType)
    {
        return deviceType switch
        {
            BiliDeviceType.Android => AndroidSecret,
            BiliDeviceType.Web => WebSecret,
            BiliDeviceType.Login => LoginSecret,
            _ => IOSSecret,
        };
    }

    private static void InitializeDeviceParameters(Dictionary<string, string> parameters, BiliDeviceType device, bool onlyUseAppKey = false)
    {
        var appKey = GetAppKey(device);
        parameters.Add("appkey", appKey);

        if (onlyUseAppKey)
        {
            return;
        }

        switch (device)
        {
            case BiliDeviceType.Apple:
                {
                    parameters.Add("mobi_app", "iphone");
                    parameters.Add("platform", "ios");
                    parameters.Add("ts", NowWithSeconds().ToString());
                }
                break;
            case BiliDeviceType.Android:
                {
                    parameters.Add("mobi_app", "android");
                    parameters.Add("platform", "android");
                    parameters.Add("ts", NowWithSeconds().ToString());
                }
                break;
            case BiliDeviceType.Web:
                {
                    parameters.Add("platform", "web");
                    parameters.Add("ts", NowWithMilliSeconds().ToString());
                }
                break;
            default:
                {
                    parameters.Add("ts", NowWithSeconds().ToString());
                }
                break;
        }
    }

    private static string GenerateSign(Dictionary<string, string> parameters, BiliDeviceType device)
    {
        var secret = GetAppSecret(device);
        var query = GenerateQuery(parameters);
        return MD5Compute(query + secret);
    }

    private static string GenerateRID(Dictionary<string, string> parameters)
    {
        if (!parameters.ContainsKey("wts"))
        {
            parameters.Add("wts", NowWithSeconds().ToString());
        }

        var query = GenerateQuery(parameters);
        return MD5Compute(query + _wbi);
    }

    private static string GenerateQuery(Dictionary<string, string> parameters)
    {
        var queryParameters = parameters.OrderBy(p => p.Key).Select(p => $"{p.Key}={p.Value}").ToList();
        return string.Join("&", queryParameters);
    }

    private static long NowWithSeconds() => DateTimeOffset.Now.ToLocalTime().ToUnixTimeSeconds();

    private static long NowWithMilliSeconds() => DateTimeOffset.Now.ToLocalTime().ToUnixTimeMilliseconds();

    private static string MD5Compute(string input)
    {
        using var md5 = MD5.Create();
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = md5.ComputeHash(inputBytes);
        var sb = new StringBuilder();
        for (var i = 0; i < hashBytes.Length; i++)
        {
            sb.Append(hashBytes[i].ToString("x2"));
        }

        return sb.ToString().ToLower(System.Globalization.CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// 网页导航响应.
    /// </summary>
    /// <remarks>
    /// https://github.com/SocialSisterYi/bilibili-API-collect/blob/master/docs/misc/sign/wbi.md.
    /// </remarks>
    internal sealed class WebNavResponse
    {
        /// <summary>
        /// 未知.
        /// </summary>
        [JsonPropertyName("wbi_img")]
        internal WbiImage? Img { get; set; }

        /// <summary>
        /// Wbi image.
        /// </summary>
        internal sealed class WbiImage
        {
            /// <summary>
            /// 未知.
            /// </summary>
            [JsonPropertyName("img_url")]
            public string? ImgUrl { get; set; }

            /// <summary>
            /// 未知.
            /// </summary>
            [JsonPropertyName("sub_url")]
            public string? SubUrl { get; set; }
        }
    }
}
