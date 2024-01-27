// Copyright (c) Bili Copilot. All rights reserved.

using System;
using Bili.Copilot.Models.Constants.App;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 影视圈页面视图模型.
/// </summary>
public sealed partial class FilmPageViewModel
{
    private static readonly Lazy<FilmPageViewModel> _lazyInstance = new(() => new FilmPageViewModel());

    private bool _isInitialized;

    [ObservableProperty]
    private FilmType _currentType;

    [ObservableProperty]
    private bool _isMovieShown;

    [ObservableProperty]
    private bool _isTvShown;

    [ObservableProperty]
    private bool _isDocumentaryShown;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private bool _isReloading;

    [ObservableProperty]
    private double _navListColumnWidth;

    /// <summary>
    /// 实例.
    /// </summary>
    public static FilmPageViewModel Instance => _lazyInstance.Value;
}
