// Copyright (c) Richasy. All rights reserved.

using Richasy.BiliKernel.Models;

namespace Richasy.BiliKernel.Authenticator;

/// <summary>
/// 基础授权执行设置.
/// </summary>
public class BasicAuthorizeExecutionSettings
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BasicAuthorizeExecutionSettings"/> class.
    /// </summary>
    public BasicAuthorizeExecutionSettings(
        BiliDeviceType device = BiliDeviceType.Apple,
        bool useTokenIfExist = true,
        bool forceNoToken = false,
        bool useCookieIfExist = false,
        bool onlyUseAppKey = false,
        bool needRID = false,
        bool needCSRF = false,
        string? additionalQuery = default)
    {
        Device = device;
        UseTokenIfExist = useTokenIfExist;
        ForceNoToken = forceNoToken;
        UseCookieIfExist = useCookieIfExist;
        OnlyUseAppKey = onlyUseAppKey;
        NeedRID = needRID;
        NeedCSRF = needCSRF;
        AdditionalQuery = additionalQuery;
    }

    /// <summary>
    /// 设备类型.
    /// </summary>
    public BiliDeviceType Device { get; set; }

    /// <summary>
    /// 是否需要Token.
    /// </summary>
    public bool UseTokenIfExist { get; set; }

    /// <summary>
    /// 是否强制不使用Token.
    /// </summary>
    public bool ForceNoToken { get; set; }

    /// <summary>
    /// 是否需要Cookie.
    /// </summary>
    public bool UseCookieIfExist { get; set; }

    /// <summary>
    /// 是否仅使用AppKey.
    /// </summary>
    public bool OnlyUseAppKey { get; set; }

    /// <summary>
    /// 是否需要RID.
    /// </summary>
    public bool NeedRID { get; set; }

    /// <summary>
    /// 是否需要CSRF.
    /// </summary>
    public bool NeedCSRF { get; set; }

    /// <summary>
    /// 附加查询.
    /// </summary>
    public string? AdditionalQuery { get; set; }
}
