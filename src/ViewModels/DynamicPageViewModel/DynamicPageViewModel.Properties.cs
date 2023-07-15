// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Dynamic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels.DynamicPageViewModel;

/// <summary>
/// 动态页面的视图模型.
/// </summary>
public sealed partial class DynamicPageViewModel
{
    private readonly Dictionary<DynamicDisplayType, IEnumerable<DynamicInformation>> _caches;
    private bool _isVideoEnd;
    private bool _isAllEnd;

    [ObservableProperty]
    private DynamicDisplayType _currentType;

    [ObservableProperty]
    private bool _isVideoShown;

    [ObservableProperty]
    private bool _isAllShown;

    [ObservableProperty]
    private bool _isEmpty;

    /// <summary>
    /// 实例.
    /// </summary>
    public static DynamicPageViewModel Instance { get; } = new();
}
