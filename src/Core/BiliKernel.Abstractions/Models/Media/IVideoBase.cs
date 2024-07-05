﻿// Copyright (c) Richasy. All rights reserved.

namespace Richasy.BiliKernel.Models.Media;

/// <summary>
/// 视频信息的基础接口.
/// </summary>
public interface IVideoBase
{
    /// <summary>
    /// 视频标识符.
    /// </summary>
    VideoIdentifier Identifier { get; }
}
