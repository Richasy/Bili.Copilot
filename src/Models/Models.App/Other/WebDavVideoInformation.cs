// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.App.Other;

/// <summary>
/// WebDAV 视频信息.
/// </summary>
public sealed class WebDavVideoInformation
{
    /// <summary>
    /// 获取或设置 WebDAV 配置.
    /// </summary>
    public WebDavConfig Config { get; set; }

    /// <summary>
    /// 获取或设置视频地址.
    /// </summary>
    public string Href { get; set; }

    /// <summary>
    /// 获取或设置内容类型.
    /// </summary>
    public string ContentType { get; set; }
}
