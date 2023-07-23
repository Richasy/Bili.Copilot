// Copyright (c) Bili Copilot. All rights reserved.

using System.Text;
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
    public const string OSVersion = "16.5.1";

    /// <summary>
    /// 厂商.
    /// </summary>
    public const string Brand = "Apple";

    /// <summary>
    /// 手机系统.
    /// </summary>
    public const string Model = "iPhone 14 Pro";

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
    /// 获取 x-bili-aurora-eid.
    /// </summary>
    /// <param name="uid">用户Id，未登录则为0.</param>
    /// <returns>x-bili-aurora-eid.</returns>
    public static string GetAuroraEid(long uid)
    {
        if (uid == 0)
        {
            return string.Empty;
        }

        var resultByte = new List<byte>(64);

        // 1. 将 UID 字符串转为字节数组.
        var midByte = Encoding.UTF8.GetBytes(uid.ToString());

        // 2. 将字节数组逐位(记为第 i 位)与 b"ad1va46a7lza" 中第 (i % 12) 位进行异或操作, 作为结果数组第 i 位.
        for (var i = 0; i < midByte.Length; i++)
        {
            resultByte.Add((byte)(midByte[i] ^ Encoding.UTF8.GetBytes("ad1va46a7lza")[i % 12]));
        }

        // 3. 对字节数组执行 Base64 编码, 注意 no padding, 即得到 x-bili-aurora-eid.
        return Convert.ToBase64String(resultByte.ToArray()).TrimEnd('=');
    }

    /// <summary>
    /// 获取 trace-id.
    /// </summary>
    /// <returns>Trace id.</returns>
    public static string GetTraceId()
    {
        // 1. 生成 32 位随机字符串 random_id , Charset 为 0~9, a~z.
        var random_id = GenRandomString(32);
        var random_trace_id = new StringBuilder(40);

        // 2. 取 random_id 前 24 位, 作为 random_trace_id.
        random_trace_id.Append(random_id.Substring(0, 24));

        // 3. 初始化一个长度为 3 的数组 b_arr, 初始值都为 0.
        var b_arr = new sbyte[3];

        // 并获取当前时间戳
        var ts = DateTimeOffset.Now.ToUnixTimeSeconds();

        // 使用循环从高位到低位遍历 b_arr 数组, 循环体内执行以下逻辑:
        //  - 首先将 ts 右移 8 位
        //  - 然后根据条件向 b_arr 的第 i 位赋值:
        //    - 如果 (ts / 128) % 2的结果为0, 则 b_arr[i] = ts % 256
        //    - 否则 b_arr[i] = ts % 256 - 256
        for (var i = 2; i >= 0; i--)
        {
            ts >>= 8;
#pragma warning disable IDE0047
            b_arr[i] = ((ts / 128) % 2) == 0 ? (sbyte)(ts % 256) : (sbyte)((ts % 256) - 256);
#pragma warning restore IDE0047
        }

        // 4. 将数组 b_arr 中的每个元素逐个转换为两位的十六进制字符串并追加到 random_trace_id 中.
        for (var i = 0; i < 3; i++)
        {
            random_trace_id.Append(b_arr[i].ToString("x2"));
        }

        // 5. 将 random_id 的第 31, 32 个字符追加到 random_trace_id 中, 此时 random_trace_id 生成完毕, 应当为 32 位长度.
        random_trace_id.Append(random_id.Substring(30, 2));

        // 6. 最后, 按 `{random_trace_id}:{random_trace_id[16..32]}:0:0` 的顺序拼接起来, 即为 x-bili-trace-id
        var random_trace_id_final = new StringBuilder(64);
        random_trace_id_final.Append(random_trace_id);
        random_trace_id_final.Append(":");
        random_trace_id_final.Append(random_trace_id.ToString(16, 16));
        random_trace_id_final.Append(":0:0");
        return random_trace_id_final.ToString();
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

    private static string GenRandomString(int length)
    {
        var random = new Random();
        const string charset = "0123456789abcdefghijklmnopqrstuvwxyz";
        var randomString = new StringBuilder(length);
        for (var i = 0; i < length; i++)
        {
            randomString.Append(charset[random.Next(charset.Length)]);
        }

        return randomString.ToString();
    }
}
