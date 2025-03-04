// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Models.Constants;

namespace BiliCopilot.UI.Models;

/// <summary>
/// 固定项.
/// </summary>
public sealed class PinItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PinItem"/> class.
    /// </summary>
    public PinItem()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PinItem"/> class.
    /// </summary>
    public PinItem(string id, string title, string cover, PinContentType type)
    {
        Cover = cover;
        Title = title;
        Id = id;
        Type = type;
    }

    /// <summary>
    /// 封面.
    /// </summary>
    public string Cover { get; set; }

    /// <summary>
    /// 标题.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 标识符.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 类型.
    /// </summary>
    public PinContentType Type { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is PinItem item && Id == item.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Id);
}
