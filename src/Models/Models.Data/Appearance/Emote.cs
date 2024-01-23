// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.Data.Appearance;

/// <summary>
/// 表情.
/// </summary>
public sealed class Emote
{
    /// <summary>
    /// 替代文本.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// 图片信息.
    /// </summary>
    public Image Image { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is Emote emote && Key == emote.Key;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Key);
}
