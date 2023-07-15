// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.ViewModels;
using Windows.Foundation;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 剧集单集条目视图.
/// </summary>
public sealed class EpisodeItem : ReactiveControl<EpisodeItemViewModel>, IRepeaterItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EpisodeItem"/> class.
    /// </summary>
    public EpisodeItem()
        => DefaultStyleKey = typeof(EpisodeItem);

    /// <inheritdoc/>
    public Size GetHolderSize() => new Size(210, 248);
}
