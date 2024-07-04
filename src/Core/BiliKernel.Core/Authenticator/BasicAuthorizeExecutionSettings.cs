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
        BiliApiType apiType = BiliApiType.App,
        bool useToken = true,
        bool forceNoToken = false,
        bool useCookie = false,
        bool onlyUseAppKey = false,
        bool needRID = false,
        bool needCSRF = false,
        string? additionalQuery = default)
    {
        ApiType = apiType;
        UseToken = useToken;
        ForceNoToken = forceNoToken;
        UseCookie = useCookie;
        OnlyUseAppKey = onlyUseAppKey;
        NeedRID = needRID;
        NeedCSRF = needCSRF;
        AdditionalQuery = additionalQuery;
    }

    /// <summary>
    /// 设备类型.
    /// </summary>
    public BiliApiType ApiType { get; set; }

    /// <summary>
    /// 是否需要Token.
    /// </summary>
    public bool UseToken { get; set; }

    /// <summary>
    /// 是否强制不使用Token.
    /// </summary>
    public bool ForceNoToken { get; set; }

    /// <summary>
    /// 是否需要Cookie.
    /// </summary>
    public bool UseCookie { get; set; }

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
