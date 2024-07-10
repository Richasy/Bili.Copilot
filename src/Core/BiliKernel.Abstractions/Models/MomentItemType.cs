// Copyright (c) Richasy. All rights reserved.

namespace Richasy.BiliKernel.Models;

/// <summary>
/// 动态类型.
/// </summary>
public enum MomentItemType
{
    /// <summary>
    /// 不支持的动态类型.
    /// </summary>
    Unsupported,

    /// <summary>
    /// 视频动态.
    /// </summary>
    Video,

    /// <summary>
    /// PGC 内容动态.
    /// </summary>
    Pgc,

    /// <summary>
    /// 文章动态.
    /// </summary>
    Article,

    /// <summary>
    /// 图文动态.
    /// </summary>
    Image,

    /// <summary>
    /// 纯文本动态.
    /// </summary>
    PlainText,

    /// <summary>
    /// 转发动态.
    /// </summary>
    Forward,
}
