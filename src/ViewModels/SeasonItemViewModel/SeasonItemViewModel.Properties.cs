// Copyright (c) Bili Copilot. All rights reserved.

using System;
using System.Collections.Generic;
using Bili.Copilot.Models.Data.Pgc;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 剧集条目视图模型.
/// </summary>
public sealed partial class SeasonItemViewModel
{
    private readonly Action<SeasonItemViewModel> _additionalAction;

    [ObservableProperty]
    private SeasonInformation _data;

    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private bool _isShowRating;

    [ObservableProperty]
    private string _trackCountText;

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is SeasonItemViewModel model && EqualityComparer<SeasonInformation>.Default.Equals(Data, model.Data);

    /// <inheritdoc/>
    public override int GetHashCode() => Data.GetHashCode();
}
