﻿// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using Bili.Copilot.Models.App.Other;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels.Items;

/// <summary>
/// 导航条目.
/// </summary>
public sealed partial class NavigateItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private NavigateItem _data;

    [ObservableProperty]
    private string _displayTitle;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigateItemViewModel"/> class.
    /// </summary>
    public NavigateItemViewModel(NavigateItem data)
        => Data = data;

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is NavigateItemViewModel model && EqualityComparer<NavigateItem>.Default.Equals(Data, model.Data);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Data);
}
