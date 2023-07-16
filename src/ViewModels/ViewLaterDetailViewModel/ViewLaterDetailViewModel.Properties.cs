// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 稍后再看详情视图模型.
/// </summary>
public sealed partial class ViewLaterDetailViewModel
{
    private bool _isEnd;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _isClearing;

    /// <summary>
    /// 实例.
    /// </summary>
    public static ViewLaterDetailViewModel Instance { get; } = new();
}
