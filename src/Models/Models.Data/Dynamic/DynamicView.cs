// Copyright (c) Bili Copilot. All rights reserved.

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
    public DynamicView(
        IEnumerable<DynamicInformation> dynamics)
        => Dynamics = dynamics;

    /// <summary>
    /// 动态列表.
    /// </summary>
    public IEnumerable<DynamicInformation> Dynamics { get; }
}
