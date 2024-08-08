// Copyright (c) Bili Copilot. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Richasy.BiliKernel.Bili.Article;
using Richasy.BiliKernel.Models.Article;

namespace BiliCopilot.UI.ViewModels.View;

/// <summary>
/// 文章阅读器页面视图模型.
/// </summary>
public sealed partial class ArticleReaderPageViewModel
{
    private readonly IArticleDiscoveryService _service;
    private readonly ILogger<ArticleReaderPageViewModel> _logger;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _title;

    /// <summary>
    /// 文章已加载.
    /// </summary>
    public event EventHandler ArticleInitialized;

    /// <summary>
    /// 文章标识.
    /// </summary>
    public ArticleIdentifier? Article { get; private set; }

    /// <summary>
    /// 文章内容.
    /// </summary>
    public ArticleDetail? Content { get; private set; }
}
