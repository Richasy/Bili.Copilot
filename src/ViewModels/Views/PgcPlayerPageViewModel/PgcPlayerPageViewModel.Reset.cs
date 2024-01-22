﻿// Copyright (c) Bili Copilot. All rights reserved.

namespace Bili.Copilot.ViewModels;

/// <summary>
/// PGC 播放页面视图模型.
/// </summary>
public sealed partial class PgcPlayerPageViewModel
{
    private void ResetOverview()
    {
        IsError = false;
        TryClear(Celebrities);
        IsShowCelebrities = false;
    }

    private void ResetOperation()
    {
        IsLiked = false;
        IsCoined = false;
        IsFavorited = false;
        IsTracking = false;
        FavoriteFoldersErrorText = default;
        IsFavoriteFoldersError = false;
        TryClear(FavoriteFolders);
    }

    private void ResetCommunityInformation()
    {
        PlayCountText = default;
        DanmakuCountText = default;
        CommentCountText = default;
        LikeCountText = default;
        CoinCountText = default;
        FavoriteCountText = default;
        RatingCountText = default;
    }

    private void ResetInterop()
    {
        _playNextEpisodeAction = default;
        PlayerDetail?.SetPlayNextAction(default);
        PlayerDetail?.SetPlayPreviousAction(default);
        IsVideoFixed = false;
    }

    private void ResetSections()
    {
        TryClear(Sections);
        TryClear(Episodes);
        TryClear(Seasons);
        TryClear(Extras);
        CurrentSection = null;
        CurrentEpisode = null;
        IsShowEpisodes = false;
        IsShowSeasons = false;
        IsShowComments = false;
        IsShowExtras = false;
        Comments.ClearData();
    }
}
