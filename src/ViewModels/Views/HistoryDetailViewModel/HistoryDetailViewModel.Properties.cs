// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 历史记录详情模块的视图模型.
/// </summary>
public sealed partial class HistoryDetailViewModel
{
    private bool _isEnd;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _isClearing;

    /// <summary>
    /// 实例.
    /// </summary>
    public static HistoryDetailViewModel Instance { get; } = new();
}
