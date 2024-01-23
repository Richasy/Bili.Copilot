// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.Data.Appearance;

/// <summary>
/// 表情包.
/// </summary>
public sealed class EmotePackage
{
    /// <summary>
    /// 名称.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 图标.
    /// </summary>
    public Image Icon { get; set; }

    /// <summary>
    /// 表情列表.
    /// </summary>
    public List<Emote> Images { get; set; }
}
