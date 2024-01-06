// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Data.User;

namespace Bili.Copilot.Models.Data.Dynamic;

/// <summary>
/// 动态视图.
/// </summary>
public sealed class DynamicView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicView"/> class.
    /// </summary>
    /// <param name="dynamics">动态列表.</param>
    /// <param name="ups">UP 主列表.</param>
    public DynamicView(
        IEnumerable<DynamicInformation> dynamics,
        IEnumerable<DynamicUper> ups,
        string footprint = default)
    {
        Dynamics = dynamics;
        Ups = ups;
        Footprint = footprint;
    }

    /// <summary>
    /// 动态列表.
    /// </summary>
    public IEnumerable<DynamicInformation> Dynamics { get; }

    /// <summary>
    /// 关注的 UP 主列表.
    /// </summary>
    public IEnumerable<DynamicUper> Ups { get; }

    /// <summary>
    /// 透传字段.
    /// </summary>
    public string Footprint { get; }
}
