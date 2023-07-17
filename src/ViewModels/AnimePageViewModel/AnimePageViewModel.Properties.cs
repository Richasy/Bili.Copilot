// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Pgc;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 动漫页面的视图模型.
/// </summary>
public sealed partial class AnimePageViewModel
{
    private readonly Dictionary<AnimeDisplayType, IEnumerable<SeasonInformation>> _caches;

    private bool _isBangumiFinished;
    private bool _isDomesticFinished;

    [ObservableProperty]
    private AnimeDisplayType _currentType;

    [ObservableProperty]
    private bool _isBangumiShown;

    [ObservableProperty]
    private bool _isDomesticShown;

    [ObservableProperty]
    private bool _isTimelineShown;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private string _title;

    /// <summary>
    /// 实例.
    /// </summary>
    public static AnimePageViewModel Instance { get; } = new();
}
