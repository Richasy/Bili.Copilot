﻿// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.Constants.App;

/// <summary>
/// 页面标识.
/// </summary>
public enum PageType
{
    /// <summary>
    /// 未指定页面.
    /// </summary>
    None,

    /// <summary>
    /// 分区（包括分区推荐和热门视频）.
    /// </summary>
    Partition,

    /// <summary>
    /// 流行视频，包括推荐视频、热门视频和综合排行榜.
    /// </summary>
    Popular,

    /// <summary>
    /// 直播.
    /// </summary>
    Live,

    /// <summary>
    /// 动态.
    /// </summary>
    Dynamic,

    /// <summary>
    /// 动漫（包括日漫和国创）
    /// </summary>
    Anime,

    /// <summary>
    /// 影视。包括纪录片、电影、电视剧.
    /// </summary>
    Film,

    /// <summary>
    /// 专栏.
    /// </summary>
    Article,

    /// <summary>
    /// 设置页面.
    /// </summary>
    Settings,

    /// <summary>
    /// WebDAV 页面.
    /// </summary>
    WebDav,

    /// <summary>
    /// 消息.
    /// </summary>
    Message,
}
