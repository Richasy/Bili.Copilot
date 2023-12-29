// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.ViewModels;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 直播间视频卡片.
/// </summary>
public sealed class LiveItem : ReactiveControl<LiveItemViewModel>, IRepeaterItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LiveItem"/> class.
    /// </summary>
    public LiveItem()
        => DefaultStyleKey = typeof(LiveItem);

    /// <inheritdoc/>
    public Size GetHolderSize() => new(210, 248);
}
