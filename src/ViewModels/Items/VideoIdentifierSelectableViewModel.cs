// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Data.Video;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 可选择的视频标识符视图模型.
/// </summary>
public sealed partial class VideoIdentifierSelectableViewModel : SelectableViewModel<VideoIdentifier>
{
    /// <summary>
    /// 索引.
    /// </summary>
    [ObservableProperty]
    private int _index;

    /// <summary>
    /// Initializes a new instance of the <see cref="VideoIdentifierSelectableViewModel"/> class.
    /// </summary>
    public VideoIdentifierSelectableViewModel(VideoIdentifier data)
        : base(data)
    {
    }
}
