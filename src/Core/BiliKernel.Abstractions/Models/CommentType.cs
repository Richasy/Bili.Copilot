﻿// Copyright (c) Richasy. All rights reserved.

namespace Richasy.BiliKernel.Models;

/// <summary>
/// 评论区类型.
/// </summary>
public enum CommentType
{
    /// <summary>
    /// 视频.
    /// </summary>
    Video = 1,

    /// <summary>
    /// 相簿/图片动态.
    /// </summary>
    Album = 11,

    /// <summary>
    /// 专栏文章.
    /// </summary>
    Article = 12,

    /// <summary>
    /// 音频.
    /// </summary>
    Music = 14,

    /// <summary>
    /// 纯文本动态/分享.
    /// </summary>
    Moment = 17,

    /// <summary>
    /// 课程.
    /// </summary>
    Course = 33,

    /// <summary>
    /// 不设置.
    /// </summary>
    None = 99,
}
