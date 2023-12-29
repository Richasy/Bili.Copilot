// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 用于ItemsRepeater的条目.
/// </summary>
public interface IRepeaterItem
{
    /// <summary>
    /// 获取占位大小.
    /// </summary>
    /// <returns><see cref="Size"/>.</returns>
    Size GetHolderSize();
}
