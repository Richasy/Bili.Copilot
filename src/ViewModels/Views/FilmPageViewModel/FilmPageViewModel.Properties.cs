// Copyright (c) Bili Copilot. All rights reserved.

using Bili.Copilot.Models.Constants.App;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 影视圈页面视图模型.
/// </summary>
public sealed partial class FilmPageViewModel
{
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
    private bool _isFavoriteShown;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private bool _isReloading;

    /// <summary>
    /// 实例.
    /// </summary>
    public static FilmPageViewModel Instance { get; } = new();
}
