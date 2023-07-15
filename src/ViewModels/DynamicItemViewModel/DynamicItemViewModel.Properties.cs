// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Data.Dynamic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 动态条目的视图模型.
/// </summary>
public sealed partial class DynamicItemViewModel
{
    [ObservableProperty]
    private UserItemViewModel _publisher;

    [ObservableProperty]
    private DynamicInformation _data;

    [ObservableProperty]
    private bool _isLiked;

    [ObservableProperty]
    private string _likeCountText;

    [ObservableProperty]
    private string _commentCountText;

    [ObservableProperty]
    private bool _isShowCommunity;

    [ObservableProperty]
    private bool _canAddViewLater;
}
