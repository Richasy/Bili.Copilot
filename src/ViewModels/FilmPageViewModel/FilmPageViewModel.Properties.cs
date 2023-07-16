// Copyright (c) Bili Copilot. All rights reserved.

using System.Collections.Generic;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Data.Pgc;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 影视圈页面视图模型.
/// </summary>
public sealed partial class FilmPageViewModel
{
    private readonly Dictionary<FilmType, IEnumerable<SeasonInformation>> _seasonCaches;

    [ObservableProperty]
    private FilmType _currentType;

    [ObservableProperty]
    private bool _isMovieShown;

    [ObservableProperty]
    private bool _isTvShown;

    [ObservableProperty]
    private bool _isDocumentaryShown;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private string _title;

    /// <summary>
    /// 实例.
    /// </summary>
    public static FilmPageViewModel Instance { get; } = new();
}
