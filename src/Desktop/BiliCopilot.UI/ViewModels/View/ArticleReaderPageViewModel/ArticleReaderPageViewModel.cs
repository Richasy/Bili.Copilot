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
            ArticleInitialized?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "加载文章内容时失败");
        }

        IsLoading = false;
    }
}
