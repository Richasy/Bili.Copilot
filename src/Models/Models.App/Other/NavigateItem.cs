// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.Models.App.Other;

/// <summary>
/// 导航条目.
/// </summary>
public sealed class NavigateItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NavigateItem"/> class.
    /// </summary>
    /// <param name="id">页面 Id.</param>
    /// <param name="title">标题.</param>
    public NavigateItem(PageType id, string title)
    {
        Id = id;
        Title = title;
    }

    /// <summary>
    /// 页面 Id.
    /// </summary>
    public PageType Id { get; }

    /// <summary>
    /// 标题.
    /// </summary>
    public string Title { get; }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is NavigateItem item && Id == item.Id;

    /// <inheritdoc/>
    public override int GetHashCode() => Id.GetHashCode();
}
