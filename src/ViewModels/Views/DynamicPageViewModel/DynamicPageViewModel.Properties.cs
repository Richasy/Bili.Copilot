// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    private string _userDynamicOffset;
    private bool _isCurrentUserEnd;
    private string _allFootprint;

    [ObservableProperty]
    private DynamicDisplayType _currentType;

    [ObservableProperty]
    private bool _isVideoShown;

    [ObservableProperty]
    private bool _isAllShown;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _isCurrentSpaceEmpty;

    [ObservableProperty]
    private bool _isNoUps;

    [ObservableProperty]
    private bool _isAllDynamicSelected;

    [ObservableProperty]
    private UserItemViewModel _selectedUp;

    /// <summary>
    /// 实例.
    /// </summary>
    public static DynamicPageViewModel Instance { get; } = new();

    /// <summary>
    /// 显示的 UP 主列表.
    /// </summary>
    public ObservableCollection<UserItemViewModel> DisplayUps { get; }

    /// <summary>
    /// 用户空间的动态.
    /// </summary>
    public ObservableCollection<DynamicItemViewModel> UserSpaceDynamics { get; }
}
