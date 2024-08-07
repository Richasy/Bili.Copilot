// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;

namespace BiliCopilot.UI.ViewModels.Items;

/// <summary>
/// 动态条目视图模型.
/// </summary>
public sealed partial class MomentItemViewModel
{
    [ObservableProperty]
    private double? _likeCount;

    [ObservableProperty]
    private double? _commentCount;

    [ObservableProperty]
    private bool _isLiked;
}
