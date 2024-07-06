// Copyright (c) Richasy. All rights reserved.

namespace Richasy.BiliKernel.Models;

/// <summary>
/// 稍后再看清空类型.
/// </summary>
public enum ViewLaterCleanType
{
    /// <summary>
    /// 删除全部视频.
    /// </summary>
    All = 0,

    /// <summary>
    /// 删除无效视频.
    /// </summary>
    Invalid = 1,

    /// <summary>
    /// 删除已观看视频.
    /// </summary>
    Completed = 2,
}
