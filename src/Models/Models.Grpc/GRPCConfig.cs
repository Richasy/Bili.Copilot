// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bilibili.App.View.V1;
using Bilibili.Metadata;
using Bilibili.Metadata.Device;
using Bilibili.Metadata.Fawkes;
using Bilibili.Metadata.Locale;
using Bilibili.Metadata.Network;
using Google.Protobuf;

namespace Bili.Copilot.Models.Grpc;

/// <summary>
/// gRPC的请求配置.
/// </summary>
public class GRPCConfig
{
    /// <summary>
    /// 系统版本.
    /// </summary>
    public const string OSVersion = "14.6";

    /// <summary>
    /// 厂商.
    /// </summary>
    public const string Brand = "Apple";

    /// <summary>
    /// 手机系统.
    /// </summary>
    public const string Model = "iPhone 11";

    /// <summary>
    /// 应用版本.
    /// </summary>
    public const string AppVersion = "6.7.0";

    /// <summary>
    /// 构建标识.
    /// </summary>
    public const int Build = 6070600;

    /// <summary>
    /// 频道.
    /// </summary>
    public const string Channel = "bilibili140";

    /// <summary>
    /// 网络状况.
    /// </summary>
    public const int NetworkType = 2;

    /// <summary>
    /// 未知.
    /// </summary>
    public const int NetworkTF = 0;

    /// <summary>
    /// 未知.
    /// </summary>
    public const string NetworkOid = "46007";

    /// <summary>
    /// 未知.
    /// </summary>
    public const string Cronet = "1.21.0";

    /// <summary>
    /// 未知.
    /// </summary>
    public const string Buvid = "XZFD48CFF1E68E637D0DF11A562468A8DC314";

    /// <summary>
    /// 应用类型.
    /// </summary>
    public const string MobileApp = "iphone";

    /// <summary>
    /// 移动平台.
    /// </summary>
    public const string Platform = "iphone";

    /// <summary>
    /// 产品环境.
    /// </summary>
    public const string Envorienment = "prod";

    /// <summary>
    /// 应用Id.
    /// </summary>
    public const int AppId = 1;

    /// <summary>
    /// 国家或地区.
    /// </summary>
    public const string Region = "CN";

    /// <summary>
    /// 语言.
    /// </summary>
    public const string Language = "zh";

    /// <summary>
    /// Initializes a new instance of the <see cref="GRPCConfig"/> class.
    /// </summary>
    /// <param name="accessToken">访问令牌.</param>
    public GRPCConfig(string accessToken) => AccessToken = accessToken;

    /// <summary>
    /// 访问令牌.
    /// </summary>
    public string AccessToken { get; set; }

    /// <summary>
    /// 获取客户端在Fawkes系统中的信息标头.
    /// </summary>
    /// <returns>Base64字符串.</returns>
    public static string GetFawkesreqBin()
    {
        var msg = new FawkesReq
        {
            Appkey = MobileApp,
            Env = Envorienment,
        };
        return ToBase64(msg.ToByteArray());
    }

    /// <summary>
    /// 获取设备标头.
    /// </summary>
    /// <returns>Base64字符串.</returns>
    public static string GetDeviceBin()
    {
        var msg = new Device
        {
            AppId = AppId,
            MobiApp = MobileApp,
            Build = Build,
            Channel = Channel,
            Buvid = Buvid,
            Platform = Platform,
            Brand = Brand,
            Model = Model,
            Osver = OSVersion,
        };
        return ToBase64(msg.ToByteArray());
    }

    /// <summary>
    /// 获取网络标头.
    /// </summary>
    /// <returns>Base64字符串.</returns>
    public static string GetNetworkBin()
    {
        var msg = new Network
        {
            Type = Bilibili.Metadata.Network.NetworkType.Wifi,
            Oid = NetworkOid,
        };
        return ToBase64(msg.ToByteArray());
    }

    /// <summary>
    /// 获取限制标头.
    /// </summary>
    /// <returns>Base64字符串.</returns>
    public static string GetRestrictionBin()
    {
        var msg = new Restriction();

        return ToBase64(msg.ToByteArray());
    }

    /// <summary>
    /// 获取本地化标头.
    /// </summary>
    /// <returns>Base64字符串.</returns>
    public static string GetLocaleBin()
    {
        var msg = new Locale
        {
            CLocale = new LocaleIds(),
            SLocale = new LocaleIds(),
        };
        msg.CLocale.Language = Language;
        msg.CLocale.Region = Region;
        msg.SLocale.Language = Language;
        msg.SLocale.Region = Region;
        return ToBase64(msg.ToByteArray());
    }

    /// <summary>
    /// 将数据转换为Base64字符串.
    /// </summary>
    /// <param name="data">数据.</param>
    /// <returns>Base64字符串.</returns>
    public static string ToBase64(byte[] data) => Convert.ToBase64String(data).TrimEnd('=');

    /// <summary>
    /// 获取元数据标头.
    /// </summary>
    /// <returns>Base64字符串.</returns>
    public string GetMetadataBin()
    {
        var msg = new Metadata
        {
            AccessKey = AccessToken,
            MobiApp = MobileApp,
            Build = Build,
            Channel = Channel,
            Buvid = Buvid,
            Platform = Platform,
        };
        return ToBase64(msg.ToByteArray());
    }
}
