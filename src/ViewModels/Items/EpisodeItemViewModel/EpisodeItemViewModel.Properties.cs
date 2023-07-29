// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using Bili.Copilot.Models.Data.Pgc;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 剧集单集视图模型.
/// </summary>
public sealed partial class EpisodeItemViewModel
{
    [ObservableProperty]
    private EpisodeInformation _data;

    [ObservableProperty]
    private string _playCountText;

    [ObservableProperty]
    private string _danmakuCountText;

    [ObservableProperty]
    private string _trackCountText;

    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private string _durationText;

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is EpisodeItemViewModel model && EqualityComparer<EpisodeInformation>.Default.Equals(Data, model.Data);

    /// <inheritdoc/>
    public override int GetHashCode() => Data.GetHashCode();
}
