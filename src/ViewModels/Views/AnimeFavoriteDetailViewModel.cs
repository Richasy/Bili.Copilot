// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.ViewModels;

/// <summary>
/// 追番详情视图模型.
/// </summary>
public sealed class AnimeFavoriteDetailViewModel : PgcFavoriteDetailViewModel
{
    private AnimeFavoriteDetailViewModel()
        : base(Models.Constants.App.FavoriteType.Anime)
    {
    }

    /// <summary>
    /// 实例.
    /// </summary>
    public static AnimeFavoriteDetailViewModel Instance { get; } = new();
}
