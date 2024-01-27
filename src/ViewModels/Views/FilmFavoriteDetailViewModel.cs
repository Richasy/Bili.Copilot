// Copyright (c) Bili Copilot. All rights reserved.

using System;

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 追剧详情视图模型.
/// </summary>
public sealed class FilmFavoriteDetailViewModel : PgcFavoriteDetailViewModel
{
    private static readonly Lazy<FilmFavoriteDetailViewModel> _lazyInstance = new(() => new FilmFavoriteDetailViewModel());

    /// <summary>
    /// Initializes a new instance of the <see cref="FilmFavoriteDetailViewModel"/> class.
    /// </summary>
    private FilmFavoriteDetailViewModel()
     : base(Models.Constants.App.FavoriteType.Film)
    {
    }

    /// <summary>
    /// 实例.
    /// </summary>
    public static FilmFavoriteDetailViewModel Instance => _lazyInstance.Value;
}
