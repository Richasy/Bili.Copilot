// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 可选择的视图模型.
/// </summary>
/// <typeparam name="T">数据类型.</typeparam>
public abstract partial class SelectableViewModel<T> : ViewModelBase
{
    [ObservableProperty]
    private T _data;

    [ObservableProperty]
    private bool _isSelected;

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectableViewModel{T}"/> class.
    /// </summary>
    public SelectableViewModel(T data) => Data = data;

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is SelectableViewModel<T> model && EqualityComparer<T>.Default.Equals(Data, model.Data);

    /// <inheritdoc/>
    public override int GetHashCode() => Data.GetHashCode();
}
