// Copyright (c) Richasy. All rights reserved.

namespace Richasy.BiliKernel.Models;

/// <summary>
/// 哔哩 API 类型.
/// </summary>
public enum BiliApiType
{
    /// <summary>
    /// App API，需要用到 access key.
    /// </summary>
    App,

    /// <summary>
    /// 网页 API，需要用到 cookie.
    /// </summary>
    Web,
}
