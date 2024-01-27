﻿// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Models.Constants.App;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 动漫页面的视图模型.
/// </summary>
public sealed partial class AnimePageViewModel
{
    private static readonly Lazy<AnimePageViewModel> _lazyInstance = new Lazy<AnimePageViewModel>(() => new AnimePageViewModel());
    private bool _isInitialized;

    [ObservableProperty]
    private AnimeDisplayType _currentType;

    [ObservableProperty]
    private bool _isBangumiShown;

    [ObservableProperty]
    private bool _isDomesticShown;

    [ObservableProperty]
    private bool _isTimelineShown;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private bool _isReloading;

    [ObservableProperty]
    private double _navListColumnWidth;

    /// <summary>
    /// 实例.
    /// </summary>
    public static AnimePageViewModel Instance => _lazyInstance.Value;
}
