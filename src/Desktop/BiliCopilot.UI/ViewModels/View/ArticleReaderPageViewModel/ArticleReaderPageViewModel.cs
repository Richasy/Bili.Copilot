// Copyright (c) Bili Copilot. All rights reserved.

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
        IArticleDiscoveryService service,
        ILogger<ArticleReaderPageViewModel> logger)
    {
        _service = service;
        _logger = logger;
    }

    [RelayCommand]
    private async Task InitializeAsync(ArticleIdentifier article)
    {
        Title = article.Title;
        Content = default;
        IsLoading = true;
        Article = article;
        try
        {
            Content = await _service.GetArticleContentAsync(article);
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
