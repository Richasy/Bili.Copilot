// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 视频项视图模型.
/// </summary>
public sealed partial class VideoItemViewModel
{
    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private Uri _cover;

    [ObservableProperty]
    private string? _author;

    [ObservableProperty]
    private string? _duration;

    [ObservableProperty]
    private string? _publishRelativeTime;

    [ObservableProperty]
    private Uri? _avatar;

    [ObservableProperty]
    private double? _playCount;

    [ObservableProperty]
    private string? _tagName;

    [ObservableProperty]
    private string? _recommendReason;
}
