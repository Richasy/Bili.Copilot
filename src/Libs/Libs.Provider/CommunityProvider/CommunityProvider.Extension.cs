// Copyright (c) Bili Copilot. All rights reserved.

using Bilibili.Main.Community.Reply.V1;

namespace Bili.Copilot.Libs.Provider;

/// <summary>
/// 社区交互数据处理.
/// </summary>
public partial class CommunityProvider
{
    private (string Offset, string Baseline) _videoDynamicOffset;
    private (string Offset, string Baseline) _comprehensiveDynamicOffset;

    private CursorReq _mainCommentCursor;
    private CursorReq _detailCommentCursor;

    /// <summary>
    /// 实例.
    /// </summary>
    public static CommunityProvider Instance { get; } = new();
}
