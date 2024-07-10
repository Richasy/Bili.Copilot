// Copyright (c) Richasy. All rights reserved.

using System.Collections.Generic;

namespace Richasy.BiliKernel.Models.Moment;

/// <summary>
/// 动态视图.
/// </summary>
public sealed class MomentView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MomentView"/> class.
    /// </summary>
    public MomentView(
        IReadOnlyList<MomentInformation> moments,
        IReadOnlyList<MomentProfile>? users,
        string? offset,
        string? updateBaseline,
        string? footprint,
        bool? hasMoreMoments)
    {
        Moments = moments;
        Users = users;
        Offset = offset;
        UpdateBaseline = updateBaseline;
        Footprint = footprint;
        HasMoreMoments = hasMoreMoments;
    }

    /// <summary>
    /// 动态列表.
    /// </summary>
    public IReadOnlyList<MomentInformation> Moments { get; }

    /// <summary>
    /// 发布动态的用户列表.
    /// </summary>
    public IReadOnlyList<MomentProfile>? Users { get; }

    /// <summary>
    /// 偏移值，用于下一次请求.
    /// </summary>
    public string? Offset { get; }

    /// <summary>
    /// 更新动态列表的基线，用于历史记录的请求.
    /// </summary>
    public string? UpdateBaseline { get; }

    /// <summary>
    /// 透传字段，用于标记用户已读的动态.
    /// </summary>
    public string? Footprint { get; }

    /// <summary>
    /// 是否还有更多动态.
    /// </summary>
    public bool? HasMoreMoments { get; }
}
