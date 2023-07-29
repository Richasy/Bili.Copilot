// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.ViewModels;
using Windows.Foundation;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 动态条目.
/// </summary>
public sealed class DynamicItem : ReactiveControl<DynamicItemViewModel>, IRepeaterItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicItem"/> class.
    /// </summary>
    public DynamicItem() => DefaultStyleKey = typeof(DynamicItem);

    /// <inheritdoc/>
    public Size GetHolderSize() => new(300, 200);
}
