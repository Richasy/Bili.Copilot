// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Richasy.WinUI.Share.ViewModels;

/// <summary>
/// 视图模型基类.
/// </summary>
public abstract class ViewModelBase : ObservableObject
{
}

/// <summary>
/// 数据视图模型基类.
/// </summary>
/// <typeparam name="TData">数据类型.</typeparam>
public abstract partial class ViewModelBase<TData> : ViewModelBase
    where TData : class
{
    [ObservableProperty]
    private TData _data;

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelBase{TData}"/> class.
    /// </summary>
    protected ViewModelBase(TData data) => Data = data;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ViewModelBase<TData> @base && EqualityComparer<TData>.Default.Equals(Data, @base.Data);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Data);
}
