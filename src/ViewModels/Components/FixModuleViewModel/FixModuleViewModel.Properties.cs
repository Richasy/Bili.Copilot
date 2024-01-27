// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.ObjectModel;
using Bili.Copilot.Models.Data.Local;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 固定模块视图模型.
/// </summary>
public sealed partial class FixModuleViewModel
{
    private static readonly Lazy<FixModuleViewModel> _lazyInstance = new(() => new FixModuleViewModel());

    /// <summary>
    /// 是否有固定的内容.
    /// </summary>
    [ObservableProperty]
    private bool _hasFixedItems;

    [ObservableProperty]
    private bool _isLoading;

    /// <summary>
    /// 实例.
    /// </summary>
    public static FixModuleViewModel Instance => _lazyInstance.Value;

    /// <summary>
    /// 固定条目集合.
    /// </summary>
    public ObservableCollection<FixedItem> FixedItemCollection { get; }
}
