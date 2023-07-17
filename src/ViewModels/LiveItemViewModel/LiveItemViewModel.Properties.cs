// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using Bili.Copilot.Models.Data.Live;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 直播间条目的视图模型.
/// </summary>
public sealed partial class LiveItemViewModel
{
    [ObservableProperty]
    private LiveInformation _data;

    [ObservableProperty]
    private string _viewerCountText;

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is LiveItemViewModel model && EqualityComparer<LiveInformation>.Default.Equals(Data, model.Data);

    /// <inheritdoc/>
    public override int GetHashCode() => Data.GetHashCode();
}
