// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using Bilibili.Main.Community.Reply.V1;

namespace Bili.Copilot.Libs.Provider;

/// <summary>
/// 社区交互数据处理.
/// </summary>
public partial class CommunityProvider
{
    private static readonly Lazy<CommunityProvider> _lazyInstance = new(() => new CommunityProvider());
    private readonly Dictionary<string, CursorReq> _mainCommentCursorCache;
    private readonly Dictionary<string, CursorReq> _detailCommentCursorCache;
    private (string Offset, string Baseline) _videoDynamicOffset;
    private (string Offset, string Baseline) _comprehensiveDynamicOffset;

    /// <summary>
    /// 实例.
    /// </summary>
    public static CommunityProvider Instance => _lazyInstance.Value;
}
