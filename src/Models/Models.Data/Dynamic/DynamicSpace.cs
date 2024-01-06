// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.Data.Dynamic;

/// <summary>
/// 动态空间.
/// </summary>
public sealed class DynamicSpace
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicSpace"/> class.
    /// </summary>
    public DynamicSpace(IEnumerable<DynamicInformation> dynamics, string offset, bool hasMore)
    {
        Dynamics = dynamics;
        Offset = offset;
        HasMore = hasMore;
    }

    /// <summary>
    /// 偏移值.
    /// </summary>
    public string Offset { get; }

    /// <summary>
    /// 是否还有更多.
    /// </summary>
    public bool HasMore { get; }

    /// <summary>
    /// 动态列表.
    /// </summary>
    public IEnumerable<DynamicInformation> Dynamics { get; }
}
