// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 信息流视图模型.
/// </summary>
/// <typeparam name="T">核心数据集合的类型.</typeparam>
public abstract partial class InformationFlowViewModel<T>
{
    private bool _isNeedLoadAgain;

    [ObservableProperty]
    private bool _isReloading;

    [ObservableProperty]
    private bool _isInitialized;

    [ObservableProperty]
    private bool _isIncrementalLoading;

    [ObservableProperty]
    private bool _isError;

    [ObservableProperty]
    private string _errorText;

    /// <summary>
    /// 数据集合.
    /// </summary>
    public ObservableCollection<T> Items { get; }
}
