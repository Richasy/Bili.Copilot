// Copyright (c) Bili Copilot. All rights reserved.

using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 历史记录详情模块的视图模型.
/// </summary>
public sealed partial class HistoryDetailViewModel
{
    private static readonly Lazy<HistoryDetailViewModel> _lazyInstance = new(() => new HistoryDetailViewModel());
    private bool _isEnd;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _isClearing;

    /// <summary>
    /// 实例.
    /// </summary>
    public static HistoryDetailViewModel Instance => _lazyInstance.Value;
}
