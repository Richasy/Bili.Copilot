// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.ViewModels;
using Windows.Foundation;

namespace Bili.Copilot.App.Controls.Base;

/// <summary>
/// 用来显示视频条目的 UI 单元.
/// </summary>
public sealed partial class VideoItem : ReactiveControl<VideoItemViewModel>, IRepeaterItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VideoItem"/> class.
    /// </summary>
    public VideoItem() => DefaultStyleKey = typeof(VideoItem);

    /// <inheritdoc/>
    public Size GetHolderSize() => new(400, 180);
}
