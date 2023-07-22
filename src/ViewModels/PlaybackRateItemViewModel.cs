// Copyright (c) Bili Copilot. All rights reserved.

using System;
using CommunityToolkit.Mvvm.Input;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 播放速率条目视图模型.
/// </summary>
public sealed partial class PlaybackRateItemViewModel : SelectableViewModel<double>
{
    private readonly Action<double> _action;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlaybackRateItemViewModel"/> class.
    /// </summary>
    public PlaybackRateItemViewModel(double data, Action<double> action)
        : base(data)
    {
        _action = action;
    }

    [RelayCommand]
    private void Active() => _action?.Invoke(Data);
}
