// Copyright (c) Bili Copilot. All rights reserved.

using BiliCopilot.UI.Pages.Overlay;
using BiliCopilot.UI.ViewModels.Components;
using BiliCopilot.UI.ViewModels.Core;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Article;
using Richasy.BiliKernel.Models.Article;
using Richasy.WinUI.Share.ViewModels;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 文章阅读器页面视图模型.
/// </summary>
public sealed partial class ArticleReaderPageViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArticleReaderPageViewModel"/> class.
    /// </summary>
    public ArticleReaderPageViewModel(
        IArticleDiscoveryService discovery,
        IArticleOperationService operation,
        ILogger<ArticleReaderPageViewModel> logger,
        CommentMainViewModel comment)
    {
        _discoveryService = discovery;
        _operationService = operation;
        _logger = logger;
        CommentModule = comment;
    }

    [RelayCommand]
    private async Task InitializeAsync(ArticleIdentifier article)
    {
        Title = article.Title;
        Content = default;
        IsLoading = true;
        Article = article;
        IsCommentsOpened = false;
        CommentModule.Initialize(article.Id, Richasy.BiliKernel.Models.CommentTargetType.Article, Richasy.BiliKernel.Models.CommentSortType.Hot);
        try
        {
            Content = await _discoveryService.GetArticleContentAsync(article);
            InitializeUser();
            InitializeStats();
            ArticleInitialized?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "加载文章内容时失败");
        }

        IsLoading = false;
    }

    [RelayCommand]
    private async Task ToggleLikeAsync()
    {
        if (Article is null)
        {
            return;
        }

        var state = !IsLiked;
        try
        {
            await _operationService.ToggleLikeAsync(Article.Value, state ?? false);
            IsLiked = state;
            if (IsLiked ?? false)
            {
                LikeCount++;
            }
            else
            {
                LikeCount--;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "切换点赞状态时失败");
        }
    }

    [RelayCommand]
    private async Task ToggleFavoriteAsync()
    {
        if (Article is null)
        {
            return;
        }

        var state = !IsFavorited;
        try
        {
            await _operationService.ToggleFavoriteAsync(Article.Value, state ?? false);
            IsFavorited = state;
            if (IsFavorited ?? false)
            {
                FavoriteCount++;
            }
            else
            {
                FavoriteCount--;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "切换收藏状态时失败");
        }
    }

    [RelayCommand]
    private async Task ToggleCommentsAsync()
    {
        IsCommentsOpened = !IsCommentsOpened;

        if (IsCommentsOpened && CommentModule.Comments.Count == 0)
        {
            await CommentModule.LoadItemsCommand.ExecuteAsync(default);
        }
    }

    [RelayCommand]
    private void ShowUserSpace()
        => this.Get<NavigationViewModel>().NavigateToOver(typeof(UserSpacePage).FullName, Content.Author.Profile.User);

    private void InitializeUser()
    {
        if (Content is null)
        {
            return;
        }

        Author = Content.Author.Profile.User.Name;
        Avatar = Content.Author.Profile.User.Avatar.Uri;
        IsVip = Content.Author.Profile.IsVip ?? false;
        IsFollowed = Content.Author.Community.Relation == Richasy.BiliKernel.Models.User.UserRelationStatus.Following;
    }

    private void InitializeStats()
    {
        if (Content is null)
        {
            return;
        }

        IsLiked = Content.IsLiked;
        IsFavorited = Content.IsFavorited;
        LikeCount = Content.CommunityInformation.LikeCount ?? 0;
        CommentCount = Content.CommunityInformation.CommentCount ?? 0;
        FavoriteCount = Content.CommunityInformation.FavoriteCount ?? 0;
    }
}
